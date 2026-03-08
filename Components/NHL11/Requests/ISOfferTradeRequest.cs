using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ISOfferTradeRequest
{
    [TdfMember("CARD")] 
    public List<long> mCardList;
    
    [TdfMember("CRED")] 
    public uint mCredits;
    
    [TdfMember("TID")] 
    public long mTradeId;
    
    [TdfMember("UID")] 
    public ulong mUserId;

}