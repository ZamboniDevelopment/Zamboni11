using Tdf;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct ApplySalaryCapResponse
{
    [TdfMember("PID")] 
    public long mPlayerCardId;
    
    [TdfMember("SAL")] 
    public short mSalaryCap;
    
    [TdfMember("UID")] 
    public long mUserId;

}