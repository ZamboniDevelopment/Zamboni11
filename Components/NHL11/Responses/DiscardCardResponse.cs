using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct DiscardCardResponse
{
    [TdfMember("CRED")] 
    public uint mCRED;

    [TdfMember("VER")] 
    public VersionInfo mVersionInfo;
    
}