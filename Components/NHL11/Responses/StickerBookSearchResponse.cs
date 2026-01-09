using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct StickerBookSearchResponse
{
    [TdfMember("SRES")] 
    public List<CardData> mSearchResults;

}