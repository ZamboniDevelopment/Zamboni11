using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct StickerBookStatResult
{
    [TdfMember("CTTP")] 
    public byte mContextId;
    
    [TdfMember("CTVL")] 
    public uint mContextValue;
    
    [TdfMember("TYPE")] 
    public byte mTypeId;
    
    [TdfMember("VALU")] 
    public uint mValue;
    
}