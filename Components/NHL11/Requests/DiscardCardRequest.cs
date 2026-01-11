using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct DiscardCardRequest
{
    [TdfMember("CID")] 
    public long mCardId;

    [TdfMember("CRED")] 
    public uint mCredits;
    
    [TdfMember("UID")] 
    public long mUserId;
    
}