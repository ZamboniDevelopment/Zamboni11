using Tdf;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct UserReliabilityInfoResponse
{
    [TdfMember("DISC")] 
    public byte mPreviousMatchUnfinished;
    
    [TdfMember("MFI")] 
    public int mMatchesFinished;
    
    [TdfMember("MST")] 
    public int mMatchesStarted;
    
    [TdfMember("REL")] 
    public int mReliability;
    
    [TdfMember("UID")] 
    public long mUserId;

}