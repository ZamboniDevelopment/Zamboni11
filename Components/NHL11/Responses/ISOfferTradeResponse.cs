using Tdf;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct ISOfferTradeResponse
{
    
    [TdfMember("OID")] 
    public long mOfferId;
    
}