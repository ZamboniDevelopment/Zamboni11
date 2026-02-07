using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct TickerFilter
{
    [TdfMember("BOT_")] 
    public long mBottom;

    [TdfMember("TOP_")] 
    public long mTop;
    
}