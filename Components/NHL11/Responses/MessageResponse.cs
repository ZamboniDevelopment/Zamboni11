using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct MessageResponse
{
    [TdfMember("ENUM")] 
    public ENUM mEnum;

    [TdfMember("MSGS")] 
    public List<MSGS> mMessagesList;
}