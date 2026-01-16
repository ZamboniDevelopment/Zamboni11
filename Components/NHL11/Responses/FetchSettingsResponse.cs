using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct FetchSettingsResponse
{
    
    [TdfMember("LSIN")] 
    public List<SettingInteger> mIntegerSettingList;

    [TdfMember("LSST")] 
    public List<SettingString> mStringSettingList;
    
}