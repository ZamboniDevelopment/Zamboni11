using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct CreatePackResponse
{
    [TdfMember("CDAT")] 
    public List<CardData> mCardDataList;

    [TdfMember("DUPL")] 
    public List<CardIdPair> mDuplicateCardIdPairList;
    
    //TODO THESE THREE MIGHT BE NAMED WRONG
    //TODO \/
    
    [TdfMember("NUM")] 
    public int mNumCards;
    
    [TdfMember("PCNT")] 
    public long mNumPackPurchased;
    
    [TdfMember("PKTY")] 
    public int mRandPackType;
    
    //TODO /\
    //TODO THESE THREE MIGHT BE NAMED WRONG
    
    [TdfMember("VER")] 
    public VersionInfo mVersionInfo;
    
}