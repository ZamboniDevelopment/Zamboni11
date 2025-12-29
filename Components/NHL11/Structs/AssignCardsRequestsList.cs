using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct AssignCardsRequestsList
{
    [TdfMember("CID")] 
    public long mCID;

    [TdfMember("CSTT")] 
    public byte mCSTT;
    
    [TdfMember("DPOS")] 
    public uint mDPOS;
    
    [TdfMember("DTYP")] 
    public uint mDTYP;
    
}