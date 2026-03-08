using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ISStartRequest
{
    [TdfMember("CID")] 
    public long mCardId;

    [TdfMember("CRED")] 
    public uint mCredits;
    
    [TdfMember("OFTX")] 
    public string mOfferText;
    
    [TdfMember("PRD")] 
    public int mPeriod;

    [TdfMember("RESV")] 
    public uint mReserve;
    
    [TdfMember("UID")] 
    public ulong mUserId;
    
}