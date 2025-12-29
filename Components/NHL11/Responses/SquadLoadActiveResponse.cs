using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct SquadLoadActiveResponse
{
    [TdfMember("ACTV")] 
    public List<SCDL> mACTV;
    
    [TdfMember("SQAD")] 
    public SQAD mSQAD;
    
    [TdfMember("TUID")] 
    public long mTUID;

}