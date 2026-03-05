using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ISSearchRequest
{
    [TdfMember("CAT")] 
    public int mCategory;

    [TdfMember("CTYP")] 
    public int mCardType;
    
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


// ISSearchRequest = {
//     CAT = -1 (0xFFFFFFFF)
//     CTYP = -1 (0xFFFFFFFF)
//     FORM = -1 (0xFFFFFFFF)
//     LEAG = -1 (0xFFFFFFFF)
//     LEV = -1 (0xFFFFFFFF)
//     MACR = -1 (0xFFFFFFFF)
//     MAXB = -1 (0xFFFFFFFF)
//     MICR = -1 (0xFFFFFFFF)
//     MINB = -1 (0xFFFFFFFF)
//     MYTR = 2 (0x00000002)
//     NAT = -1 (0xFFFFFFFF)
//     NOAC = 1 (0x00000001)
//     NUMR = 50 (0x00000032)
//     POS = -1 (0xFFFFFFFF)
//     STRT = 0 (0x00000000)
//     TEAM = -1 (0xFFFFFFFF)
//     UID = 0 (0x0000000000000000)
//     ZONE = -1 (0xFFFFFFFF)
// }