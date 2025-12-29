using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct SquadSaveRequest
{
    
    [TdfMember("CHEM")] 
    public uint mCHEM;
    
    [TdfMember("FORM")] 
    public uint mFORM;
    
    [TdfMember("LINE")] 
    public List<uint> mLINE;
    
    [TdfMember("MNGR")] 
    public long mMNGR;
    
    [TdfMember("NAME")] 
    public string mNAME;
    
    [TdfMember("PLRS")] 
    public List<long> mPLRS;
    
    [TdfMember("RTNG")] 
    public uint mRTNG;
    
    [TdfMember("SQID")] 
    public uint mSQID;
    
    [TdfMember("UID")] 
    public long mUID;
}