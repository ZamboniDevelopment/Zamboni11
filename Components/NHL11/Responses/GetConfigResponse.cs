using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct GetConfigResponse
{
    
    [TdfMember("GCFL")] 
    public List<uint> mConfigList;
    
}