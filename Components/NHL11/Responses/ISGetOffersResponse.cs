using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct ISGetOffersResponse
{
    
    [TdfMember("LIST")] 
    public List<ISOfferInfo> mOfferList;
    
    [TdfMember("TOTC")] 
    public int mTotalCount;
    
}