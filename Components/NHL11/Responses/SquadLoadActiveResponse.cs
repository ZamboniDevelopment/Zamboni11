using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct SquadLoadActiveResponse
{
    [TdfMember("ACTV")] 
    public List<CardData> mActiveCards;
    
    [TdfMember("SQAD")] 
    public SquadInfo mSquadInfo;
    
    [TdfMember("TUID")] 
    public long mTUID;

}