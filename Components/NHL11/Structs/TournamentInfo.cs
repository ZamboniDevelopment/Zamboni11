using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct TournamentInfo
{
    [TdfMember("AIGR")] 
    public int mAiGroup;

    [TdfMember("BTID")] 
    public int mBlazeTournamentId;
    
    [TdfMember("DIFF")] 
    public int mDifficulty;
    
    [TdfMember("EL1T")] 
    public ElgType mElg1Type;
    
    [TdfMember("EL1D")] 
    public int mElg1Data;

    [TdfMember("EL2T")] 
    public ElgType mElg2Type;
    
    [TdfMember("EL2D")] 
    public int mElg12Data;
    
    [TdfMember("END")] 
    public uint mEndTime;

    [TdfMember("MLEN")] 
    public int mMatchLenght;
    
    [TdfMember("PRIZ")] 
    public int mPrize;
    
    [TdfMember("RWD1")] 
    public int mReward1;

    [TdfMember("RWD2")] 
    public int mReward2;
    
    [TdfMember("RWD3")] 
    public int mReward3;
    
    [TdfMember("RWD4")] 
    public int mReward4;

    [TdfMember("SLRY")] 
    public int mSalaryCap;
    
    [TdfMember("STRT")] 
    public uint mStartTime;
    
    [TdfMember("TCDI")] 
    public int mTrophyCardDbId;
    
    [TdfMember("TID")] 
    public int mTournamentId;
    
    [TdfMember("TYPE")] 
    public int mType;

    [TdfMember("UNLK")] 
    public int mTrophiesRequiredToEnter;
    
}