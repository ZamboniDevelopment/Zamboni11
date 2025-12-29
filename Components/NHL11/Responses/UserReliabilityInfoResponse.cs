using Tdf;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct UserReliabilityInfoResponse
{
    [TdfMember("DISC")] 
    public byte mDISC;
    
    [TdfMember("MFI")] 
    public uint mMFI;
    
    [TdfMember("MST")] 
    public uint mMST;
    
    [TdfMember("REL")] 
    public uint mREL;
    
    [TdfMember("UID")] 
    public long mUID;

}