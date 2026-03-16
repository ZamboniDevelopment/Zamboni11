using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct StorePackQuantitiesResponse
{
    [TdfMember("PQTL")] 
    public List<int> mPackQuantitiesList;
    
}