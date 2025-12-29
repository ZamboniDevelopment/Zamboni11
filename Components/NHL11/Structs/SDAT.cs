using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct SDAT
{
    [TdfMember("ARM")] 
    public byte mARM;

    [TdfMember("BACK")] 
    public byte mBACK;

    [TdfMember("CON")] 
    public byte mCON;
    
    [TdfMember("FIT")] 
    public byte mFIT;

    [TdfMember("FOOT")] 
    public byte mFOOT;

    [TdfMember("GKD")] 
    public byte mGKD;
    
    [TdfMember("GKH")] 
    public byte mGKH;
    
    [TdfMember("GKK")] 
    public byte mGKK;

    [TdfMember("GKO")] 
    public byte mGKO;

    [TdfMember("GKP")] 
    public byte mGKP;
    
    [TdfMember("GKR")] 
    public byte mGKR;

    [TdfMember("HEAD")] 
    public byte mHEAD;

    [TdfMember("HIP")] 
    public byte mHIP;
    
    [TdfMember("LEG")] 
    public byte mLEG;

    [TdfMember("PDEF")] 
    public byte mPDEF;

    [TdfMember("PDR")] 
    public byte mPDR;
    
    [TdfMember("PHE")] 
    public byte mPHE;
    
    [TdfMember("PPAC")] 
    public byte mPPAC;

    [TdfMember("PPAS")] 
    public byte mPPAS;

    [TdfMember("PSH")] 
    public byte mPSH;
    
    [TdfMember("SHOU")] 
    public byte mSHOU;

    [TdfMember("TALK")] 
    public byte mTALK;
}