using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct CreatePackRequest
{
    [TdfMember("DCID")] 
    public uint mDCID;

    [TdfMember("PTYP")] 
    public uint mPTYP;
    
    [TdfMember("UID")] 
    public uint mUID;
    
}