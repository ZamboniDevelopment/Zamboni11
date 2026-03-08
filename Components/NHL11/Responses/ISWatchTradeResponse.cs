using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct ISWatchTradeResponse
{
    
    [TdfMember("VAL")] 
    public byte mValue;
    
}