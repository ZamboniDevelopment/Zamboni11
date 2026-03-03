using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Requests;

[TdfStruct]
public struct PlayGameRequest
{
    
    [TdfMember("ACID")] 
    public List<long> mGameCards;
    
    [TdfMember("CRED")] 
    public uint mCredits;
    
    [TdfMember("GTIC")] 
    public uint mGoldenTickets;
    
    [TdfMember("PGMR")] 
    public byte mMatchResult;
    
    [TdfMember("PRES")] 
    public uint mPrestige;
    
    [TdfMember("STAT")] 
    public uint mState;
    
    [TdfMember("TID")] 
    public uint mTournamentId;
    
    [TdfMember("TTYP")] 
    public byte mIsOnlineTournament;
    
    [TdfMember("UID")] 
    public ulong mUserId;
    
}