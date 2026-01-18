using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ISWatchListRequest
{
    [TdfMember("NUM")] 
    public byte mPageSize;

    [TdfMember("ST")] 
    public short mStart;
    
    [TdfMember("UID")] 
    public ulong mUserId;
    
}