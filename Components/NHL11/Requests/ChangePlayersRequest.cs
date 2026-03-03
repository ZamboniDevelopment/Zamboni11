using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ChangePlayersRequest
{
    [TdfMember("CARD")] 
    public List<CardData> mCardDataList;
    
    [TdfMember("UID")] 
    public long mUserId;

}