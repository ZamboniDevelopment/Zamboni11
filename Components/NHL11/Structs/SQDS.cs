using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct SQDS
{
    [TdfMember("CHEM")] 
    public uint mCHEM;
    
    [TdfMember("FORM")] 
    public uint mFORM;
    
    [TdfMember("RTNG")] 
    public uint mRTNG;
    
    [TdfMember("SQID")] 
    public uint mSQID;
    
    [TdfMember("SQNM")] 
    public string mSQNM;

}