using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct GamerSetInfoRequest
{
    [TdfMember("INFO")] 
    public GamerInfo mGamerInfo;

    [TdfMember("UID")] 
    public long mUserId;
    
}