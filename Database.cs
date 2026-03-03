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
    private readonly string connectionString = Program.ZamboniConfig.DatabaseConnectionString;
    public readonly bool isEnabled;

    private uint fallbackGameIdCounter = 1;

    public Database()
    {
        try
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            isEnabled = true;
            Logger.Warn("Database is accessible.");
        }
        catch (Exception)
        {
            isEnabled = false;
            Logger.Warn("Database is not accessible. Gamedata wont be saved");
            return;
        }

        CreateGameIdSequence();
        CreateGamesTable();
        CreateReportTable();
        CreateOtpReportTable();
        CreateSoReportTable();
    }

    private void CreateGameIdSequence()
    {
        using var conn = new NpgsqlConnection(connectionString);
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
        using var conn = new NpgsqlConnection(connectionString);
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
        using var conn = new NpgsqlConnection(connectionString);
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
        using var conn = new NpgsqlConnection(connectionString);
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
        using var conn = new NpgsqlConnection(connectionString);
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

    public void InsertReport(GameReport report, long reporterUserId)
    {
        using var conn = new NpgsqlConnection(connectionString);
        conn.Open();

        const string insertGameQuery = @"
            INSERT INTO games (
                game_id, fnsh, gtyp
            ) VALUES (
                @game_id, @fnsh, @gtyp
            )
            ON CONFLICT (game_id) DO NOTHING;";

        using var cmd = new NpgsqlCommand(insertGameQuery, conn);
        cmd.Parameters.AddWithValue("game_id", (long)report.mGameReportingId);
        cmd.Parameters.AddWithValue("fnsh", report.mFinished);
        cmd.Parameters.AddWithValue("gtyp", (long)report.mGameTypeId);
        cmd.Parameters.AddWithValue("prcs", report.mProcess);
        cmd.ExecuteNonQuery();

        var gameAttributeMap = report.mAttributeMap;
        foreach (var key in gameAttributeMap.Keys)
        {
            var column = key.ToLower();
            var insertGameAttributeQuery = $@"
                INSERT INTO games (game_id, {column})
                    VALUES (@game_id, @value)
                ON CONFLICT (game_id) DO UPDATE
                    SET {column} = EXCLUDED.{column};";

            using var cmd1 = new NpgsqlCommand(insertGameAttributeQuery, conn);
            cmd1.Parameters.AddWithValue("game_id", (long)report.mGameReportingId);

            if (int.TryParse(gameAttributeMap[key], out var intValue))
                cmd1.Parameters.AddWithValue("value", intValue);
            else
                cmd1.Parameters.AddWithValue("value", gameAttributeMap[key]);
            cmd1.ExecuteNonQuery();
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

            using var cmd1 = new NpgsqlCommand(insertPlayerQuery, conn);
            cmd1.Parameters.AddWithValue("game_id", (long)report.mGameReportingId);
            cmd1.Parameters.AddWithValue("user_id", userId);
            cmd1.ExecuteNonQuery();
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

            using var cmd1 = new NpgsqlCommand(insertPlayerAttributeQuery, conn);
            cmd1.Parameters.AddWithValue("game_id", (long)report.mGameReportingId);
            cmd1.Parameters.AddWithValue("user_id", reporterUserId);

            if (int.TryParse(playerAttributeMap[key], out var intValue))
                cmd1.Parameters.AddWithValue("value", intValue);
            else
                cmd1.Parameters.AddWithValue("value", playerAttributeMap[key]);
            cmd1.ExecuteNonQuery();
        }
    }

    public async Task<List<uint>> GetListDbIds(CardSubType cardSubType)
    {
        var ids = new List<uint>();
        if (cardSubType > CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK) return ids;

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        string sql = "SELECT carddbid FROM fcc_playercards WHERE preferredposition = @pos";

        await using (var cmd = new NpgsqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("pos", (short)cardSubType);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    ids.Add((uint)reader.GetInt32(0));
                }
            }
        }

        return ids;
    }

    private static int bugGcounter = 0;

    public async Task<CardData?> GetCardDataByDbId(uint cardDbId)
    {
        const string sql = "SELECT * FROM fcc_playercards WHERE carddbid = @dbid LIMIT 1";

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("dbid", (int)cardDbId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
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
                mCardStateId = 1,
                mCardId = HutCardFactory.CardIdCounter++,
                mCardDbId = cardDbId,
                mFormationId = reader.GetByte(reader.GetOrdinal("formationid")),
                // mFREE = 40, //
                mCareerRemaining = 50, //
                mInjuryGames = reader.GetByte(reader.GetOrdinal("injuryduration")),
                mInjuryType = reader.GetByte(reader.GetOrdinal("injury")),
                mMaxTrainingCardsCanApply = 10, //
                // mNumberOfOwners = 86, //
                mPreferredPositionId = reader.GetByte(reader.GetOrdinal("preferredposition")),
                mDiscardPrice = 85, //
                mRareFlag = reader.GetByte(reader.GetOrdinal("rare")),
                mRating = reader.GetByte(reader.GetOrdinal("rating")),
                mSalaryCap = 84, //
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
                mListTrainingCards = new List<int>
                {
                    //TODO Figure this out
                    0,0,0,0,0,0,0,0,0,0
                },
                mUsesRemaining = 10
            };
        }

        return null;
    }

    public uint GetNextGameId()
    {
        if (!isEnabled) return fallbackGameIdCounter++;
        using var conn = new NpgsqlConnection(connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT nextval('zamboni_game_id_seq');", conn);
        var result = cmd.ExecuteScalar() ?? throw new InvalidOperationException("Failed to get next game ID.");
        var nextId = (long)result;
        if (nextId > uint.MaxValue) throw new OverflowException("Over 4 billion games played, what we do now?");
        return (uint)nextId;
    }
}