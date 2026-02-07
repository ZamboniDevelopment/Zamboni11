using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct SettingGroup
{
    [TdfMember("ID")] 
    public string mId;

    [TdfMember("LSET")] 
    public List<string> mSettingList;

    [TdfMember("LVWS")] 
    public List<SettingView> mViewList;
    
}