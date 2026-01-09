using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct GeneralInfo
{
    [TdfMember("CRED")] 
    public uint mCRED;

    [TdfMember("STAT")] 
    public List<byte> mSTAT;
}