using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct SeasonDetailsRequest
{
    [TdfMember("SID")] 
    public uint mSeasonId;

}