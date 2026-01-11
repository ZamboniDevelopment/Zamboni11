using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct SquadSaveRequest
{
    
    [TdfMember("CHEM")] 
    public uint mChemistry;
    
    [TdfMember("FORM")] 
    public uint mFormation;
    
    [TdfMember("LINE")] 
    public List<uint> mLines;
    
    [TdfMember("MNGR")] 
    public ulong mManager;
    
    [TdfMember("NAME")] 
    public string mSquadName;
    
    [TdfMember("PLRS")] 
    public List<ulong> mPlayers;
    
    [TdfMember("RTNG")] 
    public uint mStarRating;
    
    [TdfMember("SQID")] 
    public uint mSquadId;
    
    [TdfMember("UID")] 
    public long mUserId;
}