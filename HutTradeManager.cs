using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Npgsql;
using Zamboni11.Components.NHL11;
using Zamboni11.Components.NHL11.Requests;
using Zamboni11.Components.NHL11.Responses;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutTradeManager
{
    public static async Task<long> InsertTrade(ISStartRequest request, long userId, string sellerName)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
            INSERT INTO hut_trade_info (
                user_id, card_id, starting_price, seller_name,
                buy_out_price, trade_state, duration_seconds, created_at_seconds
            ) VALUES (
                @user_id, @card_id, @starting_price, @seller_name,
                @buy_out_price, @trade_state, @duration_seconds, @created_at_seconds
            ) RETURNING trade_id;";

        await using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("card_id", request.mCardId);
        cmd.Parameters.AddWithValue("starting_price", request.mReserve);
        cmd.Parameters.AddWithValue("seller_name", sellerName);

        cmd.Parameters.AddWithValue("buy_out_price", request.mCredits);
        cmd.Parameters.AddWithValue("trade_state", (int)TradeState.CARDHOUSE_TRADESTATE_ACTIVE);
        cmd.Parameters.AddWithValue("duration_seconds", request.mPeriod);
        // cmd.Parameters.AddWithValue("duration_seconds", 20);
        cmd.Parameters.AddWithValue("created_at_seconds", (long)Util.TimeNow());

        var tradeId = await cmd.ExecuteScalarAsync();

        return (long)tradeId;
    }

    public static async Task<long> InsertOffer(ISOfferTradeRequest request, long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        await CleanExpired();

        const string sql = @"
            INSERT INTO hut_offer_info (
                trade_id, user_id, offer_state, credits,
                card_ids, created_at_seconds
            ) VALUES (
                @trade_id, @user_id, @offer_state, @credits,
                @card_ids, @created_at_seconds
            ) RETURNING offer_id;";

        await using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("trade_id", request.mTradeId);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("offer_state", (int)OfferState.CARDHOUSE_OFFERSTATE_WINNINGBID);
        cmd.Parameters.AddWithValue("credits", request.mCredits);
        var cards = (request.mCardList != null && request.mCardList.Count > 0)
            ? request.mCardList.ToArray()
            : Array.Empty<long>();

        cmd.Parameters.AddWithValue("card_ids", cards);
        cmd.Parameters.AddWithValue("created_at_seconds", (long)Util.TimeNow());

        var offerId = await cmd.ExecuteScalarAsync();

        await UpdateTradeAfterOffer(request.mTradeId, (long)offerId, request.mCredits);

        return (long)offerId;
    }

    private static async Task<bool> UpdateTradeAfterOffer(long tradeId, long offerId, int bidCredits)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string updateSql = @"
            UPDATE hut_trade_info 
            SET 
                highest_bid = @bid_credits,
                trade_state = CASE 
                    WHEN buy_out_price > 0 AND @bid_credits >= buy_out_price THEN 4 
                    ELSE 1                                                         
                END
            WHERE trade_id = @trade_id 
              AND trade_state = 1 
              AND @bid_credits > highest_bid
              AND @bid_credits >= starting_price
            RETURNING trade_state;";

        await using var cmd = new NpgsqlCommand(updateSql, conn);
        cmd.Parameters.AddWithValue("bid_credits", bidCredits);
        cmd.Parameters.AddWithValue("trade_id", tradeId);

        var result = await cmd.ExecuteScalarAsync();

        if (result != null)
        {
            TradeState returningTradeState = (TradeState)(int)result;

            if (returningTradeState == TradeState.CARDHOUSE_TRADESTATE_CLOSED)
            {
                // Buyout
                await SetOfferState(offerId, OfferState.CARDHOUSE_OFFERSTATE_TRADECLOSED);
                await ExecuteTrade(tradeId, offerId);
            }
            else
            {
                // Normal high bid. 
                await SetOfferState(offerId, OfferState.CARDHOUSE_OFFERSTATE_WINNINGBID);
            }
            return true;
        }

        return false;
    }

    private static async Task ExecuteTrade(long tradeId, long offerId)
    {
        const string offerSql = @"
        SELECT * 
        FROM hut_trade_info 
        WHERE trade_id = @trade_id;";
    }

    private static async Task SetOfferState(long offerId, OfferState offerState)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();
        const string sql = @"
            UPDATE hut_offer_info 
            SET offer_state = @offer_state 
            WHERE offer_id = @offer_id;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("offer_id", offerId);
        cmd.Parameters.AddWithValue("offer_state", (int)offerState);
        await cmd.ExecuteScalarAsync();
    }

    public static async Task<ISSearchResponse> SearchTradesAsync(ISSearchRequest request, long userId)
    {
        List<ISTradeInfo> results = new List<ISTradeInfo>();

        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        await CleanExpired();

        var sql = new StringBuilder(@"
        SELECT *, 
               GREATEST(0, (created_at_seconds + duration_seconds) - EXTRACT(EPOCH FROM NOW()))::INT AS expire_time
        FROM hut_trade_info
        WHERE 1=1");

        if (request.mNonActive == 0)
        {
            sql.Append(" AND trade_state = 1");
        }
        else
        {
            sql.Append(" AND trade_state >= 1");
        }

        if (request.mMyTrades == 2)
        {
            sql.Append(" AND user_id = @userId");
        }

        sql.Append(" ORDER BY created_at_seconds DESC;");

        await using var cmd = new NpgsqlCommand(sql.ToString(), conn);

        if (request.mMyTrades == 2)
        {
            cmd.Parameters.AddWithValue("userId", userId);
        }

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(await ReadTrade(reader, userId));
        }

        return new ISSearchResponse
        {
            mSearchResults = results,
            mTotalCount = results.Count
        };
    }

    private static async Task<int> CleanExpired()
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string updateSql = @"
        UPDATE hut_trade_info 
        SET trade_state = CASE 
            WHEN highest_bid >= starting_price AND highest_bid > 0 THEN 4 
            ELSE 3                                                      
        END
        WHERE trade_state = 1 
          AND (created_at_seconds + duration_seconds) < EXTRACT(EPOCH FROM NOW());";

        await using var updateCmd = new NpgsqlCommand(updateSql, conn);
        return await updateCmd.ExecuteNonQueryAsync();

    }

    public static async Task<ISViewTradeResponse> ViewTradeAsync(ISViewTradeRequest request, long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        await CleanExpired();

        var sql = @"
        SELECT *, 
               GREATEST(0, (created_at_seconds + duration_seconds) - EXTRACT(EPOCH FROM NOW()))::INT AS expire_time
        FROM hut_trade_info
        WHERE trade_id = @tid 
        LIMIT 1;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("tid", request.mTradeId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var result = await ReadTrade(reader, userId);
            int credits;
            if (result.mBuyOutPrice == 0)
            {
                if (result.mHighestBid == 0)
                {
                    credits = result.mStartingPrice;
                }
                else
                {
                    credits = result.mHighestBid;
                }
            }
            else
            {
                credits = result.mBuyOutPrice;
            }

            return new ISViewTradeResponse
            {
                mCredits = credits,
                mISTradeInfo = result
            };
        }

        return new ISViewTradeResponse();
    }

    private static async Task<ISTradeInfo> ReadTrade(NpgsqlDataReader reader, long readerUserId)
    {
        YourBid yourBid = await DetermineMyBidState(reader.GetInt64(reader.GetOrdinal("trade_id")), readerUserId);
        CardData cardData = await CardHouseComponent.GetCard(reader.GetInt64(reader.GetOrdinal("card_id")));
        return new ISTradeInfo
        {
            mBlazeUserId = reader.GetInt64(reader.GetOrdinal("user_id")),
            // mCardData = HutManager.GetCard(reader.GetInt64(reader.GetOrdinal("card_id"))),
            mCardData = cardData,

            mTradeId = reader.GetInt64(reader.GetOrdinal("trade_id")),
            mUserId = reader.GetInt64(reader.GetOrdinal("user_id")),
            mYourBidState = yourBid,
            mCardId = reader.GetInt64(reader.GetOrdinal("card_id")),
            mStartingPrice = reader.GetInt32(reader.GetOrdinal("starting_price")),
            mCardDbId = cardData.mCardDbId,
            mSellerEstDate = 0,
            mHighestBid = reader.GetInt32(reader.GetOrdinal("highest_bid")),
            // mInbox = 0,
            // mIsWatched = 0,
            // mOfferPendingCount = 0,
            mBuyOutPrice = reader.GetInt32(reader.GetOrdinal("buy_out_price")),
            mSellerName = reader.GetString(reader.GetOrdinal("seller_name")),
            mTradeState = (TradeState)reader.GetInt32(reader.GetOrdinal("trade_state")),
            mSecondsLeft = reader.GetInt32(reader.GetOrdinal("expire_time")),
            // mGlow = 0,
        };
    }

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static async Task<YourBid> DetermineMyBidState(long tradeId, long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sellerCheckSql = "SELECT user_id FROM hut_trade_info WHERE trade_id = @trade_id;";
        await using (var sellerCmd = new NpgsqlCommand(sellerCheckSql, conn))
        {
            sellerCmd.Parameters.AddWithValue("trade_id", tradeId);
            var sellerId = await sellerCmd.ExecuteScalarAsync();

            if (sellerId != null && (long)sellerId == userId)
            {
                return YourBid.CARDHOUSE_YOURBID_NONE;
            }
        }

        const string offerSql = @"
        SELECT offer_state 
        FROM hut_offer_info 
        WHERE trade_id = @trade_id AND user_id = @user_id;";

        List<int> states = new List<int>();
        await using (var cmd = new NpgsqlCommand(offerSql, conn))
        {
            cmd.Parameters.AddWithValue("trade_id", tradeId);
            cmd.Parameters.AddWithValue("user_id", userId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                states.Add(reader.GetInt32(0));
            }
        }


        if (states.Count == 0)
        {
            return YourBid.CARDHOUSE_YOURBID_NONE;
        }

        if (states.Contains(7))
        {
            return YourBid.CARDHOUSE_YOURBID_HIGHEST; // 2
        }

        if (states.TrueForAll(s => s == 5))
        {
            return YourBid.CARDHOUSE_YOURBID_PREVIOUS; // 1
        }

        return YourBid.CARDHOUSE_YOURBID_NONE;
    }


    // public static ISTradeInfo CreateAuction(ISStartRequest request, BlazeServerConnection connection)
    // {
    //     long id = TradeIdCounter++;
    //     ServerPlayer serverPlayer = ServerManager.GetServerPlayer(connection);
    //     CardData cardData = HutManager.GetCard(request.mCardId);
    //     ISTradeInfo info = new ISTradeInfo
    //     {
    //         mBlazeUserId = serverPlayer.UserIdentification.mAccountId,
    //         mCardData = cardData,
    //         mCardId = request.mCardId,
    //         mCredits = request.mCredits,
    //         mCardDbId = cardData.mCardDbId,
    //         mSellerEstDate = Util.TimeNow(),
    //         mSecondsLeft = 20, //TODO
    //         mGlow = 0,
    //         mHighestBid = 0,
    //         mInbox = 1,
    //         mIsWatched = 1,
    //         mOfferPendingCount = 0,
    //         mReserve = request.mReserve,
    //         mSellerName = serverPlayer.UserIdentification.mName,
    //         mTradeState = TradeState.CARDHOUSE_TRADESTATE_ACTIVE,
    //         mTradeId = id,
    //         mUserId = serverPlayer.UserIdentification.mAccountId,
    //         mYourBidState = YourBid.CARDHOUSE_YOURBID_HIGHEST
    //     };
    //     Auctions.TryAdd(id, info);
    //     return info;
    // }

    // public static ISOfferInfo CreateOffer(ISOfferTradeRequest request, BlazeServerConnection connection)
    // {
    //     long id = OfferIdCounter++;
    //     ServerPlayer serverPlayer = ServerManager.GetServerPlayer(connection);
    //     List<CardData> retList = new List<CardData>();
    //     if (request.mCardList != null && request.mCardList.Count >= 1)
    //     {
    //         foreach (var VARIABLE in request.mCardList)
    //         {
    //             retList.Add(HutManager.GetCard(VARIABLE));
    //         }
    //     }
    //
    //     ISOfferInfo info = new ISOfferInfo
    //     {
    //         mCardList = request.mCardList,
    //         mCardDataList = retList,
    //         mCredits = request.mCredits,
    //         mOfferId = id,
    //         mOfferState = OfferState.CARDHOUSE_OFFERSTATE_WINNINGBID,
    //         mTradeId = request.mTradeId,
    //         mUserId = 0
    //     };
    //     Offers.TryAdd(id, info);
    //     TradeIdOfferAssocication.TryAdd(request.mTradeId, new ConcurrentBag<long>());
    //     TradeIdOfferAssocication[request.mTradeId].Add(id);
    //     return info;
    // }
}