using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ISRemoveWatchRequest
{
    [TdfMember("DEL")] 
    public byte mRemoveExpired;

    [TdfMember("TID")] 
    public ulong mTradeId;
    
    [TdfMember("TIDL")] 
    public List<ulong> mTradeIdList;
    
    [TdfMember("UID")] 
    public ulong mUserId;
}