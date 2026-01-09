using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct Stats
{
    [TdfMember("CTTP")] 
    public byte mCTTP;
    
    [TdfMember("CTVL")] 
    public uint mCTVL;
    
    [TdfMember("TYPE")] 
    public byte mTYPE;
    
    [TdfMember("VALU")] 
    public uint mVALU;
    
}