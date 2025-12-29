using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct CreatePackResponse
{
    [TdfMember("CDAT")] 
    public List<SCDL> mCDAT;

    [TdfMember("DUPL")] 
    public List<SDUP> mDUPL;
    
    [TdfMember("NUM")] 
    public uint mNUM;
    
    [TdfMember("PCNT")] 
    public long mPCNT;
    
    [TdfMember("PKTY")] 
    public uint mPKTY;
    
    [TdfMember("VER")] 
    public VER mVER;
    
}