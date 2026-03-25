using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct NumericRequest
{
    
    [TdfMember("UID")] 
    public long mUserId;
    
}