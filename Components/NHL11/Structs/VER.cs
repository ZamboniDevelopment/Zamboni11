using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct VER
{
    [TdfMember("VESC")] 
    public uint mVESC;

    [TdfMember("VGEN")] 
    public uint mVGEN;
    
    [TdfMember("VUNA")] 
    public uint mVUNA;
}