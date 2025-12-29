using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct DeckInfoResponse
{
    [TdfMember("DUPE")] 
    public List<SDUP> mDUPE;

    [TdfMember("DUPU")] 
    public List<SDUP> mDUPU;
    
    [TdfMember("ECDL")] 
    public List<SCDL> mECDL;
    
    [TdfMember("ECNT")] 
    public byte mECNT;
    
    [TdfMember("GEN")] 
    public GEN mGEN;
    
    [TdfMember("RATE")] 
    public uint mRATE;
    
    [TdfMember("UCDL")] 
    public List<SCDL> mUCDL;
    
    [TdfMember("UID")] 
    public long mUID;
    
    [TdfMember("VER")] 
    public Version MVersion;
}