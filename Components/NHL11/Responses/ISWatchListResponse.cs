using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct ISWatchListResponse
{
    
    [TdfMember("SRES")] 
    public List<ISTradeInfo> mTradeResults;
    
    [TdfMember("TOTC")] 
    public uint mTotalCount;
    
}