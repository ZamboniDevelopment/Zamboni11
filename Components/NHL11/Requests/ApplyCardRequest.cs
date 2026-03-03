using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct ApplyCardRequest
{
    [TdfMember("CID")] 
    public long mCardId;
    
    [TdfMember("CIDT")] 
    public List<long> mTargetCards;
    
    [TdfMember("UID")] 
    public ulong mUserId;

}