using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct GEN
{
    [TdfMember("CRED")] 
    public uint mCRED;

    [TdfMember("STAT")] 
    public List<ExampleResponse> mSTAT;
}