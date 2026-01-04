using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct SettingViewData
{
    [TdfMember("DEFS")] 
    public string mDEFS;

    [TdfMember("HLAB")] 
    public string mHLAB;

    [TdfMember("ID")] 
    public string mID;

    [TdfMember("TOGG")] 
    public uint mTOGG;
    
    [TdfMember("VAL")] 
    public string mVAL;
    
}