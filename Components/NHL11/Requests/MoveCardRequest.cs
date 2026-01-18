using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct MoveCardRequest
{
    
    [TdfMember("CID")] 
    public ulong mCardId;
    
    [TdfMember("DECK")] 
    public uint mDeckType;
    
    [TdfMember("SWAP")] 
    public ulong mSwapCardId;
    
    [TdfMember("UID")] 
    public ulong mUserId;
    
}