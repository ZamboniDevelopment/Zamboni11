using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct GamerSetInfoRequest
{
    [TdfMember("INFO")] 
    public INFO mINFO;

    [TdfMember("UID")] 
    public long mUID;
    
}