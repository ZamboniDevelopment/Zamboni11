using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ISSearchRequest
{
    [TdfMember("CAT")] 
    public int mCategory;

    [TdfMember("CTYP")] 
    public CardSearchTypeParameter mCardType;
    
    [TdfMember("FORM")] 
    public int mFormation;
    
    [TdfMember("LEAG")] 
    public int mLeagueId;
    
    [TdfMember("LEV")] 
    public int mLevel;

    [TdfMember("MACR")] 
    public int mMaxCredits;
    
    [TdfMember("MAXB")] 
    public int mMaxBuyPrice;
    
    [TdfMember("MICR")] 
    public int mMinCredits;
    
    [TdfMember("MINB")] 
    public int mMinBuyPrice;

    [TdfMember("MYTR")] 
    public int mMyTrades;
    
    [TdfMember("NAT")] 
    public int mNation;
    
    [TdfMember("NOAC")] 
    public int mNonActive;
    
    [TdfMember("POS")] 
    public int mPosition;

    [TdfMember("STRT")] 
    public int mStart;
    
    [TdfMember("TEAM")] 
    public int mTeamId;
    
    [TdfMember("UID")] 
    public ulong mUserId;
    
    [TdfMember("ZONE")] 
    public int mFieldZone;
    
}