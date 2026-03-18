using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct StickerBookStatResult
{
    [TdfMember("CTTP")] 
    public ResultContext mContextId;
    
    [TdfMember("CTVL")] 
    public int mContextValue;
    
    [TdfMember("TYPE")] 
    public ResultType mTypeId;
    
    [TdfMember("VALU")] 
    public int mValue;
    
}