using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct CardData
{
    [TdfMember("ATTR")] 
    public List<byte> mAttributes;

    [TdfMember("CDST")] 
    public CardState mCardStateId;
    
    [TdfMember("CID")] 
    public long mCardId;
    
    [TdfMember("DBID")] 
    public uint mCardDbId; //The players databaseId, (Refer nhlng.db file and nhlviewng/tdbview program)
    
    [TdfMember("FORM")] 
    public byte mFormationId;

    [TdfMember("FREE")] 
    public byte mFREE;
    
    [TdfMember("FTNS")] 
    public byte mCareerRemaining;

    [TdfMember("INJG")] 
    public byte mInjuryGames;

    [TdfMember("INJT")] 
    public byte mInjuryType;
    
    [TdfMember("MORL")] 
    public byte mMaxTrainingCardsCanApply;
    
    [TdfMember("OWNR")] 
    public byte mNumberOfOwners;

    [TdfMember("POSI")] 
    public byte mPreferredPositionId;
    
    [TdfMember("PRIC")] 
    public byte mDiscardPrice;
    
    [TdfMember("RARE")] 
    public byte mRareFlag;

    [TdfMember("RTNG")] 
    public byte mRating;
    
    [TdfMember("SCAP")] 
    public short mSalaryCap;
    
    [TdfMember("STAT")] 
    public List<int> mListStats;
    
    [TdfMember("SUB")] 
    public CardSubType mCardSubTypeId;
    
    [TdfMember("TIME")] 
    public uint mDateIssued;
    
    [TdfMember("TMID")] 
    public uint mTeamId;
    
    [TdfMember("TRNG")] 
    public List<int> mListTrainingCards;
    
    [TdfMember("USRE")] 
    public byte mUsesRemaining;

}