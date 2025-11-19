using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct DeckInfoRequest
{
    [TdfMember("PERS")] 
    public string mPersona;

    [TdfMember("UID")] 
    public long mUID;
    
    [TdfMember("VER")] 
    public VER mVER;
    
}