using System.Collections.Generic;
using System.Threading.Tasks;
using BlazeCommon;
using Zamboni11.Components.NHL11.Bases;
using Zamboni11.Components.NHL11.Responses;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11;

internal class OSDKSettingsComponent : OSDKSettingsComponentBase.Server
{
    public override Task<FetchSettingsResponse> FetchSettingsAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new FetchSettingsResponse
        {
            mIntegerSettingList = new List<SettingInteger>
            {
                new SettingInteger
                {
                    mDefault = 0,
                    mHelpLabel = "1",
                    mId = "1",
                    mLabel = "1",
                    mLocalizedFields = 0,
                    mPossibleValueMap = new SortedDictionary<uint, string>()
                    {
                        {
                            0, "zero"
                        },
                        {
                            1, "one"
                        }
                    },
                    mToggles = 1
                }
            },
            mStringSettingList = new List<SettingString>
            {
                new SettingString
                {
                    mDefault = "2",
                    mHelpLabel = "2",
                    mId = "2",
                    mLabel = "2",
                    mLocalizedFields = 0,
                    mToggles = 1
                }
            }
        });
    }

    public override Task<FetchSettingsGroupsResponse> FetchSettingsGroupsAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new FetchSettingsGroupsResponse
        {
            mSettingGroupList = new List<SettingGroup>
            {
                new SettingGroup
                {
                    mID = "1",
                    mLSET = new List<string>
                    {
                        "1", "2"
                    },
                    mViewList = new List<SettingView>
                    {
                        new SettingView
                        {
                            mID = "1",
                            mSettingViewDataList = new List<SettingViewData>
                            {
                                new SettingViewData
                                {
                                    mDefaultStr = "1",
                                    mHelpLabel = "1",
                                    mId = "1",
                                    mToggles = 1,
                                    mValueStr = "1"
                                }
                            }
                        },
                        new SettingView
                        {
                            mID = "2",
                            mSettingViewDataList = new List<SettingViewData>
                            {
                                new SettingViewData
                                {
                                    mDefaultStr = "2",
                                    mHelpLabel = "2",
                                    mId = "2",
                                    mToggles = 1,
                                    mValueStr = "2"
                                }
                            }
                        }
                    }
                }
            }
        });
    }
}