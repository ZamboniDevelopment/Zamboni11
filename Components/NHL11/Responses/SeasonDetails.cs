using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct SeasonDetails
{
    [TdfMember("NRST")] 
    public long mNextRegularSeasonStart;

    [TdfMember("PET")] 
    public long mPlayOffEnd;
    
    [TdfMember("PST")] 
    public long mPlayOffStart;

    [TdfMember("RET")] 
    public long mRegularSeasonEnd;
    
    [TdfMember("RST")] 
    public long mRegularSeasonStart;

    [TdfMember("SID")] 
    public uint mSeasonID;
    
    [TdfMember("SNUM")] 
    public uint mSeasonNumber;

    [TdfMember("STAT")] 
    public SeasonState mSeasonState;
}