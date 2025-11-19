using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct MessageRequest
{
    [TdfMember("LCAL")] 
    public string mLocale;

    [TdfMember("MODE")] 
    public string mMode;

    [TdfMember("TITL")] 
    public string mTitle;

    [TdfMember("TOKN")] 
    public string mToken;
}