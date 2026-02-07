using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct UpdateFiltersRequest
{
    [TdfMember("FILT")] 
    public TickerFilter mTickerFilter;

    [TdfMember("IDEN")] 
    public ulong mBlazeId;
}