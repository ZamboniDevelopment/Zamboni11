using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct LoginRequest
{
    [TdfMember("CP")] 
    public uint mCreatePlayer;

    [TdfMember("PERS")] 
    public string mPersona;

    [TdfMember("PUR")] 
    public uint mPurchased;

    [TdfMember("UID")] 
    public ulong mUserId;
}