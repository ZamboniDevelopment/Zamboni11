using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct StorePackTypeData
{
    [TdfMember("ATTR")] 
    public StorePackAttribute mAttributes;

    [TdfMember("AVL")] 
    public StorePackAvailability mAvailability;
    
    [TdfMember("CCST")] 
    public uint mCoinCost;

    [TdfMember("END")] 
    public uint mEndDate;
    
    [TdfMember("ID")] 
    public StorePackId mId;

    [TdfMember("QTY")] 
    public int mQuantity;
    
    [TdfMember("SLTP")] 
    public StoreSaleType mSaleType;

    [TdfMember("STRT")] 
    public uint mStartDate;
    
    [TdfMember("STTE")] 
    public StorePackState mState;
    
}