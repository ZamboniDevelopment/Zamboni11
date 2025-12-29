using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct SquadLoadActiveResponse
{
    [TdfMember("ACTV")] 
    public List<Card> mACTV;
    
    [TdfMember("SQAD")] 
    public Squad mSquad;
    
    [TdfMember("TUID")] 
    public long mTUID;

}