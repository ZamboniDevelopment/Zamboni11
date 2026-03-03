using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct SquadLoadActiveRequest
{
    
    [TdfMember("TUID")] 
    public ulong mTargetUserId;
    
    [TdfMember("UID")] 
    public ulong mUserId;
    
}