using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ISGetOffersRequest
{
    [TdfMember("MSID")] 
    public uint mMSID;
    
    [TdfMember("NOAC")] 
    public uint mNonActive;
    
    [TdfMember("NUMR")] 
    public uint mNumRetrieve;
    
    [TdfMember("STRT")] 
    public uint mStart;
    
    [TdfMember("TID")] 
    public long mTradeId;
    
    [TdfMember("UID")] 
    public long mUserId;

}