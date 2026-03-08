using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct ISTradeInfo
{
    
    [TdfMember("BUID")] 
    public long mBlazeUserId;
    
    [TdfMember("CDAT")] 
    public CardData mCardData;

    [TdfMember("CID")] 
    public long mCardId;
    
    [TdfMember("CRED")] 
    public uint mCredits;

    [TdfMember("DBID")] 
    public uint mCardDbId;
    
    [TdfMember("EST")] 
    public uint mSellerEstDate;

    [TdfMember("EXTM")] 
    public int mExpireTime;
    
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
    public uint mBuyOutPrice;
    
    [TdfMember("SELN")] 
    public string mSellerName;
    
    [TdfMember("STAT")] 
    public TradeState mTradeState;
    
    [TdfMember("TID")] 
    public long mTradeId;
    
    [TdfMember("UID")] 
    public long mUserId;
    
    [TdfMember("YBID")] 
    public YourBid mYourBidState;

}