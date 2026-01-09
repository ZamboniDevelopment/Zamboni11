using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct AssignCardsResponse
{
    
    [TdfMember("VER")] 
    public VersionInfo mVersionInfo;
    
}