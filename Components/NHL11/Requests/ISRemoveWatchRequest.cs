using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ISRemoveWatchRequest
{
    [TdfMember("DEL")] 
    public byte mRemoveExpired;

    [TdfMember("TID")] 
    public long mTradeId;
    
    [TdfMember("TIDL")] 
    public List<long> mTradeIdList;
    
    [TdfMember("UID")] 
    public long mUserId;
}