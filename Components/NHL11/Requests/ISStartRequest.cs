using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ISStartRequest
{
    [TdfMember("CID")] 
    public long mCardId;

    [TdfMember("CRED")] 
    public int mCredits;
    
    [TdfMember("OFTX")] 
    public string mOfferText;
    
    [TdfMember("PRD")] 
    public int mPeriod; //Duration seconds

    [TdfMember("RESV")] 
    public int mReserve;
    
    [TdfMember("UID")] 
    public ulong mUserId;
    
}