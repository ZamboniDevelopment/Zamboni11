using Tdf;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct ActivateCardResponse
{
    
    [TdfMember("CID")] 
    public long mCardId;
    
}