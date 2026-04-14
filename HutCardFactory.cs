using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Npgsql;
using Zamboni11.Components.NHL11;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutCardFactory
{
    private static readonly Random Random = new();

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static async Task<CardData> CreateRandomHeadCoachCard(long owner)
    {
        return await CreateNonPlayerCard(owner, (uint)Random.Next(2000000, 2000025 + 1), CardSubType.CARDHOUSE_CARD_TYPE_STAFF_HEADCOACH);
    }

    public static async Task<CardData> CreateRandomContractCard(long owner)
    {
        return await CreateNonPlayerCard(owner, (uint)Random.Next(5001001, 5001011 + 1), CardSubType.CARDHOUSE_CARD_TYPE_CONTRACT_PLAYER);
    }

    public static async Task<CardData> CreateRandomTrainingCard(long owner)
    {
        var list = await HutHelper.GetAllTrainingCardIds();
        var cardDbId = list[Random.Next(list.Count)];
        var trainingCard = await Database.GetTrainingCardByDbIdAsync((uint)cardDbId);
        return await CreateNonPlayerCard(owner, (uint)cardDbId, (CardSubType)trainingCard.CardSubtype);
    }

    public static async Task<CardData> CreateRandomLogoCard(long owner)
    {
        return await CreateNonPlayerCard(owner, (uint)Random.Next(6000000, 6000211 + 1), CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_BADGE);
    }

    public static async Task<CardData> CreateRandomStadiumCard(long owner)
    {
        return await CreateNonPlayerCard(owner, (uint)Random.Next(6200000, 6200005 + 1), CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_STADIUM);
    }

    public static async Task<CardData> CreateRandomJerseyCard(long owner, bool? isHome = null, bool? isRare = null)
    {
        var cardDbIds = await Database.GetKitCards(isHome, isRare);
        var kit = cardDbIds[Random.Next(cardDbIds.Count)];
        return await CreateNonPlayerCard(owner, kit.CardDbId, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT, (byte)(kit.IsHome ? 1 : 0));
    }
    
    public static async Task<CardData> RollPlayerCard(long owner, List<CardData> alreadyRolled, Range overall, bool guaranteeUnique, params CardSubType[] subTypes)
    {
        int[] excludeIds = alreadyRolled.Select(c => (int)c.mCardDbId).ToArray();

        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        var possibilities = new List<uint>();

        string sql = @"
            SELECT carddbid 
            FROM fcc_playercards p
            WHERE preferredposition = ANY(@subTypes) 
            AND rating >= @overallStart AND rating <= @overallEnd 
            AND NOT (carddbid = ANY(@excludeIds))
            AND (@guaranteeUnique = FALSE OR NOT EXISTS (
                SELECT 1 FROM hut_cards h 
                WHERE h.user_id = @owner AND h.db_id = p.carddbid AND deck_type != 6
            ))";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("subTypes", subTypes.Select(type => (int)type).ToArray());
        cmd.Parameters.AddWithValue("overallStart", overall.Start.Value);
        cmd.Parameters.AddWithValue("overallEnd", overall.End.Value);
        cmd.Parameters.AddWithValue("excludeIds", excludeIds);
        cmd.Parameters.AddWithValue("owner", owner);
        cmd.Parameters.AddWithValue("guaranteeUnique", guaranteeUnique);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            possibilities.Add((uint)reader.GetInt32(0));
        }

        if (possibilities.Count == 0)
        {
            if (overall.End.Value <= 1) return await RollPlayerCard(owner, alreadyRolled, new Range(0, 100), false, subTypes);

            return await RollPlayerCard(owner, alreadyRolled, new Range(overall.Start.Value - 1, overall.End.Value - 1), guaranteeUnique, subTypes);
        }

        return await CreatePlayerCard(owner, possibilities[Random.Next(possibilities.Count)]);
    }

    public static async Task<int> TeamIdFromDbId(uint dbId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
        SELECT teamid FROM fcc_badges WHERE carddbid = @carddbid
        UNION ALL
        SELECT teamid FROM fcc_kitcards WHERE carddbid = @carddbid
        LIMIT 1;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("carddbid", (int)dbId);

        var result = await cmd.ExecuteScalarAsync();

        if (result != null && result != DBNull.Value)
        {
            return Convert.ToInt32(result);
        }

        return 0;
    }

    public static async Task<CardData> CreateNonPlayerCard(long owner, uint dbId, CardSubType cardSubType, byte formationId = 0)
    {
        CardState cardState = CardState.CARDHOUSE_CARDSTATE_FREE;
        DeckType deckType = DeckType.CARDHOUSE_DECK_UNASSIGNED;
        if (CardHouseComponent.TrophyTypes.Contains(cardSubType))
        {
            deckType = DeckType.CARDHOUSE_DECK_STICKERBOOK;
        }

        var cardData = new CardData()
        {
            mAttributes = new List<byte>(),
            mCardStateId = cardState,
            mCardId = 0,
            mCardDbId = dbId,
            mFormationId = formationId,
            mFREE = 0,
            mCareerRemaining = 0,
            mInjuryGames = 0,
            mInjuryType = 0,
            mMaxTrainingCardsCanApply = 0,
            mNumberOfOwners = 0,
            mPreferredPositionId = (byte)cardSubType,
            mDiscardPrice = 100,
            mRareFlag = 0,
            mRating = 0,
            mSalaryCap = 0,
            mListStats = new List<int>(),
            mCardSubTypeId = cardSubType,
            mDateIssued = Util.TimeNow(),
            mTeamId = (uint)await TeamIdFromDbId(dbId),
            mListTrainingCards = new List<int>(),
            mUsesRemaining = 0
        };
        return await CreateOrUpdateCard(cardData, owner, deckType);
    }

    public static async Task<CardData> CreatePlayerCard(long owner, uint dbId)
    {
        var staticCardData = await Program.Database.GetPlayerCardDataByDbId(dbId);
        if (!staticCardData.HasValue) throw new Exception();
        return await CreateOrUpdateCard(staticCardData.Value, owner, DeckType.CARDHOUSE_DECK_UNASSIGNED);
    }

    public static async Task<CardData> CreateOrUpdateCard(CardData cardData, long ownerUserId, DeckType? deckType = null)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        string cardIdValue = cardData.mCardId == 0 ? "DEFAULT" : "@card_id";
        bool updateDeck = deckType.HasValue;

        string sql = $@"
        INSERT INTO hut_cards (
            card_id, user_id, attributes, state_id, db_id, formation_id, 
            free, career_remaining, injury_games, injury_type, 
            morale, preferred_position_id, discard_price, 
            rare_flag, rating, salary_cap,
            list_stats, sub_type, date_issued,
            team_id, list_training_cards, uses_remaining
            {(updateDeck ? ", deck_type" : "")} 
        ) 
        VALUES (
            {cardIdValue}, @user_id, @attributes, @state_id, @db_id, @formation_id, 
            @free, @career_remaining, @injury_games, @injury_type, 
            @morale, @preferred_position_id, @discard_price, 
            @rare_flag, @rating, @salary_cap,
            @list_stats, @sub_type, @date_issued, @team_id, @list_training_cards, 
            @uses_remaining
            {(updateDeck ? ", @deck_type" : "")}
        )
        ON CONFLICT (card_id) DO UPDATE SET
            user_id = EXCLUDED.user_id,
            attributes = EXCLUDED.attributes,
            state_id = EXCLUDED.state_id,
            db_id = EXCLUDED.db_id,
            formation_id = EXCLUDED.formation_id,
            free = EXCLUDED.free,
            career_remaining = EXCLUDED.career_remaining,
            injury_games = EXCLUDED.injury_games,
            injury_type = EXCLUDED.injury_type,
            morale = EXCLUDED.morale,
            preferred_position_id = EXCLUDED.preferred_position_id,
            discard_price = EXCLUDED.discard_price,
            rare_flag = EXCLUDED.rare_flag,
            rating = EXCLUDED.rating,
            salary_cap = EXCLUDED.salary_cap,
            list_stats = EXCLUDED.list_stats,
            sub_type = EXCLUDED.sub_type,
            team_id = EXCLUDED.team_id,
            list_training_cards = EXCLUDED.list_training_cards,
            uses_remaining = EXCLUDED.uses_remaining
            {(updateDeck ? ", deck_type = EXCLUDED.deck_type" : "")}
        RETURNING card_id;";

        await using var cmd = new NpgsqlCommand(sql, conn);

        if (cardData.mCardId != 0) cmd.Parameters.AddWithValue("card_id", cardData.mCardId);

        cmd.Parameters.AddWithValue("user_id", ownerUserId);
        cmd.Parameters.AddWithValue("attributes", cardData.mAttributes.Select(b => (short)b).ToArray());
        cmd.Parameters.AddWithValue("state_id", (int)cardData.mCardStateId);
        cmd.Parameters.AddWithValue("db_id", (long)cardData.mCardDbId);
        cmd.Parameters.AddWithValue("formation_id", (int)cardData.mFormationId);
        cmd.Parameters.AddWithValue("free", (int)cardData.mFREE);
        cmd.Parameters.AddWithValue("career_remaining", (int)cardData.mCareerRemaining);
        cmd.Parameters.AddWithValue("injury_games", (short)cardData.mInjuryGames);
        cmd.Parameters.AddWithValue("injury_type", (short)cardData.mInjuryType);
        cmd.Parameters.AddWithValue("morale", (short)cardData.mMaxTrainingCardsCanApply);
        cmd.Parameters.AddWithValue("preferred_position_id", (int)cardData.mPreferredPositionId);
        cmd.Parameters.AddWithValue("discard_price", (int)cardData.mDiscardPrice);
        cmd.Parameters.AddWithValue("rare_flag", (int)cardData.mRareFlag);
        cmd.Parameters.AddWithValue("rating", (int)cardData.mRating);
        cmd.Parameters.AddWithValue("salary_cap", (int)cardData.mSalaryCap);
        cmd.Parameters.AddWithValue("list_stats", cardData.mListStats.ToArray());
        cmd.Parameters.AddWithValue("list_training_cards", cardData.mListTrainingCards.ToArray());
        cmd.Parameters.AddWithValue("sub_type", (int)cardData.mCardSubTypeId);
        cmd.Parameters.AddWithValue("date_issued", (long)Util.TimeNow());
        cmd.Parameters.AddWithValue("team_id", (int)cardData.mTeamId);
        cmd.Parameters.AddWithValue("uses_remaining", (int)cardData.mUsesRemaining);
        if (updateDeck)
        {
            cmd.Parameters.AddWithValue("deck_type", (int)deckType.Value);
        }

        cardData.mCardId = (long)await cmd.ExecuteScalarAsync();

        return cardData;
    }
}