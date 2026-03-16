using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct StoreGetPackTypesRequest
{
    [TdfMember("GPID")] 
    public int mGroupId;

    [TdfMember("UID")] 
    public ulong mUserId;
}