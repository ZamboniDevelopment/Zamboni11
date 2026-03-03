using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct SquadSmall
{
    [TdfMember("CHEM")] 
    public uint mChemistry;
    
    [TdfMember("FORM")] 
    public uint mFormation;
    
    [TdfMember("RTNG")] 
    public uint mRating;
    
    [TdfMember("SQID")] 
    public uint mSquadId;
    
    [TdfMember("SQNM")] 
    public string mSquadName;

}