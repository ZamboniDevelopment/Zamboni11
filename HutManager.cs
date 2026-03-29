using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlazeCommon;
using NLog;
using Npgsql;
using Zamboni11.Components.NHL11;
using Zamboni11.Components.NHL11.Requests;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutManager
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static async Task<GamerInfo?> GetGamerInfo(long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = "SELECT * FROM hut_gamer_info WHERE user_id = @uid;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("uid", userId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new GamerInfo
            {
                mCustomTactics = reader.GetString(reader.GetOrdinal("custom_tactics")),
                mTeamFormation = (uint)reader.GetInt32(reader.GetOrdinal("team_formation")),
                mKickTakers = reader.GetString(reader.GetOrdinal("kick_takers")),
                mLineup = reader.GetString(reader.GetOrdinal("lineup")),
                mLogoUrl = reader.GetString(reader.GetOrdinal("logo_url")),
                mTeamName = reader.GetString(reader.GetOrdinal("team_name")),
                mPlayoffsQualified = (uint)reader.GetInt32(reader.GetOrdinal("playoffs_qualified")),
                mPlayoffWon = (uint)reader.GetInt32(reader.GetOrdinal("playoff_won")),
                mQuickTactics = reader.GetString(reader.GetOrdinal("quick_tactics")),
                mSpecialPacksBought = (uint)reader.GetInt32(reader.GetOrdinal("special_packs_bought")),
                mTeamAbbreviation = reader.GetString(reader.GetOrdinal("team_abbreviation")),
                mTournaments = reader.GetString(reader.GetOrdinal("tournaments")),
                mTPPL = (uint)reader.GetInt32(reader.GetOrdinal("tppl")),
                mTrophies = reader.GetString(reader.GetOrdinal("trophies"))
            };
        }

        return null;
    }

    public static async Task SetGamerInfo(GamerInfo gamerInfo, long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
        INSERT INTO hut_gamer_info (
            user_id, custom_tactics, team_formation, kick_takers, lineup, 
            logo_url, team_name, playoffs_qualified, playoff_won, 
            quick_tactics, special_packs_bought, team_abbreviation, 
            tournaments, tppl, trophies
        ) 
        VALUES (
            @user_id, @custom_tactics, @team_formation, @kick_takers, @lineup, 
            @logo_url, @team_name, @playoffs_qualified, @playoff_won, 
            @quick_tactics, @special_packs_bought, @team_abbreviation, 
            @tournaments, @tppl, @trophies
        )
        ON CONFLICT (user_id) DO UPDATE SET
            custom_tactics = EXCLUDED.custom_tactics,
            team_formation = EXCLUDED.team_formation,
            kick_takers = EXCLUDED.kick_takers,
            lineup = EXCLUDED.lineup,
            logo_url = EXCLUDED.logo_url,
            team_name = EXCLUDED.team_name,
            playoffs_qualified = EXCLUDED.playoffs_qualified,
            playoff_won = EXCLUDED.playoff_won,
            quick_tactics = EXCLUDED.quick_tactics,
            special_packs_bought = EXCLUDED.special_packs_bought,
            team_abbreviation = EXCLUDED.team_abbreviation,
            tournaments = EXCLUDED.tournaments,
            tppl = EXCLUDED.tppl,
            trophies = EXCLUDED.trophies;";

        await using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("custom_tactics", gamerInfo.mCustomTactics);
        cmd.Parameters.AddWithValue("team_formation", (int)gamerInfo.mTeamFormation);
        cmd.Parameters.AddWithValue("kick_takers", gamerInfo.mKickTakers);
        cmd.Parameters.AddWithValue("lineup", gamerInfo.mLineup);
        cmd.Parameters.AddWithValue("logo_url", gamerInfo.mLogoUrl);
        cmd.Parameters.AddWithValue("team_name", gamerInfo.mTeamName ?? "");
        cmd.Parameters.AddWithValue("playoffs_qualified", (int)gamerInfo.mPlayoffsQualified);
        cmd.Parameters.AddWithValue("playoff_won", (int)gamerInfo.mPlayoffWon);
        cmd.Parameters.AddWithValue("quick_tactics", gamerInfo.mQuickTactics);
        cmd.Parameters.AddWithValue("special_packs_bought", (int)gamerInfo.mSpecialPacksBought);
        cmd.Parameters.AddWithValue("team_abbreviation", gamerInfo.mTeamAbbreviation);
        cmd.Parameters.AddWithValue("tournaments", gamerInfo.mTournaments);
        cmd.Parameters.AddWithValue("tppl", (int)gamerInfo.mTPPL);
        cmd.Parameters.AddWithValue("trophies", gamerInfo.mTrophies);

        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<GeneralInfo?> GetGeneralInfo(long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = "SELECT * FROM hut_general_info WHERE user_id = @uid;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("uid", userId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new GeneralInfo
            {
                mCredits = reader.GetInt32(reader.GetOrdinal("pucks")),
                mStats = reader.GetFieldValue<int[]>(reader.GetOrdinal("stats")).ToList(),
            };
        }

        return null;
    }

    public static async Task<GeneralInfo> SetGeneralInfo(GeneralInfo generalInfo, long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
        INSERT INTO hut_general_info (
            user_id, pucks, stats
        ) 
        VALUES (
            @user_id, @pucks, @stats
        )
        ON CONFLICT (user_id) DO UPDATE SET
            pucks = EXCLUDED.pucks,
            stats = EXCLUDED.stats;";

        await using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("pucks", generalInfo.mCredits);
        cmd.Parameters.AddWithValue("stats", generalInfo.mStats.ToArray());

        await cmd.ExecuteNonQueryAsync();

        await IncrementVersionInfo(userId, VersionType.General);

        return generalInfo;
    }

    public static async Task<SquadInfo?> GetSquadInfo(long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = "SELECT * FROM hut_squad_info WHERE user_id = @user_id;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("user_id", userId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            List<CardData> playersOrdered = new List<CardData>();
            foreach (var cardId in reader.GetFieldValue<List<long>>(reader.GetOrdinal("players")))
            {
                playersOrdered.Add((await GetCard(cardId, userId)).Card);
            }

            return new SquadInfo
            {
                mChemistry = (uint)reader.GetInt32(reader.GetOrdinal("chemistry")),
                // mCHNG = ,
                mFormationId = (uint)reader.GetInt32(reader.GetOrdinal("formation_id")),
                mLines = reader.GetFieldValue<int[]>(reader.GetOrdinal("lines")).ToList(),
                mManager = (await GetCard(reader.GetInt64(reader.GetOrdinal("manager")))).Card,
                mSquadName = reader.GetString(reader.GetOrdinal("squad_name")),
                mPlayers = playersOrdered,
                mStarRating = (uint)reader.GetInt32(reader.GetOrdinal("star_rating")),
                mSquadId = (uint)reader.GetInt32(reader.GetOrdinal("squad_id"))
            };
        }

        return null;
    }

    public static async Task<VersionInfo?> GetVersionInfo(long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = "SELECT * FROM hut_version_info WHERE user_id = @user_id;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("user_id", userId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new VersionInfo
            {
                mVersionEscrow = (uint)reader.GetInt32(reader.GetOrdinal("escrow_version")),
                mVersionGeneral = (uint)reader.GetInt32(reader.GetOrdinal("general_version")),
                mVersionUnassigned = (uint)reader.GetInt32(reader.GetOrdinal("unassigned_version")),
            };
        }

        return null;
    }

    public static async Task<VersionInfo> CreateVersionInfo(VersionInfo versionInfo, long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
        INSERT INTO hut_version_info (
            user_id, escrow_version, general_version, unassigned_version
        ) 
        VALUES (
            @user_id, @escrow_version, @general_version, @unassigned_version
        );";

        await using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("escrow_version", (int)versionInfo.mVersionEscrow);
        cmd.Parameters.AddWithValue("general_version", (int)versionInfo.mVersionGeneral);
        cmd.Parameters.AddWithValue("unassigned_version", (int)versionInfo.mVersionUnassigned);

        await cmd.ExecuteNonQueryAsync();
        return versionInfo;
    }

    public enum VersionType
    {
        Escrow,
        General,
        Unassigned
    }

    public static async Task<VersionInfo> IncrementVersionInfo(long userId, VersionType type)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
        INSERT INTO hut_version_info (user_id, escrow_version, general_version, unassigned_version)
        VALUES (@uid, 1, 1, 1)
        ON CONFLICT (user_id) DO UPDATE SET
            escrow_version = CASE WHEN @type = 'Escrow' THEN hut_version_info.escrow_version + 1 ELSE hut_version_info.escrow_version END,
            general_version = CASE WHEN @type = 'General' THEN hut_version_info.general_version + 1 ELSE hut_version_info.general_version END,
            unassigned_version = CASE WHEN @type = 'Unassigned' THEN hut_version_info.unassigned_version + 1 ELSE hut_version_info.unassigned_version END
        RETURNING escrow_version, general_version, unassigned_version;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("uid", userId);
        cmd.Parameters.AddWithValue("type", type.ToString());

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new VersionInfo
            {
                mVersionEscrow = (uint)reader.GetInt32(reader.GetOrdinal("escrow_version")),
                mVersionGeneral = (uint)reader.GetInt32(reader.GetOrdinal("general_version")),
                mVersionUnassigned = (uint)reader.GetInt32(reader.GetOrdinal("unassigned_version"))
            };
        }

        return new VersionInfo { mVersionEscrow = 1, mVersionGeneral = 1, mVersionUnassigned = 1 };
    }

    public static async Task<List<CardData>> GetCardList(long userId, DeckType deckType, CardState cardState)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = "SELECT * FROM hut_cards WHERE user_id = @user_id AND deck_type = @deck_type AND state_id = @state_id;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("deck_type", (int)deckType);
        cmd.Parameters.AddWithValue("state_id", (short)cardState);

        await using var reader = await cmd.ExecuteReaderAsync();

        List<CardData> cardDataList = new List<CardData>();

        while (await reader.ReadAsync())
        {
            cardDataList.Add(HutHelper.ReadCardData(reader));
        }

        return cardDataList;
    }


    public static async Task<(CardData Card, DeckType DeckType)> GetCard(long cardId, long userId = 0)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        var sql = new StringBuilder(@"
        SELECT *, deck_type 
        FROM hut_cards
        WHERE card_id = @card_id");

        if (userId != 0) sql.Append(" AND user_id = @user_id");

        await using var cmd = new NpgsqlCommand(sql.ToString(), conn);
        cmd.Parameters.AddWithValue("card_id", cardId);
        if (userId != 0) cmd.Parameters.AddWithValue("user_id", userId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var card = HutHelper.ReadCardData(reader);
            DeckType deckType = (DeckType)reader.GetInt32(reader.GetOrdinal("deck_type"));

            return (card, deckType);
        }

        return (new CardData(), DeckType.CARDHOUSE_DECK_GENERAL);
    }
    
    public static async Task<(CardData Card, DeckType DeckType)> GetCard(uint cardDbId, long userId = 0)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        var sql = new StringBuilder(@"
            SELECT *, deck_type 
            FROM hut_cards
            WHERE card_id = @card_id");

        if (userId != 0) sql.Append(" AND user_id = @user_id");

        await using var cmd = new NpgsqlCommand(sql.ToString(), conn);
        cmd.Parameters.AddWithValue("db_id", (int)cardDbId);
        if (userId != 0) cmd.Parameters.AddWithValue("user_id", userId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var card = HutHelper.ReadCardData(reader);
            DeckType deckType = (DeckType)reader.GetInt32(reader.GetOrdinal("deck_type"));

            return (card, deckType);
        }

        return (new CardData(), DeckType.CARDHOUSE_DECK_GENERAL);
    }

    public static async Task SetSquadInfo(SquadSaveRequest request, long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
        INSERT INTO hut_squad_info (
            user_id, chemistry, formation_id, lines, 
            manager, squad_name, players, star_rating, squad_id
        ) 
        VALUES (
            @user_id, @chemistry, @formation_id, @lines, 
            @manager, @squad_name, @players, @star_rating, @squad_id
        )
        ON CONFLICT (user_id) DO UPDATE SET
            user_id = EXCLUDED.user_id,
            chemistry = EXCLUDED.chemistry,
            formation_id = EXCLUDED.formation_id,
            lines = EXCLUDED.lines,
            manager = EXCLUDED.manager,
            squad_name = EXCLUDED.squad_name,
            players = EXCLUDED.players,
            star_rating = EXCLUDED.star_rating,
            squad_id = EXCLUDED.squad_id;";

        await using var cmd = new NpgsqlCommand(sql, conn);

        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("chemistry", (int)request.mChemistry);
        cmd.Parameters.AddWithValue("formation_id", (int)request.mFormation);
        cmd.Parameters.AddWithValue("lines", request.mLines);
        cmd.Parameters.AddWithValue("manager", request.mManager);
        cmd.Parameters.AddWithValue("squad_name", request.mSquadName);
        cmd.Parameters.AddWithValue("players", request.mPlayers);
        cmd.Parameters.AddWithValue("star_rating", (int)request.mStarRating);
        cmd.Parameters.AddWithValue("squad_id", (int)request.mSquadId);

        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<Dictionary<int, int>> GetTeamCardCountsAsync(long userId, int leagueId, DeckType deckType, params CardSubType[] subTypes)
    {
        var counts = new Dictionary<int, int>();

        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        string sql = @"
            SELECT team_id, COUNT(*) 
            FROM hut_cards 
            WHERE user_id = @user_id 
            AND team_id >= @startId AND team_id <= @endId 
            AND deck_type = @deck_type";

        if (subTypes.Length > 0)
        {
            sql += " AND sub_type = ANY(@sub_types)";
        }

        sql += " GROUP BY team_id";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("startId", HutCardFactory.LeagueTeamsMapping[leagueId].Start.Value);
        cmd.Parameters.AddWithValue("endId", HutCardFactory.LeagueTeamsMapping[leagueId].End.Value);
        cmd.Parameters.AddWithValue("deck_type", (int)deckType);


        if (subTypes.Length > 0)
        {
            cmd.Parameters.AddWithValue("sub_types", subTypes.Select(s => (short)s).ToArray());
        }

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            counts[reader.GetInt32(0)] = (int)reader.GetInt64(1);
        }

        return counts;
    }

    public static async Task<int> GetCardCountAsync(long userId, DeckType deckType, params CardSubType[] subTypes)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        string sql = "SELECT COUNT(*) FROM hut_cards WHERE user_id = @user_id";

        sql += " AND deck_type = @deck_type";

        if (subTypes.Length > 0)
        {
            sql += " AND sub_type = ANY(@sub_types)";
        }

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("deck_type", (int)deckType);

        if (subTypes.Length > 0)
        {
            cmd.Parameters.AddWithValue("sub_types", subTypes.Select(s => (short)s).ToArray());
        }

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public static async Task<List<CardData>> GetCardList(long userId, StickerBookSearchRequest request)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        var sql = new StringBuilder(@"
        SELECT * 
              FROM hut_cards
        WHERE user_id = @user_id");

        var deckType = DeckType.CARDHOUSE_DECK_STICKERBOOK;

        sql.Append(" AND deck_type = @deck_type");
        switch (request.mCollectionSearchCardType)
        {
            //Here we might have to filter based if its in players active roster (SquadInfo)
            //but seems it's not needed because client has up to date info on his Squad all the time,
            //and client doesnt mind sending him again his whole squad
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_ALL: break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_HEADCOACH: sql.Append(" AND sub_type = 6"); break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_BADGE: sql.Append(" AND sub_type = 12"); break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_STADIUM: sql.Append(" AND sub_type = 11"); break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_PLAYER_C: sql.Append(" AND sub_type = 0"); break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_PLAYER_LW: sql.Append(" AND sub_type = 1"); break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_PLAYER_RW: sql.Append(" AND sub_type = 2"); break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_PLAYER_DEFENDER: sql.Append(" AND sub_type = 3"); break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_PLAYER_GK: sql.Append(" AND sub_type = 4"); break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_PLAYER_ALL: sql.Append(" AND sub_type BETWEEN 0 AND 4"); break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_PLAYER: sql.Append(" AND sub_type BETWEEN 0 AND 4"); break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_DEVELOPMENT: sql.Append(" AND sub_type BETWEEN 51 AND 62"); break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_OFFLINE_TROPHY: sql.Append(" AND sub_type = 145"); break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_ONLINE_TROPHY: sql.Append(" AND sub_type = 146"); break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_LIVE_TROPHY: sql.Append(" AND sub_type = 147"); break;
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_PLAYOFF_TROPHY: sql.Append(" AND sub_type = 148"); break;
            default: throw new NotImplementedException();
        }

        if (request.mLeagueId >= 0)
        {
            var range = HutCardFactory.LeagueTeamsMapping[request.mLeagueId];
            sql.Append(" AND team_id BETWEEN " + range.Start.Value + " AND " + range.End.Value + "");
        }

        if (request.mTeamId >= 0)
        {
            sql.Append(" AND team_id = " + request.mTeamId);
        }

        await using var cmd = new NpgsqlCommand(sql.ToString(), conn);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("deck_type", (int)deckType);

        await using var reader = await cmd.ExecuteReaderAsync();

        List<CardData> cardDataList = new List<CardData>();

        while (await reader.ReadAsync())
        {
            cardDataList.Add(HutHelper.ReadCardData(reader));
        }

        return cardDataList;
    }

    public static async Task HardDelete(long userId, long? cardId = null)
    {
        await using var connection = new NpgsqlConnection(Database.ConnectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            string[] tables = cardId.HasValue
                ? new[] { "hut_offer_info", "hut_trade_info", "hut_cards" }
                : new[] { "hut_cards", "hut_gamer_info", "hut_general_info", "hut_squad_info", "hut_version_info", "hut_offer_info", "hut_trade_info" };

            foreach (var table in tables)
            {
                if (cardId.HasValue && table == "hut_offer_info")
                {
                    var query = $"UPDATE {table} SET card_ids = array_remove(card_ids, @cardId) WHERE user_id = @userId"; //Todo where trade is not expired
                    await using var cmd = new NpgsqlCommand(query, connection, transaction);
                    cmd.Parameters.AddWithValue("userId", userId);
                    cmd.Parameters.AddWithValue("cardId", cardId.Value);
                    await cmd.ExecuteNonQueryAsync();
                }
                else
                {
                    bool useCardFilter = cardId.HasValue && (table == "hut_cards" || table == "hut_trade_info");
                    string columnName = useCardFilter ? "card_id" : "user_id";
                    long idValue = useCardFilter ? cardId.Value : userId;

                    var query = $"DELETE FROM {table} WHERE {columnName} = @id";
                    await using var cmd = new NpgsqlCommand(query, connection, transaction);
                    cmd.Parameters.AddWithValue("id", idValue);
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}