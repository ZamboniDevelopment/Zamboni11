using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct GamerGetInfoRequest
{
    [TdfMember("TUID")] 
    public ulong mTargetUserId;

    [TdfMember("UID")] 
    public ulong mUserId;
    
}