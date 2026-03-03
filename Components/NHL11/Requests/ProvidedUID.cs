using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ProvidedUID
{
    
    [TdfMember("UID")] 
    public long mUserId;
    
}