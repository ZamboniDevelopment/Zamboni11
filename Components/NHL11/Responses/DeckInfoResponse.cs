using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct DeckInfoResponse
{
    [TdfMember("DUPE")] 
    public List<ExampleResponse> mDUPE;

    [TdfMember("DUPU")] 
    public List<ExampleResponse> mDUPU;
    
    [TdfMember("ECDL")] 
    public List<ExampleResponse> mECDL;
    
    [TdfMember("ECNT")] 
    public byte mECNT;
    
    [TdfMember("GEN")] 
    public GEN mGEN;
    
    [TdfMember("UCDL")] 
    public List<ExampleResponse> mUCDL;
    
    [TdfMember("UID")] 
    public long mUID;
    
    [TdfMember("VER")] 
    public VER mVER;
}