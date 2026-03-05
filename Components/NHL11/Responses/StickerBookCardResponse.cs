using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct StickerBookCardResponse
{
    [TdfMember("CRED")] 
    public uint mTotalCredits;
    
    [TdfMember("VER")] 
    public VersionInfo mVersionInfo;

}