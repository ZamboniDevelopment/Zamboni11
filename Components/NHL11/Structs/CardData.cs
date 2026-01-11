using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct CardData
{
    [TdfMember("ATTR")] 
    public List<byte> mAttributes;

    [TdfMember("CDST")] 
    public byte mCardStateId;
    
    [TdfMember("CID")] 
    public ulong mCardId;
    
    [TdfMember("DBID")] 
    public uint mDatabaseId; //The players databaseId, (Refer nhlng.db file and nhlviewng program)
    
    [TdfMember("FORM")] 
    public byte mFormationId;

    [TdfMember("FREE")] 
    public byte mFREE;
    
    [TdfMember("FTNS")] 
    public byte mFitness;

    [TdfMember("INJG")] 
    public byte mInjuryGames;

    [TdfMember("INJT")] 
    public byte mInjuryType;
    
    [TdfMember("MORL")] 
    public byte mMoral;
    
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
    public List<byte> mListStats;
    
    [TdfMember("SUB")] 
    public short mCardSubTypeId;
    
    [TdfMember("TIME")] 
    public uint mDateIssued;
    
    [TdfMember("TMID")] 
    public uint mTeamId;
    
    [TdfMember("TRNG")] 
    public List<byte> mListTrainingCards;
    
    [TdfMember("USRE")] 
    public byte mUsesRemaining;

}