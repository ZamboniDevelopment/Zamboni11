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
    public uint mFORM;
    
    [TdfMember("LINE")] 
    public List<uint> mLines;
    
    [TdfMember("MNGR")] 
    public CardData mManager; //Coach of the team?
    
    [TdfMember("NAME")] 
    public string mName;

    [TdfMember("PLRS")] 
    public List<CardData> mPlayers;
    
    [TdfMember("RTNG")] 
    public uint mRating;
    
    [TdfMember("SQID")] 
    public uint mSquadId;
}