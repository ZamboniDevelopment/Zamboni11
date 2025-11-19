using System.Collections.Generic;
using Blaze3SDK.Blaze;
using Blaze3SDK.Blaze.Authentication;
using Blaze3SDK.Blaze.GameManager;
using BlazeCommon;

namespace Zamboni11;

public class ServerPlayer
{
    public ServerPlayer(BlazeServerConnection blazeServerConnection, UserIdentification userIdentification, UserSessionExtendedData extendedData, SessionInfo sessionInfo)
    {
        BlazeServerConnection = blazeServerConnection;
        UserIdentification = userIdentification;
        ExtendedData = extendedData;
        SessionInfo = sessionInfo;
        ServerManager.AddServerPlayer(this);
    }

    public BlazeServerConnection BlazeServerConnection { get; }
    public UserIdentification UserIdentification { get; set; }
    public UserSessionExtendedData ExtendedData { get; set; }
    public SessionInfo SessionInfo { get; set; }

    public ReplicatedGamePlayer ToReplicatedGamePlayer(byte slot, uint gameId)
    {
        return new ReplicatedGamePlayer
        {
            mAccountLocale = 1701729619,
            mCustomData = UserIdentification.mExternalBlob,
            mExternalId = UserIdentification.mExternalId,
            mGameId = gameId,
            mJoinedGameTimestamp = 0,
            mNetworkAddress = ExtendedData.mAddress,
            mPlayerAttribs = new SortedDictionary<string, string>(),
            mPlayerId = UserIdentification.mBlazeId,
            mPlayerName = UserIdentification.mName,
            mPlayerSessionId = (uint)UserIdentification.mBlazeId,
            mPlayerState = PlayerState.ACTIVE_CONNECTING,
            mSlotId = slot,
            mSlotType = SlotType.SLOT_PRIVATE,
            mTeamIndex = slot,
            mTeam = (ushort)(slot + 1),
            mUserGroupId = default
        };
    }
}