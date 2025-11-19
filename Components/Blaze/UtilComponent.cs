using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blaze3SDK.Blaze;
using Blaze3SDK.Blaze.Util;
using Blaze3SDK.Components;
using BlazeCommon;

namespace Zamboni11.Components.Blaze;

internal class UtilComponent : UtilComponentBase.Server
{
    public override Task<PreAuthResponse> PreAuthAsync(PreAuthRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new PreAuthResponse
        {
            mAnonymousChildAccountsEnabled = true,
            mAuthenticationSource = "",
            mComponentIds = new List<ushort>
            {
                1, 4, 5, 7, 9, 10, 11, 12, 13, 15, 21, 2148, 2249, 2250, 30722
            },
            mConfig = new FetchConfigResponse
            {
                mConfig = new SortedDictionary<string, string>
                {
                    { "connIdleTimeout", "120s" },
                    { "defaultRequestTimeout", "80s" },
                    { "pingPeriod", "20s" },
                    { "voipHeadsetUpdateRate", "1000" }
                }
            },
            mInstanceName = "nhl-2016-ps3",
            mLegalDocGameIdentifier = "Zamboni",
            mParentalConsentEntitlementGroupName = "Zamboni",
            mParentalConsentEntitlementTag = "Zamboni",
            mPersonaNamespace = "",
            mPlatform = request.mClientInfo.mPlatform,
            mQosSettings = new QosConfigInfo
            {
                mBandwidthPingSiteInfo = new QosPingSiteInfo
                {
                    mAddress = Program.GameServerIp,
                    mPort = 17502,
                    mSiteName = "qos"
                },
                mNumLatencyProbes = 10,
                mPingSiteInfoByAliasMap = new SortedDictionary<string, QosPingSiteInfo>
                {
                    {
                        "qos", new QosPingSiteInfo
                        {
                            mAddress = Program.GameServerIp,
                            mPort = 17502,
                            mSiteName = "qos"
                        }
                    }
                },
                mServiceId = 67
            },
            mRegistrationSource = "",
            mServerVersion = Program.Name,
            mUnderageSupported = true
        });
    }

    public override Task<PostAuthResponse> PostAuthAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new PostAuthResponse
        {
            // mPssConfig = new PssConfig
            // {
            //     mAddress = Program.GameServerIp,
            //     mInitialReportTypes = PssReportTypes.None,
            //     mNpCommSignature = new byte[]
            //     {
            //     },
            //     mOfferIds = new List<string>(),
            //     mPort = 7667,
            //     mProjectId = "",
            //     mTitleId = 0
            // },
            mTelemetryServer = GetTele(),
            mTickerServer = GetTicker(),
            mUserOptions = new UserOptions
            {
                mTelemetryOpt = TelemetryOpt.TELEMETRY_OPT_OUT,
                mUserId = 301116
            }
        });
    }

    public override Task<PingResponse> PingAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new PingResponse
        {
            mServerTime = (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
        });
    }

    public override Task<GetTelemetryServerResponse> GetTelemetryServerAsync(GetTelemetryServerRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(GetTele());
    }

    public override Task<UserSettingsLoadAllResponse> UserSettingsLoadAllAsync(UserSettingsLoadAllRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new UserSettingsLoadAllResponse
        {
            mDataMap = new SortedDictionary<string, string>()
        });
    }

    public override Task<UserSettingsResponse> UserSettingsLoadAsync(UserSettingsLoadRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new UserSettingsResponse
        {
            mData = "",
            mKey = ""
        });
    }

    public override Task<LocalizeStringsResponse> LocalizeStringsAsync(LocalizeStringsRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new LocalizeStringsResponse
        {
            mLocalizedStrings = new SortedDictionary<string, string>
            {
                {
                    "A", "B"
                }
            }
        });
    }

    private GetTelemetryServerResponse GetTele()
    {
        return new GetTelemetryServerResponse
        {
            mAddress = Program.GameServerIp,
            mDisable = "disa",
            mFilter = "filt",
            mIsAnonymous = false,
            mKey = "key",
            mLocale = 1701729619,
            mNoToggleOk = "nook",
            mPort = 6767,
            mSendDelay = 10,
            mSendPercentage = 10,
            mSessionID = "id",
            mUseServerTime = "true"
        };
    }

    private GetTickerServerResponse GetTicker()
    {
        return new GetTickerServerResponse
        {
            mAddress = Program.GameServerIp,
            mKey = "key",
            mPort = 6776
        };
    }


    public override Task<FetchConfigResponse> FetchClientConfigAsync(FetchClientConfigRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new FetchConfigResponse
        {
            mConfig = new SortedDictionary<string, string>()
        });
    }

    // public virtual Task<FilterUserTextResponse> FilterForProfanityAsync(FilterUserTextResponse request, BlazeRpcContext context)
    // {
    //     return Task.FromResult(new FilterUserTextResponse
    //     {
    //         mFilteredTextList = new List<FilteredUserText>
    //         {
    //             new FilteredUserText
    //             {
    //                 mFilteredText = request.mFilteredTextList[0].mFilteredText,
    //                 mResult = FilterResult.FILTER_RESULT_PASSED
    //             }
    //         }
    //     });
    // }
}