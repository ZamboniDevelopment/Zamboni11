using Tdf;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct LoginResponse
{
    [TdfMember("ABBR")] 
    public string mTeamAbbreviation;

    [TdfMember("BNUS")] 
    public byte mBonusAwarded;

    [TdfMember("NAME")] 
    public string mTeamName;

    [TdfMember("RWRD")] 
    public byte mRewardType;
    
    [TdfMember("TNOW")] 
    public uint mRewardValue;
    
    [TdfMember("UID")] 
    public long mUserId;
}