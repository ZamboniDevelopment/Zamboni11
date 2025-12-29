using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct SDUP
{
    [TdfMember("CID")] 
    public long mCID;

    [TdfMember("DCID")] 
    public long mDCID;
}