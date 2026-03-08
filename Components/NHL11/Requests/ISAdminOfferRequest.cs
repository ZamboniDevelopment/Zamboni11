using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ISAdminOfferRequest
{
    [TdfMember("OID")] 
    public long mOfferId;
    
    [TdfMember("STAT")] 
    public OfferState mOfferState;
    
    [TdfMember("UID")] 
    public ulong mUserId;

}