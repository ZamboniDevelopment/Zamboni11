using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct SquadListResponse
{
    [TdfMember("ACTV")] 
    public uint mACTV;
    
    [TdfMember("SQDS")] 
    public List<SQDS> mSQDS;

}