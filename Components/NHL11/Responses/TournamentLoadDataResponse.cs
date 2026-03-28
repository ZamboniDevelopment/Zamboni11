using Tdf;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct TournamentLoadDataResponse
{
    
    [TdfMember("DATA")] 
    public byte[] mData;
    
}