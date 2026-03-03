using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct ApplyCardResponse
{
    
    [TdfMember("CDAT")] 
    public List<CardData> mCardDataList;
    
    [TdfMember("CID")] 
    public long mCardId;
    
    [TdfMember("UID")] 
    public ulong mUserId;
    
}