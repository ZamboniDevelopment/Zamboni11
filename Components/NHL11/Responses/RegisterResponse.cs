using Tdf;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct RegisterResponse
{
    
    [TdfMember("MSGS")] 
    public uint mNumMessages;
    
}