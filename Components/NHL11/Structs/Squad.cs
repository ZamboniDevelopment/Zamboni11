using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct Squad
{
    [TdfMember("CHEM")] 
    public uint mCHEM;
    
    [TdfMember("CHNG")] 
    public uint mCHNG;
    
    [TdfMember("FORM")] 
    public uint mFORM;
    
    [TdfMember("LINE")] 
    public List<uint> mLINE;
    
    [TdfMember("MNGR")] 
    public Card mMNGR;
    
    [TdfMember("NAME")] 
    public string mNAME;

    [TdfMember("PLRS")] 
    public List<Card> mPLRS;
    
    [TdfMember("RTNG")] 
    public uint mRTNG;
    
    [TdfMember("SQID")] 
    public uint mSQID;
}