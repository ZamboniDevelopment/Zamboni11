using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct GamerInfo
{
    [TdfMember("CTAC")] 
    public string mCustomTactics;

    [TdfMember("FORM")] 
    public uint mTeamFormation;

    [TdfMember("KTAK")] 
    public string mKickTakers;

    [TdfMember("LINE")] 
    public string mLineup;

    [TdfMember("LOGU")] 
    public string mLogoUrl;

    [TdfMember("NAME")] 
    public string mTeamName;

    [TdfMember("PLYQ")] 
    public uint mPlayoffsQualified;

    [TdfMember("PLYW")] 
    public uint mPlayoffWon;

    [TdfMember("QTAC")] 
    public string mQuickTactics;

    [TdfMember("SPBT")] 
    public uint mSpecialPacksBought;

    [TdfMember("TMAB")] 
    public string mTeamAbbreviation;

    [TdfMember("TOUR")] 
    public string mTournaments;

    [TdfMember("TPPL")] 
    public uint mTPPL;

    [TdfMember("TROP")] 
    public string mTrophies;
}