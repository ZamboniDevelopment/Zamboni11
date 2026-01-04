using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct FetchSettingsGroupsResponse
{
    
    [TdfMember("LGRP")] 
    public List<SettingGroup> mSettingGroupList;

}