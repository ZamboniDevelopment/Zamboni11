using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct MoveCardResponse
{
    [TdfMember("CID")] 
    public long mDisplacedCardId;

    [TdfMember("DECK")] 
    public uint mDisplacedDeckType;
    
    [TdfMember("POS")] 
    public uint mDisplacedCardPosition;
    
    [TdfMember("VER")] 
    public VersionInfo mVersionInfo;
}