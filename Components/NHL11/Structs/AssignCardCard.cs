using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct AssignCardCard
{
    [TdfMember("CID")] 
    public long mCardId;

    [TdfMember("CSTT")] 
    public byte mCardStateId;
    
    [TdfMember("DPOS")] 
    public uint mDeckPos;
    
    [TdfMember("DTYP")] 
    public uint mDeckType;
    
}