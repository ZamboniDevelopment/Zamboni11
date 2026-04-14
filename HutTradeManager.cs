using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blaze3SDK;
using BlazeCommon;
using Npgsql;
using Zamboni11.Components.NHL11.Requests;
using Zamboni11.Components.NHL11.Responses;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutTradeManager
{
    private static readonly ConcurrentDictionary<long, SemaphoreSlim> _tradeLocks = new();

    public static async Task<ISSearchResponse> SearchTradesAsync(ISSearchRequest request, long searcherUserId)
    {
        List<ISTradeInfo> results = new List<ISTradeInfo>();

        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        await CleanAndExecuteExpired();

        var sql = new StringBuilder(@"
            SELECT t.*, 
                   c.*, 
                   GREATEST(0, (t.created_at_seconds + t.duration_seconds) - EXTRACT(EPOCH FROM NOW()))::INT AS expire_time
            FROM hut_trade_info t
            INNER JOIN hut_cards c ON t.card_id = c.card_id
            INNER JOIN fcc_leagues l ON c.team_id = l.teamid
            WHERE 1=1");

        switch (request.mCardType)
        {
            case CardSearchTypeParameter.SEARCH_PLAYERS: sql.Append(" AND c.sub_type BETWEEN 0 AND 4"); break;
            case CardSearchTypeParameter.SEARCH_HEAD_COACH: sql.Append(" AND c.sub_type = 6"); break;
            case CardSearchTypeParameter.SEARCH_TEAM_INFORMATION: sql.Append(" AND c.sub_type IN (10, 12)"); break;
            case CardSearchTypeParameter.SEARCH_TRAINING: sql.Append(" AND c.sub_type BETWEEN 51 AND 62"); break;
            case CardSearchTypeParameter.SEARCH_CONTRACTS: sql.Append(" AND c.sub_type = 201"); break;
            case CardSearchTypeParameter.SEARCH_ARENAS: sql.Append(" AND c.sub_type = 11"); break;
            case CardSearchTypeParameter.ANY: break;
            default: throw new NotImplementedException();
        }

        if (request.mCategory >= 0 || request.mFormation >= 0 || request.mLevel >= 0 || request.mNation >= 0 || request.mFieldZone >= 0) throw new NotImplementedException();

        if (request.mLeagueId >= 0) sql.Append(" AND l.leagueid = @league_id");
        if (request.mPosition >= 0) sql.Append(" AND c.sub_type = " + request.mPosition);
        if (request.mTeamId >= 0) sql.Append(" AND c.team_id = " + request.mTeamId);

        sql.Append(request.mNonActive == 0 ? " AND t.trade_state = 1" : " AND t.trade_state >= 1");

        sql.Append(request.mMyTrades == 0 ? " AND t.user_id != @userId" : " AND t.user_id = @userId");

        if (request.mMinCredits > 0) sql.Append(" AND (CASE WHEN t.highest_bid > 0 THEN t.highest_bid ELSE t.starting_price END) >= @minCredits");
        if (request.mMaxCredits > 0) sql.Append(" AND (CASE WHEN t.highest_bid > 0 THEN t.highest_bid ELSE t.starting_price END) <= @maxCredits");
        if (request.mMinBuyPrice > 0) sql.Append(" AND t.buy_out_price >= @minBuy");
        if (request.mMaxBuyPrice > 0) sql.Append(" AND t.buy_out_price <= @maxBuy AND t.buy_out_price > 0");

        await using var cmd = new NpgsqlCommand(sql.ToString(), conn);

        cmd.Parameters.AddWithValue("userId", searcherUserId);
        if (request.mMaxBuyPrice > 0) cmd.Parameters.AddWithValue("maxBuy", request.mMaxBuyPrice);
        if (request.mMinCredits > 0) cmd.Parameters.AddWithValue("minCredits", request.mMinCredits);
        if (request.mMaxCredits > 0) cmd.Parameters.AddWithValue("maxCredits", request.mMaxCredits);
        if (request.mMinBuyPrice > 0) cmd.Parameters.AddWithValue("minBuy", request.mMinBuyPrice);
        if (request.mLeagueId >= 0) cmd.Parameters.AddWithValue("league_id", request.mLeagueId);

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(await HutHelper.ReadTrade(reader, searcherUserId));
        }

        return new ISSearchResponse
        {
            mSearchResults = results,
            mTotalCount = results.Count
        };
    }

    public static async Task<(ISViewTradeResponse Response, BlazeRpcException? Exception)> ViewTradeAsync(ISViewTradeRequest request, long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        await CleanAndExecuteExpired();

        var sql = @"
        SELECT *, 
               GREATEST(0, (created_at_seconds + duration_seconds) - EXTRACT(EPOCH FROM NOW()))::INT AS expire_time
        FROM hut_trade_info
        WHERE trade_id = @tid;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("tid", request.mTradeId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var result = await HutHelper.ReadTrade(reader, userId);

            var general = await HutManager.GetGeneralInfo(userId);
            return (new ISViewTradeResponse
            {
                mCredits = general.Value.mCredits,
                mISTradeInfo = result
            }, null);
        }

        return (new ISViewTradeResponse(), new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_NO_SUCH_TRADE));
    }

    public static async Task CleanAndExecuteExpired()
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
                WITH updated_trades AS (
                    UPDATE hut_trade_info 
                    SET trade_state = CASE 
                        WHEN highest_bid >= starting_price AND highest_bid > 0 THEN 4 
                        ELSE 3                                                      
                    END
                    WHERE trade_state = 1 
                      AND (created_at_seconds + duration_seconds) < EXTRACT(EPOCH FROM NOW())
                    RETURNING trade_id, trade_state, highest_bid, card_id, user_id
                )
                SELECT 
                    ut.trade_id,
                    ut.trade_state,
                    (SELECT offer_id FROM hut_offer_info 
                     WHERE trade_id = ut.trade_id 
                     AND credits = ut.highest_bid 
                     AND offer_state = 7
                     LIMIT 1) as winning_offer_id,
                    ut.card_id,
                    ut.user_id
                FROM updated_trades ut;";

        var processed = new List<(long TradeId, TradeState State, long? OfferId, long CardId, long SellerId)>();

        await using (var cmd = new NpgsqlCommand(sql, conn))
        {
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                processed.Add((
                    reader.GetInt64(0),
                    (TradeState)reader.GetInt32(1),
                    reader.IsDBNull(2) ? null : reader.GetInt64(2),
                    reader.GetInt64(3),
                    reader.GetInt64(4)
                ));
            }
        }

        foreach (var trade in processed)
        {
            var semaphore = _tradeLocks.GetOrAdd(trade.TradeId, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync();

            try
            {
                if (trade.State == TradeState.CARDHOUSE_TRADESTATE_CLOSED && trade.OfferId.HasValue)
                {
                    await RefundLosers(trade.TradeId, OfferState.CARDHOUSE_OFFERSTATE_REJECTED, true, trade.OfferId.Value);
                    await ExecuteTrade(trade.TradeId, trade.OfferId.Value, false);
                }
                else if (trade.State == TradeState.CARDHOUSE_TRADESTATE_EXPIRED)
                {
                    var card = await HutManager.GetCard(trade.CardId);
                    card.Card.mCardStateId = CardState.CARDHOUSE_CARDSTATE_FREE;
                    await HutCardFactory.CreateOrUpdateCard(card.Card, trade.SellerId);
                    await HutManager.IncrementVersionInfo(trade.SellerId, HutManager.VersionType.Escrow);
                    await RefundLosers(trade.TradeId, OfferState.CARDHOUSE_OFFERSTATE_REJECTED, true);
                    ScheduleDelayedSemaphoreRemoval(trade.TradeId);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }


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
        // cmd.Parameters.AddWithValue("duration_seconds", request.mPeriod);
        cmd.Parameters.AddWithValue("duration_seconds", 20);
        cmd.Parameters.AddWithValue("created_at_seconds", (long)Util.TimeNow());

        var tradeId = await cmd.ExecuteScalarAsync();

        var card = await HutManager.GetCard(request.mCardId, userId);
        card.Card.mCardStateId = CardState.CARDHOUSE_CARDSTATE_ONLINESWAP;
        await HutCardFactory.CreateOrUpdateCard(card.Card, userId);
        await HutManager.IncrementVersionInfo(userId, HutManager.VersionType.Escrow);

        return (long)tradeId;
    }

    public static async Task<(long OfferId, BlazeRpcException? Exception)> InsertOffer(ISOfferTradeRequest request, long userId)
    {
        var semaphore = _tradeLocks.GetOrAdd(request.mTradeId, _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync();

        try
        {
            bool containsCards = request.mCardList != null && request.mCardList.Count > 0;

            await using var conn = new NpgsqlConnection(Database.ConnectionString);
            await conn.OpenAsync();

            const string sql = @"
            INSERT INTO hut_offer_info (
                trade_id, user_id, offer_state, credits,
                card_ids, created_at_seconds
            ) 
            SELECT @trade_id, @user_id, @offer_state, @credits, @card_ids, @created_at_seconds
            WHERE EXISTS (
                SELECT 1 FROM hut_trade_info WHERE trade_id = @trade_id AND trade_state = 1
            )
            AND (
                @has_cards = TRUE 
                OR NOT EXISTS (
                    SELECT 1 FROM hut_offer_info 
                    WHERE trade_id = @trade_id AND credits >= @credits
                )
            )
            RETURNING offer_id;";

            if (!await HutHelper.Withdraw(userId, request.mCredits))
            {
                return (0, new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_NOT_ENOUGH_CREDITS));
            }

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("trade_id", request.mTradeId);
            cmd.Parameters.AddWithValue("user_id", userId);
            cmd.Parameters.AddWithValue("offer_state", containsCards ? (int)OfferState.CARDHOUSE_OFFERSTATE_ACTIVE : (int)OfferState.CARDHOUSE_OFFERSTATE_WINNINGBID);
            cmd.Parameters.AddWithValue("credits", request.mCredits);
            cmd.Parameters.AddWithValue("has_cards", containsCards);
            var cards = containsCards ? request.mCardList.ToArray() : Array.Empty<long>();
            cmd.Parameters.AddWithValue("card_ids", cards);
            cmd.Parameters.AddWithValue("created_at_seconds", (long)Util.TimeNow());

            var result = await cmd.ExecuteScalarAsync();

            if (result == null)
            {
                await HutHelper.Deposit(userId, request.mCredits);
                return (0, new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_TRADE_MISMATCH));
            }

            if (containsCards)
            {
                foreach (var offeredCardId in request.mCardList)
                {
                    var card = await HutManager.GetCard(offeredCardId, userId);
                    card.Card.mCardStateId = CardState.CARDHOUSE_CARDSTATE_ONLINESWAP;
                    await HutCardFactory.CreateOrUpdateCard(card.Card, userId);
                }

                await HutManager.IncrementVersionInfo(userId, HutManager.VersionType.Escrow);
            }


            long offerId = Convert.ToInt64(result);
            await InsertWatching(request.mTradeId, userId);
            await UpdateTradeAfterOffer(request.mTradeId, offerId, userId, request.mCredits, containsCards);

            return (offerId, null);
        }
        finally
        {
            semaphore.Release();
        }
    }

    public static async Task UpdateTradeAfterOffer(long tradeId, long offerId, long offererId, int bidCredits, bool containsCards)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        if (containsCards) return;

        const string updateSql = @"
            UPDATE hut_trade_info 
            SET highest_bid = @bid_credits,
                trade_state = CASE WHEN buy_out_price > 0 AND @bid_credits >= buy_out_price THEN 4 ELSE 1 END,
                duration_seconds = CASE 
                    WHEN (buy_out_price <= 0 OR @bid_credits < buy_out_price) 
                         AND (created_at_seconds + duration_seconds) - EXTRACT(EPOCH FROM NOW()) < 30 
                    THEN duration_seconds + 30 ELSE duration_seconds 
                END
            WHERE trade_id = @trade_id AND trade_state = 1 AND @bid_credits > highest_bid
            RETURNING trade_state;";

        await using var cmd = new NpgsqlCommand(updateSql, conn);
        cmd.Parameters.AddWithValue("bid_credits", bidCredits);
        cmd.Parameters.AddWithValue("trade_id", tradeId);

        var result = await cmd.ExecuteScalarAsync();

        if (result != null)
        {
            var returningTradeState = (TradeState)(int)result;
            if (returningTradeState == TradeState.CARDHOUSE_TRADESTATE_CLOSED)
            {
                //Buyout
                const string acceptSql = "UPDATE hut_offer_info SET offer_state = 2 WHERE offer_id = @offer_id;";
                await using var acceptCmd = new NpgsqlCommand(acceptSql, conn);
                acceptCmd.Parameters.AddWithValue("offer_id", offerId);
                await acceptCmd.ExecuteNonQueryAsync();
                await RefundLosers(tradeId, OfferState.CARDHOUSE_OFFERSTATE_OUTBID, true, offerId);
                await ExecuteTrade(tradeId, offerId, true);
            }
            else if (returningTradeState == TradeState.CARDHOUSE_TRADESTATE_ACTIVE)
            {
                //High bid
                await RefundLosers(tradeId, OfferState.CARDHOUSE_OFFERSTATE_OUTBID, false, offerId);
                await InsertWatching(tradeId, offererId);
            }
        }
    }

    public static async Task RefundLosers(long tradeId, OfferState updatedOfferState, bool tradeEnded, long excludedOfferId = 0)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        int[] targetStates = tradeEnded ? [1, 7] : [7];

        const string outbidSql = @"
                UPDATE hut_offer_info 
                SET offer_state = @offer_state 
                WHERE trade_id = @trade_id 
                  AND offer_id != @offer_id 
                  AND offer_state = ANY(@target_states)
                RETURNING user_id, credits, card_ids;";

        await using var outbidCmd = new NpgsqlCommand(outbidSql, conn);
        outbidCmd.Parameters.AddWithValue("trade_id", tradeId);
        outbidCmd.Parameters.AddWithValue("offer_id", excludedOfferId);
        outbidCmd.Parameters.AddWithValue("offer_state", (int)updatedOfferState);
        outbidCmd.Parameters.AddWithValue("target_states", targetStates);

        await using var reader = await outbidCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            long userId = reader.GetInt64(0);
            int offerAmount = reader.GetInt32(1);
            List<long> returningCards = reader.GetFieldValue<long[]>(2).ToList();

            await HutHelper.Deposit(userId, offerAmount);
            if (returningCards.Count >= 1)
            {
                foreach (var returningCardId in returningCards)
                {
                    var card = await HutManager.GetCard(returningCardId, userId);
                    card.Card.mCardStateId = CardState.CARDHOUSE_CARDSTATE_FREE;
                    await HutCardFactory.CreateOrUpdateCard(card.Card, userId);
                }

                await HutManager.IncrementVersionInfo(userId, HutManager.VersionType.Escrow);
            }
        }
    }

    public static async Task<BlazeRpcException?> AdminAcceptOffer(long tradeId, long offerId)
    {
        var semaphore = _tradeLocks.GetOrAdd(tradeId, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();

        try
        {
            await using var conn = new NpgsqlConnection(Database.ConnectionString);
            await conn.OpenAsync();
            await using var transaction = await conn.BeginTransactionAsync();

            const string updateTradeSql = "UPDATE hut_trade_info SET trade_state = 4 WHERE trade_id = @trade_id AND trade_state = 1;";
            await using var tradeCmd = new NpgsqlCommand(updateTradeSql, conn, transaction);
            tradeCmd.Parameters.AddWithValue("trade_id", tradeId);

            int tradeRows = await tradeCmd.ExecuteNonQueryAsync();
            if (tradeRows == 0) return new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_NO_SUCH_TRADE);

            const string updateOfferSql = "UPDATE hut_offer_info SET offer_state = 2 WHERE offer_id = @offer_id AND offer_state = 1;";
            await using var offerCmd = new NpgsqlCommand(updateOfferSql, conn, transaction);
            offerCmd.Parameters.AddWithValue("offer_id", offerId);

            int offerRows = await offerCmd.ExecuteNonQueryAsync();
            if (offerRows == 0)
            {
                await transaction.RollbackAsync();
                return new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_NO_SUCH_TRADE);
            }

            await transaction.CommitAsync();

            await RefundLosers(tradeId, OfferState.CARDHOUSE_OFFERSTATE_OUTBID, true, offerId);
            await ExecuteTrade(tradeId, offerId, false);
        }
        finally
        {
            semaphore.Release();
        }

        return null;
    }

    public static async Task<BlazeRpcException?> AdminRejectOffer(long offerId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
            UPDATE hut_offer_info 
            SET offer_state = 3 
            WHERE offer_id = @offer_id 
            AND offer_state = 1
            RETURNING user_id, credits, card_ids;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("offer_id", offerId);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            long userId = reader.GetInt64(0);
            int offerAmount = reader.GetInt32(1);
            List<long> returningCards = reader.GetFieldValue<long[]>(2).ToList();

            await HutHelper.Deposit(userId, offerAmount);
            foreach (var returningCardId in returningCards)
            {
                var card = await HutManager.GetCard(returningCardId, userId);
                card.Card.mCardStateId = CardState.CARDHOUSE_CARDSTATE_FREE;
                await HutCardFactory.CreateOrUpdateCard(card.Card, userId);
            }

            await HutManager.IncrementVersionInfo(userId, HutManager.VersionType.Escrow);
        }
        else
        {
            return new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_NO_SUCH_TRADE);
        }

        return null;
    }

    public static async Task ExecuteTrade(long tradeId, long offerId, bool buyOut)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        var tradeSql = "SELECT user_id, card_id FROM hut_trade_info WHERE trade_id = @trade_id";
        var offerSql = "SELECT user_id, credits, card_ids FROM hut_offer_info WHERE offer_id = @offer_id";

        long sellerId, cardId, buyerId;
        int price;
        List<long> offerBackCards;

        await using (var cmd = new NpgsqlCommand(tradeSql, conn))
        {
            cmd.Parameters.AddWithValue("trade_id", tradeId);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) throw new Exception("Trade not found");
            sellerId = reader.GetInt64(0);
            cardId = reader.GetInt64(1);
        }

        await using (var cmd = new NpgsqlCommand(offerSql, conn))
        {
            cmd.Parameters.AddWithValue("offer_id", offerId);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) throw new Exception("Offer not found");
            buyerId = reader.GetInt64(0);
            price = reader.GetInt32(1);
            offerBackCards = reader.GetFieldValue<long[]>(reader.GetOrdinal("card_ids")).ToList();
        }

        await HutHelper.Deposit(sellerId, price);

        var cardData = (await HutManager.GetCard(cardId)).Card;
        cardData.mCardStateId = CardState.CARDHOUSE_CARDSTATE_FREE;
        if (buyOut)
        {
            await HutCardFactory.CreateOrUpdateCard(cardData, buyerId, DeckType.CARDHOUSE_DECK_UNASSIGNED);
            await HutManager.IncrementVersionInfo(buyerId, HutManager.VersionType.Unassigned);
            await RemoveWatching(tradeId, buyerId);
        }
        else
        {
            await HutCardFactory.CreateOrUpdateCard(cardData, buyerId, DeckType.CARDHOUSE_DECK_INBOX);
        }

        if (offerBackCards.Count >= 1)
        {
            foreach (var offerBackCardId in offerBackCards)
            {
                var offerBackCardData = await HutManager.GetCard(offerBackCardId, buyerId);
                offerBackCardData.Card.mCardStateId = CardState.CARDHOUSE_CARDSTATE_FREE;
                await HutCardFactory.CreateOrUpdateCard(offerBackCardData.Card, sellerId, DeckType.CARDHOUSE_DECK_UNASSIGNED);
            }

            await HutManager.IncrementVersionInfo(sellerId, HutManager.VersionType.Unassigned);
            await HutManager.IncrementVersionInfo(buyerId, HutManager.VersionType.Escrow);
        }

        await HutManager.IncrementVersionInfo(sellerId, HutManager.VersionType.Escrow);

        ScheduleDelayedSemaphoreRemoval(tradeId);
    }

    public static async Task InsertWatching(long tradeId, long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
            INSERT INTO hut_watching (user_id, trade_id)
            VALUES (@user_id, @trade_id)
            ON CONFLICT (user_id, trade_id) DO NOTHING;";

        await using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("trade_id", tradeId);

        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task RemoveWatching(long tradeId, long userId)
    {
        await RemoveWatching(new ISRemoveWatchRequest
        {
            mTradeIdList = new List<long>()
            {
                tradeId
            },
        }, userId);
    }

    public static async Task RemoveWatching(ISRemoveWatchRequest request, long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
        DELETE FROM hut_watching 
        WHERE user_id = @user_id 
        AND trade_id = ANY(@ids);";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("ids", request.mTradeIdList.ToArray());

        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<List<ISTradeInfo>> GetWatchedTrades(long userId)
    {
        var watchedTrades = new List<ISTradeInfo>();

        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        string sql = @"
            SELECT t.*,
                   GREATEST(0, (t.created_at_seconds + t.duration_seconds) - EXTRACT(EPOCH FROM NOW()))::INT AS expire_time
                   FROM hut_trade_info t
            INNER JOIN hut_watching w ON t.trade_id = w.trade_id
            WHERE w.user_id = @user_id";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("user_id", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            watchedTrades.Add(await HutHelper.ReadTrade(reader, userId));
        }

        return watchedTrades;
    }

    public static async Task<bool> IsWatching(long userId, long tradeId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        string sql = "SELECT EXISTS(SELECT 1 FROM hut_watching WHERE user_id = @user_id AND trade_id = @trade_id)";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("trade_id", tradeId);

        return (bool)await cmd.ExecuteScalarAsync();
    }

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

        if (states.Contains(7) && states.Contains(2))
        {
            return YourBid.CARDHOUSE_YOURBID_HIGHEST;
        }

        if (states.TrueForAll(s => s == 5))
        {
            return YourBid.CARDHOUSE_YOURBID_PREVIOUS;
        }

        return YourBid.CARDHOUSE_YOURBID_NONE;
    }


    public static async Task<List<ISOfferInfo>> SearchOffersAsync(ISGetOffersRequest request)
    {
        List<ISOfferInfo> results = new List<ISOfferInfo>();

        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
            SELECT * FROM hut_offer_info 
            WHERE trade_id = @tid AND offer_state = 1";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("tid", request.mTradeId);

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            results.Add(await HutHelper.ReadOffer(reader));
        }

        return results;
    }

    public static async Task<long> GetTradeId(long offerId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
            SELECT trade_id FROM hut_offer_info 
            WHERE offer_id = @oid;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("oid", offerId);

        var result = await cmd.ExecuteScalarAsync();
        return result != null ? (long)result : 0;
    }

    public static async Task<long> ActiveOffersCount(long tradeId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
            SELECT COUNT(*) FROM hut_offer_info 
            WHERE trade_id = @tid AND offer_state = 1;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("tid", tradeId);

        var result = await cmd.ExecuteScalarAsync();
        return (long)result;
    }

    private static void ScheduleDelayedSemaphoreRemoval(long tradeId)
    {
        _ = Task.Delay(TimeSpan.FromMinutes(1)).ContinueWith(_ =>
        {
            if (_tradeLocks.TryGetValue(tradeId, out var semaphore))
            {
                if (semaphore.CurrentCount == 1)
                {
                    _tradeLocks.TryRemove(tradeId, out SemaphoreSlim? _);
                }
            }
        });
    }
}