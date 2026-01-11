using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ViewCardsRequest
{
    [TdfMember("CARD")] 
    public List<long> mCardIdList;

    [TdfMember("UID")] 
    public ulong mUserId;
}