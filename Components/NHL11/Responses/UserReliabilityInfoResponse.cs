using Tdf;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct UserReliabilityInfoResponse
{
    [TdfMember("DISC")] 
    public byte mPreviousMatchUnfinished;
    
    [TdfMember("MFI")] 
    public uint mMatchesFinished;
    
    [TdfMember("MST")] 
    public uint mMatchesStarted;
    
    [TdfMember("REL")] 
    public uint mReliability;
    
    [TdfMember("UID")] 
    public ulong mUserId;

}