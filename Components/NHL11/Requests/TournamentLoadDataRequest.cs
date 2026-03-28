using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct TournamentLoadDataRequest
{
    
    [TdfMember("TYPE")] 
    public SaveTournamentType mTournamentType;
    
    [TdfMember("UID")] 
    public long mUserId;

}