using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct CardIdPair
{
    [TdfMember("CID")] 
    public long mCardId;

    [TdfMember("DCID")] 
    public long mDuplicateCardId;
}