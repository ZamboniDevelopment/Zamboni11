using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct ISOfferInfo
{
    [TdfMember("CARD")] 
    public List<long> mCardList;

    [TdfMember("CDAT")] 
    public List<CardData> mCardDataList;
    
    [TdfMember("CRED")] 
    public uint mCredits;

    [TdfMember("OID")] 
    public long mOfferId;
    
    [TdfMember("STAT")] 
    public OfferState mOfferState;

    [TdfMember("TID")] 
    public long mTradeId;
    
    [TdfMember("UID")] 
    public ulong mUserId;

}