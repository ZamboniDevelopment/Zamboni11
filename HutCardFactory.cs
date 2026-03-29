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
    
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    
    private static readonly Dictionary<CardSubType, Range> TrainingCardDbIdRanges = new();
    private static readonly Dictionary<CardSubType, List<uint>> PlayerCardDbIdsByCardSubType = new();
    public static readonly Dictionary<int, Range> LeagueTeamsMapping = new();

    static HutCardFactory()
    {
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_HIGH, new Range(5003001, 5003005));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_LOW, new Range(5003006, 5003010));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_QUICKNESS, new Range(5003011, 5003015));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_POSITIONING, new Range(5003016, 5003020));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ATTRIBUTE_REBOUNDCONTROL, new Range(5003021, 5003025));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_GK_ALL, new Range(5003026, 5003028));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_SKATING, new Range(5003029, 5003033));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_SHOOTING, new Range(5003034, 5003038));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_HANDS, new Range(5003039, 5003043));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_CHECKING, new Range(5003044, 5003048));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ATTRIBUTE_DEFENSE, new Range(5003049, 5003053));
        TrainingCardDbIdRanges.Add(CardSubType.CARDHOUSE_CARD_TYPE_TRAINING_PLAYER_ALL, new Range(5003054, 5003056));

        PlayerCardDbIdsByCardSubType.Add(CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C, Program.Database.GetListDbIds(CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
        PlayerCardDbIdsByCardSubType.Add(CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW, Program.Database.GetListDbIds(CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW));
        PlayerCardDbIdsByCardSubType.Add(CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW, Program.Database.GetListDbIds(CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW));
        PlayerCardDbIdsByCardSubType.Add(CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D, Program.Database.GetListDbIds(CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
        PlayerCardDbIdsByCardSubType.Add(CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK, Program.Database.GetListDbIds(CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK));

        LeagueTeamsMapping.Add(0, new Range(0, 31)); //NHL
        LeagueTeamsMapping.Add(1, new Range(32, 61)); //AHL
        LeagueTeamsMapping.Add(2, new Range(62, 73)); //Elitserien
        LeagueTeamsMapping.Add(3, new Range(74, 87)); //Sm-Liiga
        LeagueTeamsMapping.Add(4, new Range(88, 102)); //DEL
        LeagueTeamsMapping.Add(5, new Range(103, 116)); //O2 Extraliga
        LeagueTeamsMapping.Add(6, new Range(117, 128)); //National League
        LeagueTeamsMapping.Add(7, new Range(129, 149)); //National
        LeagueTeamsMapping.Add(8, new Range(150, 169)); //OHL
        LeagueTeamsMapping.Add(9, new Range(170, 187)); //QMJHL
        LeagueTeamsMapping.Add(10, new Range(188, 209)); //WHL
        LeagueTeamsMapping.Add(11, new Range(210, 211)); //Prospects
    }

    public static async Task<CardData> CreateRandomHeadCoachCard(long owner)
    {
        return await CreateNonPlayerCard(owner, (uint)new Random().Next(2000000, 2000025 + 1), CardSubType.CARDHOUSE_CARD_TYPE_STAFF_HEADCOACH);
    }


    public static async Task<CardData> CreateRandomContractCard(long owner)
    {
        return await CreateNonPlayerCard(owner, (uint)new Random().Next(5001001, 5001011 + 1), CardSubType.CARDHOUSE_CARD_TYPE_CONTRACT_PLAYER);
    }

    public static async Task<CardData> CreateRandomTrainingCard(long owner)
    {
        var random = new Random().Next(TrainingCardDbIdRanges.Count);
        var cardType = TrainingCardDbIdRanges.ElementAt(random).Key;
        return await CreateNonPlayerCard(owner, (uint)new Random().Next(TrainingCardDbIdRanges[cardType].Start.Value, TrainingCardDbIdRanges[cardType].End.Value + 1), cardType);
    }

    public static async Task<CardData> CreateRandomLogoCard(long owner)
    {
        return await CreateNonPlayerCard(owner, (uint)new Random().Next(6000000, 6000211 + 1), CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_BADGE);
    }

    public static async Task<CardData> CreateRandomStadiumCard(long owner)
    {
        return await CreateNonPlayerCard(owner, (uint)new Random().Next(6200000, 6200005 + 1), CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_STADIUM);
    }

    public static async Task<CardData> CreateRandomJerseyCard(long owner, bool isHome, bool isRare)
    {
        if (isRare) return await CreateNonPlayerCard(owner, (uint)new Random().Next(6500001 - 1, 6500196 - 1 + 1), CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT);
        if (isHome) return await CreateNonPlayerCard(owner, (uint)new Random().Next(6300001 - 1, 6300212 - 1 + 1), CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT);
        if (!isHome) return await CreateNonPlayerCard(owner, (uint)new Random().Next(6400001 - 1, 6400212 - 1 + 1), CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT);
        return await CreateNonPlayerCard(owner, (uint)new Random().Next(6300001 - 1, 6300212 - 1 + 1), CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT);
    }

    public static async Task<int> TeamIdFromDbId(uint dbId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
        SELECT teamid FROM fcc_badges WHERE carddbid = @carddbid
        UNION ALL
        SELECT teamid FROM fcc_kits WHERE carddbid = @carddbid
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

    public static async Task<CardData> CreateNonPlayerCard(long owner, uint dbId, CardSubType cardSubType)
    {
        CardState cardState = CardState.CARDHOUSE_CARDSTATE_INVALID;
        DeckType deckType = DeckType.CARDHOUSE_DECK_UNASSIGNED;
        if (cardSubType == CardSubType.CARDHOUSE_CARD_TYPE_STAFF_HEADCOACH || CardHouseComponent.TrophyTypes.Contains(cardSubType))
        {
            cardState = CardState.CARDHOUSE_CARDSTATE_FREE;
            deckType = DeckType.CARDHOUSE_DECK_STICKERBOOK;
        }

        var cardData = new CardData()
        {
            mAttributes = new List<byte>(),
            mCardStateId = cardState,
            mCardId = 0,
            mCardDbId = dbId,
            mFormationId = 0,
            mFREE = 0,
            mCareerRemaining = 0,
            mInjuryGames = 0,
            mInjuryType = 0,
            mMaxTrainingCardsCanApply = 0,
            mNumberOfOwners = 0,
            mPreferredPositionId = (byte)cardSubType,
            mDiscardPrice = 0,
            mRareFlag = 0,
            mRating = 0,
            mSalaryCap = 0,
            mListStats = new List<int>(),
            mCardSubTypeId = cardSubType,
            mDateIssued = 0,
            mTeamId = (uint)await TeamIdFromDbId(dbId),
            mListTrainingCards = new List<int>(),
            mUsesRemaining = 0
        };
        return await CreateOrUpdateCard(cardData, owner, deckType);
    }

    public static async Task<CardData> CreateRandomPlayerCard(long owner, CardSubType position)
    {
        if (position > CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK) throw new Exception("Position must be 0-4");
        List<uint> dbIds = PlayerCardDbIdsByCardSubType[position];
        uint cardDbId = dbIds[new Random().Next(dbIds.Count)];
        return await CreatePlayerCard(owner, cardDbId);
    }

    public static async Task<CardData> CreatePlayerCard(long owner, uint dbId)
    {
        var staticCardData = await Program.Database.GetPlayerCardDataByDbId(dbId);
        var cardData = await CreateOrUpdateCard(staticCardData, owner, DeckType.CARDHOUSE_DECK_UNASSIGNED);
        return cardData;
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
        cmd.Parameters.AddWithValue("injury_games", (int)cardData.mInjuryGames);
        cmd.Parameters.AddWithValue("injury_type", (int)cardData.mInjuryType);
        cmd.Parameters.AddWithValue("morale", (int)cardData.mMaxTrainingCardsCanApply);
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