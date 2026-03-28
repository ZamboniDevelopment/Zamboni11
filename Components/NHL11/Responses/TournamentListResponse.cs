using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct TournamentListResponse
{
    [TdfMember("TNOW")] 
    public uint mServerTime;
    
    [TdfMember("TRNS")] 
    public List<TournamentInfo> mTournaments;
    
}