using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct SeasonDetailsResponse
{
    [TdfMember("NRST")] 
    public long mNRST;

    [TdfMember("PET")] 
    public long mPET;
    
    [TdfMember("PST")] 
    public long mPST;

    [TdfMember("RET")] 
    public long mRET;
    
    [TdfMember("RST")] 
    public long mRST;

    [TdfMember("SID")] 
    public uint mSeasonID;
    
    [TdfMember("SNUM")] 
    public uint mSeasonNumber;

    [TdfMember("STAT")] 
    public SeasonState MSeasonState;
}