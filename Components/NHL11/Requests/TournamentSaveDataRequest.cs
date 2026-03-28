using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct TournamentSaveDataRequest
{
    [TdfMember("DATA")] 
    public byte[] mData;
    
    [TdfMember("TYPE")] 
    public SaveTournamentType mTournamentType;
    
    [TdfMember("UID")] 
    public long mUserId;

}