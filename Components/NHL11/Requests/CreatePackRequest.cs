using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct CreatePackRequest
{
    [TdfMember("DCID")] 
    public uint mCardDbId;

    [TdfMember("PTYP")] 
    public PackType mPackType;
    
    [TdfMember("UID")] 
    public uint mUserId;
    
}