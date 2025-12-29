using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct SquadLoadActiveRequest
{
    
    [TdfMember("TUID")] 
    public long mTUID;
    
    [TdfMember("UID")] 
    public long mUID;
    
}