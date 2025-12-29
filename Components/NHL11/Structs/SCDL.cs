using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct SCDL
{
    [TdfMember("ATTR")] 
    public List<byte> mATTR;

    [TdfMember("CDST")] 
    public byte mCDST;
    
    [TdfMember("CID")] 
    public long mCID;
    
    [TdfMember("DBID")] 
    public uint mDBID;
    
    [TdfMember("FORM")] 
    public byte mFORM;

    [TdfMember("FREE")] 
    public byte mFREE;
    
    [TdfMember("FTNS")] 
    public byte mFTNS;

    [TdfMember("INJG")] 
    public byte mINJG;

    [TdfMember("INJT")] 
    public byte mINJT;
    
    [TdfMember("MORL")] 
    public byte mMORL;
    
    [TdfMember("OWNR")] 
    public byte mOWNR;

    [TdfMember("POSI")] 
    public byte mPOSI;
    
    [TdfMember("PRIC")] 
    public byte mPRIC;
    
    [TdfMember("RARE")] 
    public byte mRARE;

    [TdfMember("RTNG")] 
    public byte mRTNG;
    
    [TdfMember("SCAP")] 
    public short mSCAP;
    
    [TdfMember("STAT")] 
    public List<byte>  mSTAT;
    
    [TdfMember("SUB")] 
    public short mSUB;
    
    [TdfMember("TIME")] 
    public uint mTIME;
    
    [TdfMember("TMID")] 
    public uint mTMID;
    
    [TdfMember("TRNG")] 
    public List<byte> mTRNG;
    
    [TdfMember("USRE")] 
    public byte mUSRE;

}