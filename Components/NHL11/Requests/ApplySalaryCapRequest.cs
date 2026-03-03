using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ApplySalaryCapRequest
{
    [TdfMember("PID")] 
    public long mPlayerCardId;
    
    [TdfMember("SAL")] 
    public short mSalaryCap;
    
    [TdfMember("UID")] 
    public long mUserId;

}