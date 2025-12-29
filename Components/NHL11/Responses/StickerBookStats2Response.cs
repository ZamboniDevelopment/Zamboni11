using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct StickerBookStats2Response
{
    [TdfMember("STAT")] 
    public List<SSTAT> mSTAT;

}