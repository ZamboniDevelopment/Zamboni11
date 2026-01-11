using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct VersionInfo
{
    [TdfMember("VESC")] 
    public uint mVersionEscrow;

    [TdfMember("VGEN")] 
    public uint mVersionGeneral;
    
    [TdfMember("VUNA")] 
    public uint mVersionUnassigned;
}