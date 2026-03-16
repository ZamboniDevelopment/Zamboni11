using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct StorePackQuantitiesRequest
{
    [TdfMember("PTIL")] 
    public List<short> mPackTypeIdList;

}