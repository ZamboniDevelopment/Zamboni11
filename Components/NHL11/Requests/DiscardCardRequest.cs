using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct DiscardCardRequest
{
    [TdfMember("CID")] 
    public long mCID;

    [TdfMember("CRED")] 
    public uint mCRED;
    
    [TdfMember("UID")] 
    public long mUID;
    
}