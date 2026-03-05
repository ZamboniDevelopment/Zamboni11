using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ActivateCardRequest
{
    [TdfMember("ATYP")] 
    public CardState mActiveState;
    
    [TdfMember("CID")] 
    public long mCardId;
    
    [TdfMember("UID")] 
    public long mUserId;

}