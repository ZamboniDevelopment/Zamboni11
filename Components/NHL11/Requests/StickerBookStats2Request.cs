using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct StickerBookStats2Request
{
    [TdfMember("CONT")] 
    public byte mCONT;

    [TdfMember("UID")] 
    public long mUID;
    
    [TdfMember("VALU")] 
    public uint mVALU;
    
    [TdfMember("YEAR")] 
    public byte mYEAR;
    
}