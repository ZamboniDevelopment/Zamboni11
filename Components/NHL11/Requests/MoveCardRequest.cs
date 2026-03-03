using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct MoveCardRequest
{
    
    [TdfMember("CID")] 
    public long mCardId;
    
    [TdfMember("DECK")] 
    public uint mDeckType;
    
    [TdfMember("SWAP")] 
    public long mSwapCardId;
    
    [TdfMember("UID")] 
    public ulong mUserId;
    
}