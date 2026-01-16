using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct SettingString
{
    [TdfMember("DEF")] 
    public string mDefault;

    [TdfMember("HLAB")] 
    public string mHelpLabel;

    [TdfMember("ID")] 
    public string mId;

    [TdfMember("LABL")] 
    public string mLabel;

    [TdfMember("LOCF")] 
    public uint mLocalizedFields;

    [TdfMember("TOGG")] 
    public uint mToggles;
    
}