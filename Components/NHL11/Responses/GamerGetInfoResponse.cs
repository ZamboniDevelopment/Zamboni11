using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct GamerGetInfoResponse
{
    [TdfMember("INFO")] 
    public GamerInfo mGamerInfo;

    [TdfMember("UID")] 
    public long mUID;
    
}