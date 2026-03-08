using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct SquadInfo
{
    [TdfMember("CHEM")] 
    public uint mChemistry;
    
    [TdfMember("CHNG")] 
    public uint mCHNG;
    
    [TdfMember("FORM")] 
    public uint mFormationId;
    
    [TdfMember("LINE")] 
    public List<uint> mLines;
    
    [TdfMember("MNGR")] 
    public CardData mManager;
    
    [TdfMember("NAME")] 
    public string mSquadName;

    [TdfMember("PLRS")] 
    public List<CardData> mPlayers;
    
    [TdfMember("RTNG")] 
    public uint mStarRating;
    
    [TdfMember("SQID")] 
    public uint mSquadId;
}