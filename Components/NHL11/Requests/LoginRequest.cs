using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct LoginRequest
{
    [TdfMember("CP")] 
    public uint mCP;

    [TdfMember("PERS")] 
    public string mPersona;

    [TdfMember("PUR")] 
    public uint mPUR;

    [TdfMember("UID")] 
    public ulong mUID;
}