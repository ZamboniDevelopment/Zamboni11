using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct CreatePackRequest
{
    [TdfMember("DCID")] 
    public uint mCardDbId;

    [TdfMember("PTYP")] 
    public uint mPackType;
    
    [TdfMember("UID")] 
    public uint mUserId;
    
}