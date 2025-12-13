using System.Collections.Generic;
using Blaze3SDK.Blaze.Example;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct SeasonConfiguration
{
    [TdfMember("DIVL")] 
    public List<DIVL> mDIVL;

    [TdfMember("LGID")] 
    public uint mLeagueID;
    
    [TdfMember("LNAM")] 
    public string mLeagueName;
    
    [TdfMember("MTYP")] 
    public MemberType mMemberType;
    
    [TdfMember("SID")] 
    public uint mSeasonID;
    
    [TdfMember("SPRT")] 
    public StatPeriod mStatPeriodEnum;
    
    [TdfMember("TID")] 
    public uint mTeamID;
}