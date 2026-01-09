using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct DeckInfoResponse
{
    [TdfMember("DUPE")] 
    public List<CardIdPair> mDuplicateEscrowCardIdPairList;

    [TdfMember("DUPU")] 
    public List<CardIdPair> mDuplicateUnassignedCardIdPairList;
    
    [TdfMember("ECDL")] 
    public List<CardData> mEscrowCardDataList;
    
    [TdfMember("ECNT")] 
    public byte mECNT;
    
    [TdfMember("GEN")] 
    public GeneralInfo mGeneralInfo;
    
    [TdfMember("RATE")] 
    public uint mRATE;
    
    [TdfMember("UCDL")] 
    public List<CardData> mUnassignedCardDataList;
    
    [TdfMember("UID")] 
    public long mUID;
    
    [TdfMember("VER")] 
    public VersionInfo mVersionInfo;
}