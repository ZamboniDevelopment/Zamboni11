using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct INFO

{
    [TdfMember("CTAC")] 
    public string mCTAC;

    [TdfMember("FORM")] 
    public uint mFORM;

    [TdfMember("KTAK")] 
    public string mKTAK;

    [TdfMember("LINE")] 
    public string mLINE;

    [TdfMember("LOGU")] 
    public string mLOGU;

    [TdfMember("NAME")] 
    public string mNAME;

    [TdfMember("PLYQ")] 
    public uint mPLYQ;

    [TdfMember("PLYW")] 
    public uint mPLYW;

    [TdfMember("QTAC")] 
    public string mQTAC;

    [TdfMember("SPBT")] 
    public uint mSPBT;

    [TdfMember("TMAB")] 
    public string mTMAB;

    [TdfMember("TOUR")] 
    public string mTOUR;

    [TdfMember("TPPL")] 
    public uint mTPPL;

    [TdfMember("TROP")] 
    public string mTROP;
}