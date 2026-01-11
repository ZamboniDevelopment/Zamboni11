using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct LogoutRequest
{
    [TdfMember("DU")] 
    public uint mDiscardUnassigned;

    [TdfMember("UID")] 
    public long mUserId;
    
}