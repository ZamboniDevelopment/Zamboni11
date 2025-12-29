using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct LogoutRequest
{
    [TdfMember("DU")] 
    public uint mDU;

    [TdfMember("UID")] 
    public long mUID;
    
}