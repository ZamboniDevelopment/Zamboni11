using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ISWatchTradeRequest
{
    [TdfMember("TID")] 
    public long mTradeId;

    [TdfMember("UID")] 
    public long mUserId;
    
}