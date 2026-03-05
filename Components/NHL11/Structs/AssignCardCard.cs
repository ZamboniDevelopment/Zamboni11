using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct AssignCardCard
{
    [TdfMember("CID")] 
    public long mCardId;

    [TdfMember("CSTT")] 
    public CardState mCardStateId;
    
    [TdfMember("DPOS")] 
    public uint mDeckPos;
    
    [TdfMember("DTYP")] 
    public DeckType mDeckType;
    
}