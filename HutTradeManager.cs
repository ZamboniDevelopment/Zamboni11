using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Npgsql;
using Org.BouncyCastle.Bcpg.OpenPgp;
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
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            var tradeSql = "SELECT user_id, card_id FROM hut_trade_info WHERE trade_id = @tId";
            var offerSql = "SELECT user_id, credits FROM hut_offer_info WHERE offer_id = @oId";

            long sellerId, cardId, buyerId;
            int price;

            await using (var cmd = new NpgsqlCommand(tradeSql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("tId", tradeId);
                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) throw new Exception("Trade not found");
                sellerId = reader.GetInt64(0);
                cardId = reader.GetInt64(1);
            }

            await using (var cmd = new NpgsqlCommand(offerSql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("oId", offerId);
                await using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) throw new Exception("Offer not found");
                buyerId = reader.GetInt64(0);
                price = reader.GetInt32(1);
            }

            var cardData = (await HutManager.GetCard(cardId)).Card;
            await HutCardFactory.CreateOrUpdateCard(cardData, buyerId, DeckType.CARDHOUSE_DECK_UNASSIGNED);

            var buyerInfo = await HutManager.GetGeneralInfo(buyerId);

            await HutManager.SetGeneralInfo(new GeneralInfo
            {
                mCredits = buyerInfo.Value.mCredits-price,
                mStats = new List<byte>()
            }, buyerId);

            
            var sellerInfo = await HutManager.GetGeneralInfo(sellerId);

            await HutManager.SetGeneralInfo(new GeneralInfo
            {
                mCredits = sellerInfo.Value.mCredits+price,
                mStats = new List<byte>()
            }, sellerId);

            await HutManager.IncrementVersionInfo(buyerId, HutManager.VersionType.Unassigned);
            await HutManager.IncrementVersionInfo(sellerId, HutManager.VersionType.Escrow);

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
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
            SELECT t.*, 
                   c.*, 
                   GREATEST(0, (t.created_at_seconds + t.duration_seconds) - EXTRACT(EPOCH FROM NOW()))::INT AS expire_time
            FROM hut_trade_info t
            INNER JOIN hut_cards c ON t.card_id = c.card_id
            WHERE 1=1");

        switch (request.mCardType)
        {
            case CardSearchTypeParameter.SEARCH_PLAYERS: sql.Append(" AND c.sub_type BETWEEN 0 AND 4"); break;
            case CardSearchTypeParameter.SEARCH_HEAD_COACH: sql.Append(" AND c.sub_type = 6"); break;
            case CardSearchTypeParameter.SEARCH_TEAM_INFORMATION: sql.Append(" AND c.sub_type IN (10, 12)"); break;
            case CardSearchTypeParameter.SEARCH_TRAINING: sql.Append(" AND c.sub_type BETWEEN 51 AND 62"); break;
            case CardSearchTypeParameter.SEARCH_CONTRACTS: sql.Append(" AND c.sub_type = 201"); break;
            case CardSearchTypeParameter.SEARCH_ARENAS: sql.Append(" AND c.sub_type = 11"); break;
        }

        if (request.mCategory >= 0 || request.mFormation >= 0 || request.mLevel >= 0 || request.mNation >= 0 || request.mFieldZone >= 0) throw new NotImplementedException();

        if (request.mLeagueId >= 0)
        {
            Range range = HutCardFactory.LeagueTeamsMapping[request.mLeagueId];
            sql.Append($" AND c.team_id BETWEEN {range.Start.Value} AND {range.End.Value}");
        }

        if (request.mPosition >= 0) sql.Append(" AND c.sub_type = " + request.mPosition);
        if (request.mTeamId >= 0) sql.Append(" AND c.team_id = " + request.mTeamId);


        sql.Append(request.mNonActive == 0 ? " AND t.trade_state = 1" : " AND t.trade_state >= 1");

        if (request.mMyTrades == 2) sql.Append(" AND t.user_id = @userId");

        if (request.mMinCredits > 0) sql.Append(" AND (CASE WHEN t.highest_bid > 0 THEN t.highest_bid ELSE t.starting_price END) >= @minCredits");
        if (request.mMaxCredits > 0) sql.Append(" AND (CASE WHEN t.highest_bid > 0 THEN t.highest_bid ELSE t.starting_price END) <= @maxCredits");
        if (request.mMinBuyPrice > 0) sql.Append(" AND t.buy_out_price >= @minBuy");
        if (request.mMaxBuyPrice > 0) sql.Append(" AND t.buy_out_price <= @maxBuy AND t.buy_out_price > 0");

        await using var cmd = new NpgsqlCommand(sql.ToString(), conn);

        if (request.mMyTrades == 2) cmd.Parameters.AddWithValue("userId", userId);
        if (request.mMaxBuyPrice > 0) cmd.Parameters.AddWithValue("maxBuy", request.mMaxBuyPrice);
        if (request.mMinCredits > 0) cmd.Parameters.AddWithValue("minCredits", request.mMinCredits);
        if (request.mMaxCredits > 0) cmd.Parameters.AddWithValue("maxCredits", request.mMaxCredits);
        if (request.mMinBuyPrice > 0) cmd.Parameters.AddWithValue("minBuy", request.mMinBuyPrice);


        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(await HutHelper.ReadTrade(reader, userId));
        }

        Logger.Debug(sql.ToString);

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
            var result = await HutHelper.ReadTrade(reader, userId);
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

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static async Task<YourBid> DetermineMyBidState(long tradeId, long userId)
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
            return YourBid.CARDHOUSE_YOURBID_HIGHEST;
        }

        if (states.TrueForAll(s => s == 5))
        {
            return YourBid.CARDHOUSE_YOURBID_PREVIOUS;
        }

        return YourBid.CARDHOUSE_YOURBID_NONE;
    }
    
}