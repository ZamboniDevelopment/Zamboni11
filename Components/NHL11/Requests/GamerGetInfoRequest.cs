using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct GamerGetInfoRequest
{
    [TdfMember("TUID")] 
    public long mCP;

    [TdfMember("UID")] 
    public long mPersona;
    
}