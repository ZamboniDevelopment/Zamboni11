using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct StickerBookStats2Request
{
    [TdfMember("CONT")] 
    public byte mContextId;

    [TdfMember("UID")] 
    public long mUserId;
    
    [TdfMember("VALU")] 
    public uint mValue;
    
    [TdfMember("YEAR")] 
    public byte mYearId;
    
}