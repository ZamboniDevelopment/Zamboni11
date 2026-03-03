using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct AssignCardsRequest
{
    [TdfMember("LIST")] 
    public List<AssignCardCard> mList;
    
    [TdfMember("UID")] 
    public long mUserId;

}