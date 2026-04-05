using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blaze3SDK.Blaze.GameReportingLegacy;
using NLog;
using Npgsql;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class Database
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly string ConnectionString = Program.ZamboniConfig.DatabaseConnectionString;
    public readonly bool isEnabled;

    private uint fallbackGameIdCounter = 1;

    public Database()
    {
        try
        {
            using var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();

            isEnabled = true;
            Logger.Warn("Database is accessible.");
        }
        catch (Exception)
        {
            isEnabled = false;
            Logger.Warn("Database is not accessible. \n" +
                        "- Gamedata wont be saved\n" +
                        "- HUT will not work");
            return;
        }

        CreateGameIdSequence();

        CreateGamesTable();
        CreateReportTable();
        CreateOtpReportTable();
        CreateSoReportTable();
        CreateHutReportTable();

        CreateTradeInfoTable();
        CreateOfferInfoTable();
        CreateWatchingTable();

        CreateHutGamerInfoTable();
        CreateHutSquadInfoTable();
        CreateHutVersionInfoTable();
        CreateHutGeneralInfoTable();

        CreateHutCardsTable();

        CreateHutTournamentsTable();
        // CreateHutTournamentAssociationTable();
    }


    private void CreateHutGeneralInfoTable()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS hut_general_info (
                    user_id BIGINT PRIMARY KEY,
                    pucks INTEGER DEFAULT 100,
                    stats INTEGER[] DEFAULT '{}'
                );";

        using var cmd = new NpgsqlCommand(createTableQuery, conn);
        cmd.ExecuteNonQuery();
    }

    private void CreateHutVersionInfoTable()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS hut_version_info (
                    user_id BIGINT PRIMARY KEY,
                    escrow_version INTEGER,
                    general_version INTEGER,
                    unassigned_version INTEGER
                );";

        using var cmd = new NpgsqlCommand(createTableQuery, conn);
        cmd.ExecuteNonQuery();
    }

    private void CreateHutGamerInfoTable()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS hut_gamer_info (
                    user_id BIGINT PRIMARY KEY,
                    custom_tactics VARCHAR,
                    team_formation INTEGER,
                    kick_takers VARCHAR,
                    lineup VARCHAR,
                    logo_url VARCHAR,
                    team_name VARCHAR,
                    playoffs_qualified INTEGER,
                    playoff_won INTEGER,
                    quick_tactics VARCHAR,
                    special_packs_bought INTEGER,
                    team_abbreviation VARCHAR,
                    tournaments VARCHAR,
                    tppl INTEGER,
                    trophies VARCHAR
                );";

        using var cmd = new NpgsqlCommand(createTableQuery, conn);
        cmd.ExecuteNonQuery();
    }

    private void CreateHutSquadInfoTable()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS hut_squad_info (
                    user_id BIGINT PRIMARY KEY,
                    chemistry INTEGER,
                    formation_id INTEGER,
                    lines INTEGER[] DEFAULT '{}',
                    manager BIGINT,
                    squad_name VARCHAR,
                    players BIGINT[] DEFAULT '{}',
                    star_rating INTEGER,
                    squad_id INTEGER
                );";

        using var cmd = new NpgsqlCommand(createTableQuery, conn);
        cmd.ExecuteNonQuery();
    }

    private void CreateHutCardsTable()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS hut_cards (
                    card_id BIGSERIAL PRIMARY KEY,
                    user_id BIGINT,
                    
                    attributes SMALLINT[] DEFAULT '{}',
                    state_id SMALLINT,
                    db_id INTEGER,
                    formation_id SMALLINT,
                    free SMALLINT,
                    career_remaining SMALLINT,
                    injury_games SMALLINT,
                    injury_type SMALLINT,
                    morale SMALLINT, --mMaxTrainingCardsCanApply/Potential
                    preferred_position_id SMALLINT,
                    discard_price SMALLINT,
                    rare_flag SMALLINT,
                    rating SMALLINT,
                    salary_cap INTEGER,
                    list_stats INTEGER[] DEFAULT '{}',
                    sub_type SMALLINT,
                    date_issued BIGINT,
                    team_id INTEGER,
                    list_training_cards INTEGER[] DEFAULT '{}',
                    uses_remaining SMALLINT,
                    deck_type INTEGER DEFAULT 1
                );";

        using var cmd = new NpgsqlCommand(createTableQuery, conn);
        cmd.ExecuteNonQuery();
    }

    private void CreateHutTournamentsTable()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS hut_tournaments (
                    user_id BIGINT,
                    tournament_type INTEGER,
                    tournament_id INTEGER,
                    blaze_tournament_id  INTEGER,
                    active BOOLEAN,
                    tournament_data BYTEA,
                    PRIMARY KEY (user_id, tournament_type)
                );";

        using var cmd = new NpgsqlCommand(createTableQuery, conn);
        cmd.ExecuteNonQuery();
    }

    // private void CreateHutTournamentAssociationTable()
    // {
    //     using var conn = new NpgsqlConnection(ConnectionString);
    //     conn.Open();
    //
    //     const string createTableQuery = @"
    //             CREATE TABLE IF NOT EXISTS hut_tournament_associations (
    //                 user_id BIGINT,
    //                 tournament_id INTEGER
    //             );";
    //
    //     using var cmd = new NpgsqlCommand(createTableQuery, conn);
    //     cmd.ExecuteNonQuery();
    // }

    private void CreateTradeInfoTable()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS hut_trade_info (
                    trade_id BIGSERIAL PRIMARY KEY,
                    user_id BIGINT,
                    card_id BIGINT,
                    starting_price INTEGER,
                    highest_bid INTEGER DEFAULT 0,
                    buy_out_price INTEGER,
                    seller_name VARCHAR,
                    trade_state INTEGER,
                    duration_seconds INTEGER,
                    created_at_seconds BIGINT
                );";

        using var cmd = new NpgsqlCommand(createTableQuery, conn);
        cmd.ExecuteNonQuery();
    }

    private void CreateOfferInfoTable()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS hut_offer_info (
                    offer_id BIGSERIAL PRIMARY KEY,
                    trade_id BIGINT,
                    user_id BIGINT,
                    offer_state INTEGER,
                    credits INTEGER,
                    card_ids BIGINT[] DEFAULT '{}',
                    created_at_seconds BIGINT
                );";

        using var cmd = new NpgsqlCommand(createTableQuery, conn);
        cmd.ExecuteNonQuery();
    }

    private void CreateWatchingTable()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS hut_watching (
                user_id BIGINT,
                trade_id BIGINT,
                PRIMARY KEY (user_id, trade_id)
            );";

        using var cmd = new NpgsqlCommand(createTableQuery, conn);
        cmd.ExecuteNonQuery();
    }

    private void CreateGameIdSequence()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createSequenceQuery = @"
            CREATE SEQUENCE IF NOT EXISTS zamboni_game_id_seq
            START 1
            INCREMENT 1;
        ";

        using var cmd = new NpgsqlCommand(createSequenceQuery, conn);
        cmd.ExecuteNonQuery();
    }

    private void CreateGamesTable()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS games (
                    game_id BIGINT PRIMARY KEY,
                    fnsh BOOLEAN,
                    gtyp INTEGER,
                    venue INTEGER,
                    ""time"" INTEGER,
                    sku INTEGER,
                    skil INTEGER,
                    shootout INTEGER,
                    pnum INTEGER,
                    prcs BOOLEAN,
                    plen INTEGER,
                    ot INTEGER,
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );";

        using var cmd = new NpgsqlCommand(createTableQuery, conn);
        cmd.ExecuteNonQuery();
    }

    private void CreateReportTable()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS reports (
                -- Primary Keys / Identifiers
                game_id BIGINT NOT NULL,
                user_id BIGINT NOT NULL,
                -- Network and Bandwidth Stats
                bandavggm INTEGER,
                bandavgnet INTEGER,
                bandhigm INTEGER,
                bandhinet INTEGER,
                bandlowgm INTEGER,
                bandlownet INTEGER,
                bytesrcvdgm INTEGER,
                bytesrcvdnet INTEGER,
                bytessentgm INTEGER,
                bytessentnet INTEGER,
                droppkts INTEGER,
                lateavggm INTEGER,
                lateavgnet INTEGER,
                latehigm INTEGER,
                latehinet INTEGER,
                latelowgm INTEGER,
                latelownet INTEGER,
                latesdevgm INTEGER,
                latesdevnet INTEGER,
                pktloss INTEGER,
                -- Performance, Synchronization, and Session Stats
                fpsavg INTEGER,
                fpsdev INTEGER,
                fpshi INTEGER,
                fpslow INTEGER,
                gdesyncend INTEGER,
                gdesyncrsn INTEGER,
                gendphase INTEGER,
                gresult INTEGER,
                grpttype INTEGER,
                grptver VARCHAR,
                guests0 INTEGER,
                guests1 INTEGER,
                usersend0 INTEGER,
                usersend1 INTEGER,
                usersstrt0 INTEGER,
                usersstrt1 INTEGER,
                voipend0 INTEGER,
                voipend1 INTEGER,
                voipstrt0 INTEGER,
                voipstrt1 INTEGER,
                -- Player Metadata and Game Outcome
                gamertag VARCHAR,
                name VARCHAR,
                team INTEGER,
                team_name VARCHAR,
                home INTEGER,
                quit INTEGER,
                disc INTEGER,
                cheat INTEGER,
                score INTEGER,
                userresult INTEGER,
                weight INTEGER,
                -- In-Game Statistics
                bkchance INTEGER,
                bkgoal INTEGER,
                blkshot INTEGER,
                faceoff INTEGER,
                hits INTEGER,
                passchance INTEGER,
                passcomp INTEGER,
                penmin INTEGER,
                ppo INTEGER,
                ppg INTEGER,
                pshchance INTEGER,
                pshgoal INTEGER,
                onetgoal INTEGER,
                onetchance INTEGER,
                shg INTEGER,
                shots INTEGER,
                toa INTEGER,
                -- Audit Field
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (game_id, user_id)
            );";

        using var cmd = new NpgsqlCommand(createTableQuery, conn);
        cmd.ExecuteNonQuery();
    }

    private void CreateSoReportTable()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS so_reports (
                -- Primary Keys / Identifiers (Assumed)
                game_id BIGINT NOT NULL,
                user_id BIGINT NOT NULL,
                -- Network and Bandwidth Stats
                bandavggm INTEGER,
                bandavgnet INTEGER,
                bandhigm INTEGER,
                bandhinet INTEGER,
                bandlowgm INTEGER,
                bandlownet INTEGER,
                bytesrcvdgm INTEGER,
                bytesrcvdnet INTEGER,
                bytessentgm INTEGER,
                bytessentnet INTEGER,
                droppkts INTEGER,
                lateavggm INTEGER,
                lateavgnet INTEGER,
                latehigm INTEGER,
                latehinet INTEGER,
                latelowgm INTEGER,
                latelownet INTEGER,
                latesdevgm INTEGER,
                latesdevnet INTEGER,
                pktloss INTEGER,
                -- Performance, Synchronization, and Session Stats
                fpsavg INTEGER,
                fpsdev INTEGER,
                fpshi INTEGER,
                fpslow INTEGER,
                gdesyncend INTEGER,
                gdesyncrsn INTEGER,
                gendphase INTEGER,
                gresult INTEGER,
                grpttype INTEGER,
                grptver VARCHAR,
                guests0 INTEGER,
                guests1 INTEGER,
                usersend0 INTEGER,
                usersend1 INTEGER,
                usersstrt0 INTEGER,
                usersstrt1 INTEGER,
                voipend0 INTEGER,
                voipend1 INTEGER,
                voipstrt0 INTEGER,
                voipstrt1 INTEGER,
                -- Player Metadata and Game Outcome
                gamertag VARCHAR,
                name VARCHAR,
                team INTEGER,
                team_name VARCHAR,
                home INTEGER,
                quit INTEGER,
                disc INTEGER,
                cheat INTEGER,
                score INTEGER,
                userresult INTEGER,
                weight INTEGER,
                shots INTEGER,
                -- Audit Field
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (game_id, user_id)
            );";

        using var cmd = new NpgsqlCommand(createTableQuery, conn);
        cmd.ExecuteNonQuery();
    }

    private void CreateOtpReportTable()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS otp_reports (
                    -- Primary Keys / Identifiers
                    game_id BIGINT NOT NULL,
                    user_id BIGINT NOT NULL,
                    -- Network/Bandwidth Stats
                    bandavggm INTEGER,
                    bandavgnet INTEGER,
                    bandhigm INTEGER,
                    bandhinet INTEGER,
                    bandlowgm INTEGER,
                    bandlownet INTEGER,
                    bytesrcvdgm INTEGER,
                    bytesrcvdnet INTEGER,
                    bytessentgm INTEGER,
                    bytessentnet INTEGER,
                    droppkts INTEGER,
                    lateavggm INTEGER,
                    lateavgnet INTEGER,
                    latehigm INTEGER,
                    latehinet INTEGER,
                    latelowgm INTEGER,
                    latelownet INTEGER,
                    latesdevgm INTEGER,
                    latesdevnet INTEGER,
                    pktloss INTEGER,
                    -- Performance/Session Stats
                    fpsavg INTEGER,
                    fpsdev INTEGER,
                    fpshi INTEGER,
                    fpslow INTEGER,
                    gdesyncend INTEGER,
                    gdesyncrsn INTEGER,
                    gendphase INTEGER,
                    gresult INTEGER,
                    grpttype INTEGER,
                    grptver VARCHAR,
                    guests0 INTEGER,
                    guests1 INTEGER,
                    usersend0 INTEGER,
                    usersend1 INTEGER,
                    usersstrt0 INTEGER,
                    usersstrt1 INTEGER,
                    voipend0 INTEGER,
                    voipend1 INTEGER,
                    voipstrt0 INTEGER,
                    voipstrt1 INTEGER,
                    -- Game & Player Metadata
                    gamertag VARCHAR,
                    name VARCHAR,
                    plycrfirst VARCHAR,
                    plycrlast VARCHAR,
                    plycrname VARCHAR,
                    team_name VARCHAR,
                    team INTEGER,
                    home INTEGER,
                    pos INTEGER,
                    quit INTEGER,
                    disc INTEGER,
                    cheat INTEGER,
                    score INTEGER,
                    userresult INTEGER,
                    -- Player In-Game Stats
                    lass INTEGER,
                    lblkshots INTEGER,
                    ldekemade INTEGER,
                    ldeketry INTEGER,
                    lfit INTEGER,
                    lfitwon INTEGER,
                    lfo INTEGER,
                    lfowon INTEGER,
                    lgdespsave INTEGER,
                    lgive INTEGER,
                    lgminplay INTEGER,
                    lgoal INTEGER,
                    lgpsave INTEGER,
                    lgpshot INTEGER,
                    lgrateo INTEGER,
                    lgratep INTEGER,
                    lgrates INTEGER,
                    lgratet INTEGER,
                    lgsa INTEGER,
                    lgsave INTEGER,
                    lgso INTEGER,
                    lgsosave INTEGER,
                    lgsoshot INTEGER,
                    lgwg INTEGER,
                    lgwin INTEGER,
                    lhits INTEGER,
                    loff INTEGER,
                    lpim INTEGER,
                    lplusmin INTEGER,
                    lpos INTEGER,
                    lppg INTEGER,
                    lscrchnce INTEGER,
                    lscrngoal INTEGER,
                    lshg INTEGER,
                    lshots INTEGER,
                    lsrateo INTEGER,
                    lsratep INTEGER,
                    lsrates INTEGER,
                    lsratet INTEGER,
                    lswin INTEGER,
                    ltake INTEGER,
                    -- Audit Field
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (game_id, user_id)
                );";

        using var cmd = new NpgsqlCommand(createTableQuery, conn);
        cmd.ExecuteNonQuery();
    }

    private void CreateHutReportTable()
    {
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        const string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS hut_reports (
                -- Primary Keys / Identifiers
                game_id BIGINT NOT NULL,
                user_id BIGINT NOT NULL,
                -- Network and Bandwidth Stats
                bandavggm INTEGER,
                bandavgnet INTEGER,
                bandhigm INTEGER,
                bandhinet INTEGER,
                bandlowgm INTEGER,
                bandlownet INTEGER,
                bytesrcvdgm INTEGER,
                bytesrcvdnet INTEGER,
                bytessentgm INTEGER,
                bytessentnet INTEGER,
                droppkts INTEGER,
                lateavggm INTEGER,
                lateavgnet INTEGER,
                latehigm INTEGER,
                latehinet INTEGER,
                latelowgm INTEGER,
                latelownet INTEGER,
                latesdevgm INTEGER,
                latesdevnet INTEGER,
                pktloss INTEGER,
                -- Performance, Synchronization, and Session Stats
                fpsavg INTEGER,
                fpsdev INTEGER,
                fpshi INTEGER,
                fpslow INTEGER,
                gdesyncend INTEGER,
                gdesyncrsn INTEGER,
                gendphase INTEGER,
                gresult INTEGER,
                grpttype INTEGER,
                grptver VARCHAR,
                guests0 INTEGER,
                guests1 INTEGER,
                usersend0 INTEGER,
                usersend1 INTEGER,
                usersstrt0 INTEGER,
                usersstrt1 INTEGER,
                voipend0 INTEGER,
                voipend1 INTEGER,
                voipstrt0 INTEGER,
                voipstrt1 INTEGER,
                -- Player Metadata and Game Outcome
                gamertag VARCHAR,
                name VARCHAR,
                team INTEGER,
                team_name VARCHAR,
                home INTEGER,
                quit INTEGER,
                disc INTEGER,
                cheat INTEGER,
                score INTEGER,
                userresult INTEGER,
                weight INTEGER,
                -- In-Game Statistics
                bkchance INTEGER,
                bkgoal INTEGER,
                blkshot INTEGER,
                faceoff INTEGER,
                hits INTEGER,
                passchance INTEGER,
                passcomp INTEGER,
                penmin INTEGER,
                ppo INTEGER,
                ppg INTEGER,
                pshchance INTEGER,
                pshgoal INTEGER,
                onetgoal INTEGER,
                onetchance INTEGER,
                shg INTEGER,
                shots INTEGER,
                toa INTEGER,
                -- Hut specific
                tropply1 INTEGER,
                tropply2 INTEGER,
                tropply3 INTEGER,
                -- Audit Field
                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (game_id, user_id)
            );";

        using var cmd = new NpgsqlCommand(createTableQuery, conn);
        cmd.ExecuteNonQuery();
    }

    public async Task InsertReport(GameReport report, long reporterUserId)
    {
        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        const string insertGameQuery = @"
            INSERT INTO games (
                game_id, fnsh, gtyp
            ) VALUES (
                @game_id, @fnsh, @gtyp
            )
            ON CONFLICT (game_id) DO NOTHING;";

        await using var cmd = new NpgsqlCommand(insertGameQuery, conn);
        cmd.Parameters.AddWithValue("game_id", (long)report.mGameReportingId);
        cmd.Parameters.AddWithValue("fnsh", report.mFinished);
        cmd.Parameters.AddWithValue("gtyp", (long)report.mGameTypeId);
        cmd.Parameters.AddWithValue("prcs", report.mProcess);
        await cmd.ExecuteNonQueryAsync();

        var gameAttributeMap = report.mAttributeMap;
        foreach (var key in gameAttributeMap.Keys)
        {
            var column = key.ToLower();
            var insertGameAttributeQuery = $@"
                INSERT INTO games (game_id, {column})
                    VALUES (@game_id, @value)
                ON CONFLICT (game_id) DO UPDATE
                    SET {column} = EXCLUDED.{column};";

            await using var cmd1 = new NpgsqlCommand(insertGameAttributeQuery, conn);
            cmd1.Parameters.AddWithValue("game_id", (long)report.mGameReportingId);

            if (int.TryParse(gameAttributeMap[key], out var intValue))
                cmd1.Parameters.AddWithValue("value", intValue);
            else
                cmd1.Parameters.AddWithValue("value", gameAttributeMap[key]);
            await cmd1.ExecuteNonQueryAsync();
        }

        var tableName = "reports";
        switch (report.mGameTypeId)
        {
            case 1:
                tableName = "reports";
                break;
            case 2:
                tableName = "so_reports";
                break;
            case 3:
                tableName = "otp_reports";
                break;
            case 6:
                tableName = "hut_reports";
                break;
        }

        var mPlayerReportMap = report.mPlayerReportMap;
        foreach (var userId in mPlayerReportMap.Keys)
        {
            var insertPlayerQuery = $@"
                INSERT INTO {tableName} ( 
                    game_id, user_id
                ) VALUES (
                    @game_id, @user_id
                )
                ON CONFLICT (game_id, user_id) DO NOTHING;";

            await using var cmd1 = new NpgsqlCommand(insertPlayerQuery, conn);
            cmd1.Parameters.AddWithValue("game_id", (long)report.mGameReportingId);
            cmd1.Parameters.AddWithValue("user_id", userId);
            await cmd1.ExecuteNonQueryAsync();
        }

        var playerAttributeMap = mPlayerReportMap[reporterUserId].mAttributeMap;
        foreach (var key in playerAttributeMap.Keys)
        {
            var column = key.ToLower();
            var insertPlayerAttributeQuery = $@"
                    INSERT INTO {tableName} (game_id, user_id, {column})
                        VALUES (@game_id, @user_id, @value)
                    ON CONFLICT (game_id, user_id) DO UPDATE
                        SET {column} = EXCLUDED.{column};";

            await using var cmd1 = new NpgsqlCommand(insertPlayerAttributeQuery, conn);
            cmd1.Parameters.AddWithValue("game_id", (long)report.mGameReportingId);
            cmd1.Parameters.AddWithValue("user_id", reporterUserId);

            if (int.TryParse(playerAttributeMap[key], out var intValue))
                cmd1.Parameters.AddWithValue("value", intValue);
            else
                cmd1.Parameters.AddWithValue("value", playerAttributeMap[key]);
            await cmd1.ExecuteNonQueryAsync();
        }
    }

    public List<uint> GetListDbIds(CardSubType cardSubType)
    {
        var ids = new List<uint>();
        if (cardSubType > CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK) return ids;

        using var conn = new NpgsqlConnection(ConnectionString);
        conn.OpenAsync();

        string sql = "SELECT carddbid FROM fcc_playercards WHERE preferredposition = @pos";

        using (var cmd = new NpgsqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("pos", (short)cardSubType);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ids.Add((uint)reader.GetInt32(0));
                }
            }
        }

        return ids;
    }

    public async Task<CardData> GetPlayerCardDataByDbId(uint cardDbId)
    {
        const string sql = "SELECT * FROM fcc_playercards WHERE carddbid = @dbid LIMIT 1";

        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("dbid", (int)cardDbId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            short rating = reader.GetInt16(reader.GetOrdinal("rating"));
            return new CardData
            {
                mAttributes = new List<byte>
                {
                    reader.GetByte(reader.GetOrdinal("attribute1")),
                    reader.GetByte(reader.GetOrdinal("attribute2")),
                    reader.GetByte(reader.GetOrdinal("attribute3")),
                    reader.GetByte(reader.GetOrdinal("attribute4")),
                    reader.GetByte(reader.GetOrdinal("attribute5")),
                    reader.GetByte(reader.GetOrdinal("attribute6")),
                    reader.GetByte(reader.GetOrdinal("attribute7")),
                    reader.GetByte(reader.GetOrdinal("attribute8")),
                },
                mCardStateId = CardState.CARDHOUSE_CARDSTATE_FREE,
                mCardDbId = cardDbId,
                mFormationId = reader.GetByte(reader.GetOrdinal("formationid")),
                // mFREE = 40, //
                mCareerRemaining = 50, //
                mInjuryGames = reader.GetByte(reader.GetOrdinal("injuryduration")),
                mInjuryType = reader.GetByte(reader.GetOrdinal("injury")),
                mMaxTrainingCardsCanApply = HutHelper.DetermineTrainingCardsCanApply(rating),
                // mMaxTrainingCardsCanApply = 2,
                // mNumberOfOwners = 86, //
                mPreferredPositionId = reader.GetByte(reader.GetOrdinal("preferredposition")),
                mDiscardPrice = 100, //
                mRareFlag = reader.GetByte(reader.GetOrdinal("rare")),
                mRating = (byte)rating,
                mSalaryCap = HutHelper.DetermineSalary(rating), //
                mListStats = new List<int>
                {
                    reader.GetByte(reader.GetOrdinal("stat1")), //Games Played
                    reader.GetByte(reader.GetOrdinal("stat2")), //Goals 
                    reader.GetByte(reader.GetOrdinal("stat3")), //Assists 
                    reader.GetByte(reader.GetOrdinal("stat4")), //Plus/Minus
                    reader.GetByte(reader.GetOrdinal("stat5")), //Penalty Minutes
                },
                mCardSubTypeId = (CardSubType)reader.GetInt16(reader.GetOrdinal("fieldpos")),
                mDateIssued = Util.TimeNow(),
                mTeamId = (uint)reader.GetInt32(reader.GetOrdinal("teamid")),
                mListTrainingCards = new List<int>()
                {
                    // 0,0,0,0,0,0,0,0,0,0,
                    // 0,0
                },
                mUsesRemaining = 20
            };
        }

        return new CardData();
    }
    
    public static async Task<HutTrainingCard> GetTrainingCardByDbIdAsync(uint cardDbId)
    {
        const string sql = "SELECT * FROM fcc_trainingcards WHERE carddbid = @cardDbId";

        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        await using var command = new NpgsqlCommand(sql, conn);
        command.Parameters.AddWithValue("cardDbId", (int)cardDbId);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new HutTrainingCard
            {
                CardDbId = (uint)reader.GetInt32(0),
                CardSubtype = reader.GetInt32(1),
                WeightRare = reader.GetInt32(2),
                CardAssetId = reader.GetInt32(3),
                Description = reader.GetString(4),
                Amount = reader.GetInt32(5),
                Rating = reader.GetInt32(6),
                AttributeSlot = reader.GetInt32(7),
                IndexedConsumableId = reader.GetInt32(8)
            };
        }

        return null;
    }
    
    public static async Task<HutContractCard> GetContractCardByDbIdAsync(uint cardDbId)
    {
        const string sql = "SELECT * FROM fcc_contractcards WHERE carddbid = @cardDbId";

        await using var conn = new NpgsqlConnection(ConnectionString);
        await conn.OpenAsync();

        await using var command = new NpgsqlCommand(sql, conn);
        command.Parameters.AddWithValue("cardDbId", (int)cardDbId);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new HutContractCard
            {
                CardDbId = (uint)reader.GetInt32(0),
                WeightRare = reader.GetInt32(1),
                Value = reader.GetInt32(2),
            };
        }

        return null;
    }

    public uint GetNextGameId()
    {
        if (!isEnabled) return fallbackGameIdCounter++;
        using var conn = new NpgsqlConnection(ConnectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT nextval('zamboni_game_id_seq');", conn);
        var result = cmd.ExecuteScalar() ?? throw new InvalidOperationException("Failed to get next game ID.");
        var nextId = (long)result;
        if (nextId > uint.MaxValue) throw new OverflowException("Over 4 billion games played, what we do now?");
        return (uint)nextId;
    }
}