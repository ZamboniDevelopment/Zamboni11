using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct StickerBookSearchRequest
{
    [TdfMember("COLL")] 
    public byte mCOLL;

    [TdfMember("COUN")] 
    public int mCOUN;
    
    [TdfMember("CTYP")] 
    public CollectionSearchType mCollectionSearchType;
    
    [TdfMember("FORM")] 
    public int mFORM;
    
    [TdfMember("LEAG")] 
    public int mLEAG;
    
    [TdfMember("LEV")] 
    public CardLevel mCardLevel;
    
    [TdfMember("NAT")] 
    public int mNAT;
    
    [TdfMember("NUMR")] 
    public int mNUMR;
    
    [TdfMember("POS")] 
    public int mPOS;
    
    [TdfMember("STAT")] 
    public CardState mCardState;
    
    [TdfMember("STRT")] 
    public int mSTRT;
    
    [TdfMember("TEAM")] 
    public int mTEAM;
    
    [TdfMember("UID")] 
    public long mUID;

}