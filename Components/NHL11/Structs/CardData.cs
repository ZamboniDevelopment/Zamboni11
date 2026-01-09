using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct CardData
{
    [TdfMember("ATTR")] 
    public List<byte> mAttributes;

    [TdfMember("CDST")] 
    public byte mCDST;
    
    [TdfMember("CID")] 
    public long mCID;
    
    [TdfMember("DBID")] 
    public uint mDatabaseId; //The players databaseId, (Refer nhlng.db file and nhlviewng program)
    
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
    public byte mNumberOfOwners;

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
    public List<byte> mSTAT;
    
    [TdfMember("SUB")] 
    public short mSUB;
    
    [TdfMember("TIME")] 
    public uint mDateIssued;
    
    [TdfMember("TMID")] 
    public uint mTMID;
    
    [TdfMember("TRNG")] 
    public List<byte> mTRNG;
    
    [TdfMember("USRE")] 
    public byte mContractLenght;

}