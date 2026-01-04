using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct SettingView
{
    [TdfMember("ID")] 
    public string mID;

    [TdfMember("LVDS")] 
    public List<SettingViewData> mSettingViewDataList;
    
}