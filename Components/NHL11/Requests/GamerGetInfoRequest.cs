using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct GamerGetInfoRequest
{
    [TdfMember("TUID")] 
    public long mTargetUserId;

    [TdfMember("UID")] 
    public long mUserId;
    
}