using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct HutLogoutRequest
{
    [TdfMember("DU")] 
    public uint mDU;

    [TdfMember("UID")] 
    public long mUID;
    
}