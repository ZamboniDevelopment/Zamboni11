using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct StaffBonusResponse
{
    [TdfMember("SDAT")] 
    public SDAT mSDAT;

}