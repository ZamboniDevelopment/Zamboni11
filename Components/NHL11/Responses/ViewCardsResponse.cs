using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct ViewCardsResponse
{
    [TdfMember("CDAT")] 
    public List<CardData> mCardDataList;
    
}