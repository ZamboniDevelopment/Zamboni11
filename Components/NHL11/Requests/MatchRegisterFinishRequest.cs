using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct MatchRegisterFinishRequest
{
    [TdfMember("ID")] 
    public long mId;

    [TdfMember("STAT")] 
    public MatchState mMatchState;

    [TdfMember("UID")] 
    public ulong mUserId;
}