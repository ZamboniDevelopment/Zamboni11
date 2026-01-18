using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct ISTradeInfo
{
    
    [TdfMember("BUID")] 
    public ulong mBlazeUserId;
    
    [TdfMember("CDAT")] 
    public CardData mCardData;

    [TdfMember("CID")] 
    public ulong mCardId;
    
    [TdfMember("CRED")] 
    public uint mCredits;

    [TdfMember("DBID")] 
    public uint mCardDbId;
    
    [TdfMember("EST")] 
    public uint mSellerEstDate;

    [TdfMember("EXTM")] 
    public uint mExpireTime;
    
    [TdfMember("GLOW")] 
    public byte mGlow;

    [TdfMember("HBID")] 
    public uint mHighestBid;
    
    [TdfMember("INBX")] 
    public byte mInbox;

    [TdfMember("ISW")] 
    public byte mIsWatched;
    
    [TdfMember("OFPE")] 
    public uint mOfferPendingCount;
    
    [TdfMember("RESV")] 
    public uint mReserve;
    
    [TdfMember("SELN")] 
    public string mSellerName;
    
    [TdfMember("STAT")] 
    public uint mTradeState;
    
    [TdfMember("TID")] 
    public ulong mTradeId;
    
    [TdfMember("UID")] 
    public ulong mUserId;
    
    [TdfMember("YBID")] 
    public uint mYourBidState;


}