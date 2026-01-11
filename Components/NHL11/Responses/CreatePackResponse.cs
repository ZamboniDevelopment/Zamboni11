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
    
    [TdfMember("NUM")] 
    public uint mNumCards;
    
    [TdfMember("PCNT")] 
    public long mPCNT;
    
    [TdfMember("PKTY")] 
    public uint mPKTY;
    
    [TdfMember("VER")] 
    public VersionInfo mVersionInfo;
    
}