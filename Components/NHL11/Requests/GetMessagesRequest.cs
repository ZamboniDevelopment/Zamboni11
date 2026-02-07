using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct GetMessagesRequest
{
    [TdfMember("IDEN")] 
    public ulong mBlazeId;

}