using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct StickerBookSearchRequest
{
    [TdfMember("COLL")] 
    public byte mCollectionYearId;

    [TdfMember("COUN")] 
    public uint mCountryId;
    
    [TdfMember("CTYP")] 
    public CollectionSearchType mCollectionSearchCardType;
    
    [TdfMember("FORM")] 
    public uint mFormation;
    
    [TdfMember("LEAG")] 
    public uint mLeagueId;
    
    [TdfMember("LEV")] 
    public CardLevel mCardLevel;
    
    [TdfMember("NAT")] 
    public uint mNation;
    
    [TdfMember("NUMR")] 
    public int mNumRetreive;
    
    [TdfMember("POS")] 
    public uint mPosition;
    
    [TdfMember("STAT")] 
    public CardState mCardState;
    
    [TdfMember("STRT")] 
    public int mStart;
    
    [TdfMember("TEAM")] 
    public uint mTeamId;
    
    [TdfMember("UID")] 
    public long mUserId;

}