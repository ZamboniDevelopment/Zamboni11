using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct DIVL
{
    [TdfMember("NUM")] 
    public uint mNUM;

    [TdfMember("SIZE")] 
    public byte mSTAT;
    
    [TdfMember("TRUL")] 
    public TournamentRule mTournamentRule;
}