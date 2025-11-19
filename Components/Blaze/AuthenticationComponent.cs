using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Blaze3SDK.Blaze;
using Blaze3SDK.Blaze.Authentication;
using Blaze3SDK.Blaze.Util;
using Blaze3SDK.Components;
using BlazeCommon;
using Tdf;
using XI5;

namespace Zamboni11.Components.Blaze;

internal class AuthenticationComponent : AuthenticationComponentBase.Server
{
    public override Task<ConsoleLoginResponse> Ps3LoginAsync(PS3LoginRequest request, BlazeRpcContext context)
    {
        var ticket = new XI5Ticket(request.mPS3Ticket);

        //Still unsure what EXBB is. Research concluded its
        //`externalblob` binary(36) DEFAULT NULL COMMENT 'sizeof(SceNpId)==36',
        //"SceNpId", Its 36 bytes long, it starts with PSN Username and suffixed with other data in the end
        //This taken straight from https://github.com/hallofmeat/Skateboard3Server/blob/master/src/Skateboard3Server.Blaze/Handlers/Authentication/LoginHandler.cs
        var externalBlob = new List<byte>();
        externalBlob.AddRange(Encoding.ASCII.GetBytes(ticket.OnlineId.PadRight(20, '\0')));
        externalBlob.AddRange(Encoding.ASCII.GetBytes(ticket.Domain));
        externalBlob.AddRange(Encoding.ASCII.GetBytes(ticket.Region));
        externalBlob.AddRange(Encoding.ASCII.GetBytes("ps3"));
        externalBlob.Add(0x0);
        externalBlob.Add(0x1);
        externalBlob.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });

        var extendedData = new UserSessionExtendedData
        {
            mAddress = null!,
            mBestPingSiteAlias = "qos",
            mBlazeObjectIdList = new List<BlazeObjectId>(),
            mClientAttributes = new SortedDictionary<uint, int>(),
            mClientData = null,
            mCountry = "",
            mDataMap = null,
            mHardwareFlags = HardwareFlags.None,
            mLatencyList = new List<int>
            {
                10
            },
            mQosData = new NetworkQosData
            {
                mDownstreamBitsPerSecond = 10,
                mNatType = NatType.NAT_TYPE_MODERATE,
                mUpstreamBitsPerSecond = 10
            },
            mUserInfoAttribute = 0
        };

        var userIdentification = new UserIdentification
        {
            mAccountId = (long)ticket.UserId,
            mAccountLocale = 1701729619,
            mBlazeId = (long)ticket.UserId,
            mExternalBlob = externalBlob.ToArray(),
            mExternalId = ticket.UserId,
            mName = ticket.OnlineId
        };

        var sessionInfo = new SessionInfo
        {
            mBlazeUserId = (long)ticket.UserId,
            mEmail = "",
            mIsFirstLogin = false,
            mLastLoginDateTime = 0,
            mPersonaDetails = new PersonaDetails
            {
                mDisplayName = ticket.OnlineId,
                mExtId = ticket.UserId,
                mExtType = ExternalRefType.BLAZE_EXTERNAL_REF_TYPE_PS3,
                mLastAuthenticated = 0,
                mPersonaId = (long)ticket.UserId,
                mStatus = PersonaStatus.ACTIVE
            },
            mSessionKey = "key",
            mUserId = (long)ticket.UserId
        };

        new ServerPlayer(context.BlazeConnection, userIdentification, extendedData, sessionInfo);

        Task.Run(async () =>
        {
            await Task.Delay(500);

            await UserSessionsBase.Server.NotifyUserAddedAsync(context.BlazeConnection, new NotifyUserAdded
            {
                mExtendedData = extendedData,
                mUserInfo = userIdentification
            });

            await Task.Delay(600);

            await UserSessionsBase.Server.NotifyUserSessionExtendedDataUpdateAsync(context.BlazeConnection, new UserSessionExtendedDataUpdate
            {
                mExtendedData = extendedData,
                mUserId = userIdentification.mAccountId
            });

            await Task.Delay(800);

            await UserSessionsBase.Server.NotifyUserUpdatedAsync(context.BlazeConnection, new UserStatus
            {
                mBlazeId = userIdentification.mBlazeId,
                mStatusFlags = UserDataFlags.Online
            });
        });
        // TODO NHL11 working
        return Task.FromResult(new ConsoleLoginResponse
        {
            mCanAgeUp = false,
            mIsOfLegalContactAge = false,
            mLegalDocHost = "a",
            mNeedsLegalDoc = false,
            mPrivacyPolicyUri = "a",
            mSessionInfo = sessionInfo
            // mTermsOfServiceUri = "a",
            // mTosHost = "b",
            // mTosUri = "c"
        });
    }

    public override Task<NullStruct> LogoutAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new NullStruct());
    }
}