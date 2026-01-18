using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct ISViewTradeResponse
{
    
    [TdfMember("CRED")] 
    public uint mCredits;
    
    [TdfMember("INFO")] 
    public ISTradeInfo mISTradeInfo;
    
}