using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ISViewTradeRequest
{
    [TdfMember("REM")] 
    public uint mRemove;

    [TdfMember("TID")] 
    public ulong mTradeId;
    
    [TdfMember("UID")] 
    public ulong mUserId;
    
}