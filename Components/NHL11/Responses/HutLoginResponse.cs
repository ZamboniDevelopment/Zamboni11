using Tdf;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct HutLoginResponse
{
    [TdfMember("ABBR")] 
    public string mTeamAbbreviation;

    [TdfMember("BNUS")] 
    public byte mBNUS;

    [TdfMember("NAME")] 
    public string mTeamName;

    [TdfMember("RWRD")] 
    public byte mRWRD;
    
    [TdfMember("TNOW")] 
    public uint mTNOW;
    
    [TdfMember("UID")] 
    public long mUID;
}