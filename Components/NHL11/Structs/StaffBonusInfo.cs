using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct StaffBonusInfo
{
    [TdfMember("ARM")] 
    public byte mPhysioArmBonus;

    [TdfMember("BACK")] 
    public byte mPhysioBackBonus;

    [TdfMember("CON")] 
    public byte mContractBonus;
    
    [TdfMember("FIT")] 
    public byte mFitnessBonus;

    [TdfMember("FOOT")] 
    public byte mPhysioFootBonus;

    [TdfMember("GKD")] 
    public byte mGKDivingBonus;
    
    [TdfMember("GKH")] 
    public byte mGKHandlingBonus;
    
    [TdfMember("GKK")] 
    public byte mGKKickingBonus;

    [TdfMember("GKO")] 
    public byte mGKOneOnOneBonus;

    [TdfMember("GKP")] 
    public byte mGKPositioningBonus;
    
    [TdfMember("GKR")] 
    public byte mGKReflexesBonus;

    [TdfMember("HEAD")] 
    public byte mPhysioHeadBonus;

    [TdfMember("HIP")] 
    public byte mPhysioHipBonus;
    
    [TdfMember("LEG")] 
    public byte mPhysioLegBonus;

    [TdfMember("PDEF")] 
    public byte mDefendingBonus;

    [TdfMember("PDR")] 
    public byte mDribblingBonus;
    
    [TdfMember("PHE")] 
    public byte mHeadingBonus;
    
    [TdfMember("PPAC")] 
    public byte mPaceBonus;

    [TdfMember("PPAS")] 
    public byte mPassingBonus;

    [TdfMember("PSH")] 
    public byte mShootingBonus;
    
    [TdfMember("SHOU")] 
    public byte mPhysioShoulderBonus;

    [TdfMember("TALK")] 
    public byte mManagerTalkBonus;
}