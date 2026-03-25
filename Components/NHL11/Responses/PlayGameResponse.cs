using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct PlayGameResponse
{
    
    [TdfMember("BNUS")] 
    public byte mBonusAwarded;
    
    [TdfMember("CRED")] 
    public int mCredits;
    
    [TdfMember("GTIC")] 
    public int mGoldenTickets;
    
    [TdfMember("PRES")] 
    public int mPrestige;
    
    [TdfMember("TRPH")] 
    public byte mTrophyCardCreated;
    
    [TdfMember("VER")] 
    public VersionInfo mVersionInfo;
    
}