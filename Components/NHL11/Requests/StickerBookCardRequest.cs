using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct StickerBookCardRequest
{
    [TdfMember("CID")] 
    public long mCardId;

    [TdfMember("SWAP")] 
    public long mSwapCardId;
    
    [TdfMember("UID")] 
    public ulong mUserId;
}