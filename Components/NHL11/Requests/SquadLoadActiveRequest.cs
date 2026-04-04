using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct SquadLoadActiveRequest
{
    
    [TdfMember("TUID")] 
    public long mTargetUserId;
    
    [TdfMember("UID")] 
    public long mUserId;
    
}