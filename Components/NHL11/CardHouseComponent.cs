using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blaze3SDK;
using BlazeCommon;
using Npgsql;
using Zamboni11.Components.NHL11.Bases;
using Zamboni11.Components.NHL11.Requests;
using Zamboni11.Components.NHL11.Responses;
using Zamboni11.Components.NHL11.Structs;
using GetConfigResponse = Zamboni11.Components.NHL11.Responses.GetConfigResponse;

namespace Zamboni11.Components.NHL11;

internal class CardHouseComponent : CardHouseComponentBase.Server
{
    public override async Task<LoginResponse> LoginRequestAsync(LoginRequest request, BlazeRpcContext context)
    {
        long userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        var gamerInfo = await GetGamerInfo(userId);
        if (gamerInfo == null) return new LoginResponse();
        return new LoginResponse
        {
            mTeamAbbreviation = gamerInfo.Value.mTeamAbbreviation,
            mBonusAwarded = 1, //TODO UNKNOWN
            mTeamName = gamerInfo.Value.mTeamName,
            mRewardType = 0, //TODO UNKNOWN
            mRewardValue = 10, //TODO UNKNOWN
            mUserId = 0 //TODO USE 0 FOR NOW FOR EVERYONE BECAUSE CLIENT SEEMS TO NOT "KNOW" HIS UID
        };
    }

    private async Task<GamerInfo?> GetGamerInfo(long userId)
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

    private async Task<GeneralInfo> GetOrCreateGeneralInfo(long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
        WITH inserted AS (
            INSERT INTO hut_general_info (user_id, pucks, stats)
            VALUES (@uid, 1000, '{}')
            ON CONFLICT (user_id) DO NOTHING
            RETURNING *
        )
        SELECT * FROM inserted
        UNION ALL
        SELECT * FROM hut_general_info WHERE user_id = @uid
        LIMIT 1;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("uid", userId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new GeneralInfo
            {
                mCredits = (uint)reader.GetInt32(reader.GetOrdinal("pucks")),
                mStats = reader.GetFieldValue<short[]>(reader.GetOrdinal("stats")).Select(s => (byte)s).ToList()
            };
        }
        throw new Exception();
    }

    private async Task<SquadInfo?> GetSquadInfo(long userId)
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
            foreach (var VARIABLE in reader.GetFieldValue<List<long>>(reader.GetOrdinal("players")))
            {
                playersOrdered.Add(await GetCard(VARIABLE, userId));
            }

            return new SquadInfo
            {
                mChemistry = (uint)reader.GetInt32(reader.GetOrdinal("chemistry")),
                // mCHNG = ,
                mFormationId = (uint)reader.GetInt32(reader.GetOrdinal("formation_id")),
                mLines = reader.GetFieldValue<int[]>(reader.GetOrdinal("lines")).ToList(),
                mManager = await GetCard(reader.GetInt64(reader.GetOrdinal("manager"))),
                mSquadName = reader.GetString(reader.GetOrdinal("squad_name")),
                mPlayers = playersOrdered,
                mStarRating = (uint)reader.GetInt32(reader.GetOrdinal("star_rating")),
                mSquadId = (uint)reader.GetInt32(reader.GetOrdinal("squad_id"))
            };
        }
        throw new Exception();
    }

    private async Task<VersionInfo> GetOrCreateVersionInfo(long userId)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = @"
        WITH existing_or_new AS (
            INSERT INTO hut_version_info (user_id, escrow_version, general_version, unassigned_version)
            VALUES (@uid, 1, 1, 1)
            ON CONFLICT (user_id) DO NOTHING
            RETURNING *
        )
        SELECT escrow_version, general_version, unassigned_version FROM existing_or_new
        UNION ALL
        SELECT escrow_version, general_version, unassigned_version 
        FROM hut_version_info 
        WHERE user_id = @uid
        LIMIT 1;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("uid", userId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new VersionInfo
            {
                mVersionEscrow = (uint)reader.GetInt32(0),
                mVersionGeneral = (uint)reader.GetInt32(1),
                mVersionUnassigned = (uint)reader.GetInt32(2)
            };
        }

        throw new Exception();
    }

    private async Task SetGamerInfo(GamerInfo gamerInfo, long userId)
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
            @uid, @ct, @tf, @kt, @li, 
            @lu, @tn, @pq, @pw, 
            @qt, @sp, @ta, 
            @to, @tp, @tr
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

        cmd.Parameters.AddWithValue("uid", userId);
        cmd.Parameters.AddWithValue("ct", gamerInfo.mCustomTactics);
        cmd.Parameters.AddWithValue("tf", (int)gamerInfo.mTeamFormation);
        cmd.Parameters.AddWithValue("kt", gamerInfo.mKickTakers);
        cmd.Parameters.AddWithValue("li", gamerInfo.mLineup);
        cmd.Parameters.AddWithValue("lu", gamerInfo.mLogoUrl);
        cmd.Parameters.AddWithValue("tn", gamerInfo.mTeamName ?? "");
        cmd.Parameters.AddWithValue("pq", (int)gamerInfo.mPlayoffsQualified);
        cmd.Parameters.AddWithValue("pw", (int)gamerInfo.mPlayoffWon);
        cmd.Parameters.AddWithValue("qt", gamerInfo.mQuickTactics);
        cmd.Parameters.AddWithValue("sp", (int)gamerInfo.mSpecialPacksBought);
        cmd.Parameters.AddWithValue("ta", gamerInfo.mTeamAbbreviation);
        cmd.Parameters.AddWithValue("to", gamerInfo.mTournaments);
        cmd.Parameters.AddWithValue("tp", (int)gamerInfo.mTPPL);
        cmd.Parameters.AddWithValue("tr", gamerInfo.mTrophies);

        await cmd.ExecuteNonQueryAsync();
    }

    private async Task SetSquadInfo(SquadSaveRequest request, long userId)
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

    private async Task SetGeneralInfo(GeneralInfo generalInfo, long userId)
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
        cmd.Parameters.AddWithValue("pucks", (int)generalInfo.mCredits);
        cmd.Parameters.AddWithValue("stats", generalInfo.mStats);

        await cmd.ExecuteNonQueryAsync();
    }

    public enum VersionType
    {
        Escrow,
        General,
        Unassigned
    }

    public async Task<VersionInfo> IncrementVersionInfo(long userId, VersionType type)
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

    public override Task<NumericResponse> LogoutRequestAsync(LogoutRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new NumericResponse
        {
            mNumber = 0,
        });
    }

    public override async Task<MoveCardResponse> MoveCardAsync(MoveCardRequest request, BlazeRpcContext context)
    {
        long userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        // HutPlayerInstance playerInstance = HutManager.GetHutPlayerInstance(context.BlazeConnection);
        // if (request.mDeckType == DeckType.CARDHOUSE_DECK_ESCROW)
        // {
        //     CardData cardData = HutManager.UserInventories[userId][request.mCardId];
        //     // cardData.mCardStateId = CardState.CARDHOUSE_CARDSTATE_INCARDSELL;
        //     HutManager.UserInventories[userId].Remove(request.mCardId);
        //     HutManager.Escrow.TryAdd(userId, new Dictionary<long, CardData>());
        //     HutManager.Escrow[userId].Add(request.mCardId, cardData);
        // }
        CardData cardData = await GetCard(request.mCardId);
        var versionInfo = await GetOrCreateVersionInfo(userId);
        switch (request.mDeckType)
        {
            case DeckType.CARDHOUSE_DECK_ESCROW:
                await HutCardFactory.CreateOrUpdateCard(cardData, userId, CardLocation.ESCROW);
                versionInfo = await IncrementVersionInfo(userId, VersionType.Escrow);
                break;
            default:
                throw new NotImplementedException();
        }

        return new MoveCardResponse
        {
            mDisplacedCardId = request.mCardId,
            mDisplacedDeckType = request.mDeckType,
            mDisplacedCardPosition = 0,
            mVersionInfo = versionInfo
        };
    }

    public override async Task<GamerGetInfoResponse> GetGamerInfoRequestAsync(GamerGetInfoRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        var gamerInfo = await GetGamerInfo(userId);
        if (gamerInfo == null) throw new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_NO_PLAYER_INFO);
        return new GamerGetInfoResponse
        {
            mGamerInfo = gamerInfo.Value,
            mUserId = 0 //TODO USE 0 FOR NOW FOR EVERYONE BECAUSE CLIENT SEEMS TO NOT "KNOW" ITS UID
        };
    }

    public override async Task<NumericResponse> SetGamerInfoRequestAsync(GamerSetInfoRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        await SetGamerInfo(request.mGamerInfo, userId);
        return new NumericResponse
        {
            mNumber = 0
        };
    }

    public async Task<List<CardData>> GetCardList(long userId, CardLocation cardLocation)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        const string sql = "SELECT * FROM hut_cards WHERE user_id = @user_id AND card_location = @card_location;";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("card_location", (int)cardLocation);

        await using var reader = await cmd.ExecuteReaderAsync();

        List<CardData> cardDataList = new List<CardData>();

        while (await reader.ReadAsync())
        {
            cardDataList.Add(ReadCardData(reader));
        }

        return cardDataList;
    }

    public async Task<List<CardData>> GetCardList(long userId, StickerBookSearchRequest request)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        var sql = new StringBuilder(@"
        SELECT * 
              FROM hut_cards
        WHERE user_id = @user_id");

        sql.Append(" AND card_location IN (0, 1, 3, 4)");
        switch (request.mCollectionSearchCardType)
        {
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_ALL:
            {
                break;
            }
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_HEADCOACH:
            {
                sql.Append(" AND sub_type = 6");
                break;
            }
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_BADGE:
            {
                sql.Append(" AND sub_type = 12");
                break;
            }
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_STADIUM:
            {
                sql.Append(" AND sub_type = 11");
                break;
            }
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_PLAYER_ALL:
            {
                sql.Append(" AND sub_type BETWEEN 0 AND 4");
                break;
            }
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_PLAYER_C:
            {
                sql.Append(" AND sub_type = 0");
                break;
            }
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_DEVELOPMENT:
            {
                sql.Append(" AND sub_type BETWEEN 51 AND 62");
                break;
            }
            default:
                throw new NotImplementedException();
        }

        if (request.mLeagueId != 255)
        {
            var range = HutCardFactory.LeagueTeamsMapping[request.mLeagueId];
            sql.Append(" AND team_id BETWEEN " + range.Start.Value + " AND " + range.End.Value + "");
        }

        if (request.mTeamId != 255)
        {
            sql.Append(" AND team_id =" + request.mTeamId + "");
        }

        await using var cmd = new NpgsqlCommand(sql.ToString(), conn);
        cmd.Parameters.AddWithValue("user_id", userId);

        await using var reader = await cmd.ExecuteReaderAsync();

        List<CardData> cardDataList = new List<CardData>();

        while (await reader.ReadAsync())
        {
            cardDataList.Add(ReadCardData(reader));
        }

        return cardDataList;
    }

    public static async Task<CardData> GetCard(long cardId, long userId = 0)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        var sql = new StringBuilder(@"
        SELECT * 
              FROM hut_cards
        WHERE 1=1");

        sql.Append(" AND card_id = @card_id");
        if (userId != 0)
        {
            sql.Append(" AND user_id = @user_id");
        }

        await using var cmd = new NpgsqlCommand(sql.ToString(), conn);
        cmd.Parameters.AddWithValue("card_id", cardId);
        if (userId != 0) cmd.Parameters.AddWithValue("user_id", userId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return ReadCardData(reader);
        }

        return new CardData();
    }

    private static CardData ReadCardData(NpgsqlDataReader reader)
    {
        CardData cardData = new CardData
        {
            mAttributes = reader.GetFieldValue<byte[]>(reader.GetOrdinal("attributes")).ToList(),
            mCardStateId = (CardState)(byte)reader.GetInt16(reader.GetOrdinal("state_id")),
            mCardId = reader.GetInt64(reader.GetOrdinal("card_id")),
            mCardDbId = (uint)reader.GetInt32(reader.GetOrdinal("db_id")),
            mFormationId = (byte)reader.GetInt16(reader.GetOrdinal("formation_id")),
            mFREE = (byte)reader.GetInt16(reader.GetOrdinal("free")),
            mCareerRemaining = (byte)reader.GetInt16(reader.GetOrdinal("career_remaining")),
            mInjuryGames = (byte)reader.GetInt16(reader.GetOrdinal("injury_games")),
            mInjuryType = (byte)reader.GetInt16(reader.GetOrdinal("injury_type")),
            mMaxTrainingCardsCanApply = (byte)reader.GetInt16(reader.GetOrdinal("morale")),
            // mNumberOfOwners = (byte)reader.GetInt16(reader.GetOrdinal("free")), ///TODO
            mPreferredPositionId = (byte)reader.GetInt16(reader.GetOrdinal("preferred_position_id")),
            mDiscardPrice = (byte)reader.GetInt16(reader.GetOrdinal("discard_price")),
            mRareFlag = (byte)reader.GetInt16(reader.GetOrdinal("rare_flag")),
            mRating = (byte)reader.GetInt16(reader.GetOrdinal("rating")),
            mSalaryCap = reader.GetInt16(reader.GetOrdinal("salary_cap")),
            mListStats = reader.GetFieldValue<int[]>(reader.GetOrdinal("list_stats")).ToList(),
            mCardSubTypeId = (CardSubType)reader.GetInt16(reader.GetOrdinal("sub_type")),
            mDateIssued = (uint)reader.GetInt64(reader.GetOrdinal("date_issued")),
            mTeamId = (uint)reader.GetInt32(reader.GetOrdinal("team_id")),
            mListTrainingCards = reader.GetFieldValue<int[]>(reader.GetOrdinal("list_training_cards")).ToList(),
            mUsesRemaining = (byte)reader.GetInt16(reader.GetOrdinal("uses_remaining"))
        };
        return cardData;
    }

    public override async Task<DeckInfoResponse> GetDeckInfoAsync(DeckInfoRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        var generalInfo = await GetOrCreateGeneralInfo(userId);
        var squadInfo = await GetSquadInfo(userId);
        uint teamRating = 0;
        if (squadInfo != null) teamRating = squadInfo.Value.mStarRating;
        var versionInfo = await GetOrCreateVersionInfo(userId);

        var escrowList = await GetCardList(userId, CardLocation.ESCROW);
        var unassignedList = await GetCardList(userId, CardLocation.UNASSIGNED);

        return new DeckInfoResponse
        {
            mDuplicateEscrowCardIdPairList = new List<CardIdPair>(),
            mDuplicateUnassignedCardIdPairList = new List<CardIdPair>(),
            mEscrowCardDataList = escrowList,
            mEscrowCount = (byte)escrowList.Count,
            mGeneralInfo = generalInfo,
            mTeamRating = teamRating,
            mUnassignedCardDataList = unassignedList,
            mUserId = 0,
            mVersionInfo = versionInfo
        };
        // HutPlayerInstance hutPlayerInstance = HutManager.GetHutPlayerInstance(context.BlazeConnection);
        // var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        // List<CardData> escrowList = new List<CardData>();
        // byte ecnt = 0;
        // if (HutManager.Escrow.ContainsKey(userId))
        // {
        //     foreach (var VARIABLE in HutManager.Escrow[userId])
        //     {
        //         escrowList.Add(VARIABLE.Value);
        //         ecnt++;
        //     }
        // }
        //
        // List<CardData> unAssigned = new List<CardData>();
        // if (HutManager.UserUnAssigned.ContainsKey(userId))
        // {
        //     foreach (var VARIABLE in HutManager.UserUnAssigned[userId])
        //     {
        //         unAssigned.Add(VARIABLE.Value);
        //     }
        // }
        //
        // return Task.FromResult(new DeckInfoResponse
        // {
        //     mDuplicateEscrowCardIdPairList = new List<CardIdPair>
        //     {
        //     },
        //     mDuplicateUnassignedCardIdPairList = new List<CardIdPair>
        //     {
        //     },
        //     mEscrowCardDataList = escrowList,
        //     mEscrowCount = ecnt,
        //     mGeneralInfo = new GeneralInfo
        //     {
        //         mCredits = hutPlayerInstance.pucks, //TODO This is EA pucks
        //         mStats = new List<byte>
        //         {
        //             6, 7, 10, 30, 50, 60, 80
        //         }
        //     },
        //     mTeamRating = hutPlayerInstance.SquadInfo.mStarRating,
        //     mUnassignedCardDataList = unAssigned,
        //     mUserId = 0,
        //     mVersionInfo = HutManager.GetHutPlayerInstance(context.BlazeConnection).GetVersionInfo()
        // });
    }

    public override Task<GetConfigResponse> GetConfigRequestAsync(ProvidedUID request, BlazeRpcContext context)
    {
        return Task.FromResult(new GetConfigResponse
        {
            mConfigList = new List<uint>
            {
                10, 20, 30, 40, 50, 60, 70, 80, 90, 100 //tf is this?
            }
        });
    }

    //
    public override async Task<DiscardCardResponse> DiscardCardAsync(DiscardCardRequest request, BlazeRpcContext context)
    {
        //TODO Maybe checks
        long userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        CardData cardData = await GetCard(request.mCardId);
        await HutCardFactory.CreateOrUpdateCard(cardData, userId, CardLocation.DISCARDED);
        var generalInfo = await GetOrCreateGeneralInfo(userId);
        await SetGeneralInfo(new GeneralInfo
        {
            mCredits = request.mCredits + generalInfo.mCredits,
            mStats = generalInfo.mStats
        }, userId);
        VersionInfo versionInfo = await IncrementVersionInfo(userId, VersionType.General);
        // HutPlayerInstance player = HutManager.GetHutPlayerInstance(context.BlazeConnection);
        // HutManager.UserInventories[userId][request.mCardId] = new CardData();
        // bool activeRemoved = HutManager.UserInventories[userId].Remove(request.mCardId);
        // if (!activeRemoved) HutManager.Escrow[userId].Remove(request.mCardId);

        // player.pucks += request.mCredits;
        return new DiscardCardResponse
        {
            mCredits = request.mCredits,
            mVersionInfo = versionInfo
        };
    }

    //
    public override Task<StaffBonusResponse> GetStaffBonusAsync(ProvidedUID request, BlazeRpcContext context)
    {
        return Task.FromResult(new StaffBonusResponse
        {
            mStaffBonusInfo = new StaffBonusInfo
            {
                mPhysioArmBonus = 0,
                mPhysioBackBonus = 0,
                mContractBonus = 0,
                mFitnessBonus = 0,
                mPhysioFootBonus = 0,
                mGKDivingBonus = 0,
                mGKHandlingBonus = 0,
                mGKKickingBonus = 0,
                mGKOneOnOneBonus = 0,
                mGKPositioningBonus = 0,
                mGKReflexesBonus = 0,
                mPhysioHeadBonus = 0,
                mPhysioHipBonus = 0,
                mPhysioLegBonus = 0,
                mDefendingBonus = 0,
                mDribblingBonus = 0,
                mHeadingBonus = 0,
                mPaceBonus = 0,
                mPassingBonus = 0,
                mShootingBonus = 0,
                mPhysioShoulderBonus = 0,
                mManagerTalkBonus = 0
            }
        });
    }

    public override async Task<AssignCardsResponse> AssignCardsAsync(AssignCardsRequest request, BlazeRpcContext context)
    {
        long userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        await IncrementVersionInfo(userId, VersionType.Unassigned);
        return new AssignCardsResponse
        {
            mVersionInfo = await GetOrCreateVersionInfo(userId)
        };
    }


    public override Task<UserReliabilityInfoResponse> GetUserReliabilityInfoAsync(ProvidedUID request, BlazeRpcContext context)
    {
        return Task.FromResult(new UserReliabilityInfoResponse
        {
            mPreviousMatchUnfinished = 0,
            mMatchesFinished = 10,
            mMatchesStarted = 10,
            mReliability = 0,
            mUserId = 0
        });
    }

    public override Task<NumericResponse> ResetUserRequestAsync(ProvidedUID request, BlazeRpcContext context)
    {
        throw new NotImplementedException();
        var userId = ServerManager.GetServerPlayer(context.Connection).UserIdentification.mAccountId;
        // HutManager.Escrow.TryRemove(userId, out _);
        // HutManager.HutPlayerInstances.TryRemove(userId, out _);
        // HutManager.UserInventories.TryRemove(userId, out _);
        // HutManager.UserUnAssigned.TryRemove(userId, out _);
        return Task.FromResult(new NumericResponse
        {
            mNumber = 0,
        });
    }

    public override Task<SquadListResponse> GetSquadListAsync(ProvidedUID request, BlazeRpcContext context)
    {
        return Task.FromResult(new SquadListResponse
        {
            mActiveSquad = 1,
            mSquads = new List<SquadSmall>
            {
                new SquadSmall
                {
                    mChemistry = 1,
                    mFormation = 1,
                    mRating = 1,
                    mSquadId = 0,
                    mSquadName = "aaaaa"
                }
            }
        });
    }

    public override async Task<ViewCardsResponse> ViewCardsAsync(ViewCardsRequest request, BlazeRpcContext context)
    {
        //TODO LIMITED TO USERS OWN CARDS
        long userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        List<CardData> retList = new List<CardData>();
        foreach (var VARIABLE in request.mCardIdList)
        {
            retList.Add(await GetCard(VARIABLE));
        }
        // foreach (var VARIABLE in HutManager.UserInventories[userId].Values.ToList())
        // {
        //     if (request.mCardIdList.Contains(VARIABLE.mCardId))
        //     {
        //         retList.Add(VARIABLE);
        //     }
        // }

        return new ViewCardsResponse
        {
            mCardDataList = retList
        };
    }

    public override async Task<SquadSaveResponse> SquadSaveAsync(SquadSaveRequest request, BlazeRpcContext context)
    {
        long userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        // var hutPlayerInstance = HutManager.GetHutPlayerInstance(context.BlazeConnection);
        // if (hutPlayerInstance == null) throw new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_NO_PLAYER_INFO);
        await SetSquadInfo(request, userId);
        // List<CardData> retList = new();
        // foreach (var VARIABLE in request.mPlayers)
        // {
        //     if (VARIABLE == 0)
        //     {
        //         retList.Add(new CardData());
        //         continue;
        //     }
        //
        //     retList.Add(HutManager.UserInventories[userId][VARIABLE]);
        // }
        //
        // hutPlayerInstance.SquadInfo = new SquadInfo
        // {
        //     mChemistry = request.mChemistry,
        //     mFormationId = request.mFormation,
        //     mLines = request.mLines,
        //     mManager = HutManager.GetCard(userId, request.mManager),
        //     mSquadName = request.mSquadName,
        //     mPlayers = retList,
        //     mStarRating = request.mStarRating,
        //     mSquadId = request.mSquadId
        // };
        return new SquadSaveResponse
        {
            mSquadId = request.mSquadId
        };
    }

    public override async Task<StickerBookStats2Response> StickerBookStats2Async(StickerBookStats2Request request, BlazeRpcContext context)
    {
        long userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;

        List<StickerBookStatResult> stats = new();

        var playerTypes = new[]
        {
            CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C,
            CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW,
            CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW,
            CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D,
            CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK
        };

        if (request.mContextId == RequestContext.CARDHOUSE_STICKERBOOK_STATS_REQUEST_CONTEXT_TOP)
        {
            stats.Add(new StickerBookStatResult
            {
                mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                mContextValue = 2,
                mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_PLAYERS,
                mValue = await GetCardCountAsync(userId, playerTypes)
            });
            stats.Add(new StickerBookStatResult
            {
                mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                mContextValue = 2,
                mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_STAFF_HEADCOACH,
                mValue = await GetCardCountAsync(userId, CardSubType.CARDHOUSE_CARD_TYPE_STAFF_HEADCOACH)
            });
            stats.Add(new StickerBookStatResult
            {
                mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                mContextValue = 2,
                mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_STADIA,
                mValue = await GetCardCountAsync(userId, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_STADIUM)
            });
            stats.Add(new StickerBookStatResult
            {
                mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                mContextValue = 2,
                mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_KITS,
                mValue = await GetCardCountAsync(userId, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT)
            });
            stats.Add(new StickerBookStatResult
            {
                mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                mContextValue = 2,
                mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_BADGES,
                mValue = await GetCardCountAsync(userId, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_BADGE)
            });
            //TODO Trophies...
            stats.Add(new StickerBookStatResult
            {
                mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                mContextValue = 2,
                mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_TROPHIES,
                mValue = 1
            });
        }

        if (request.mContextId == RequestContext.CARDHOUSE_STICKERBOOK_STATS_REQUEST_CONTEXT_YEAR)
        {
            //TODO This doesn work yet
        }

        if (request.mContextId == RequestContext.CARDHOUSE_STICKERBOOK_STATS_REQUEST_CONTEXT_LEAGUE)
        {
            uint leagueId = request.mValue;
            var teamPlayerCounts = await GetTeamCountsInRangeAsync(userId, leagueId, playerTypes);
            foreach (var teamId in teamPlayerCounts.Keys)
            {
                stats.Add(new StickerBookStatResult
                {
                    mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_TEAM,
                    mContextValue = teamId,
                    mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_PLAYERS,
                    mValue = teamPlayerCounts[teamId]
                });
            }

            var teamJerseyCounts = await GetTeamCountsInRangeAsync(userId, leagueId, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT);
            foreach (var teamId in teamJerseyCounts.Keys)
            {
                stats.Add(new StickerBookStatResult
                {
                    mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_TEAM,
                    mContextValue = teamId,
                    mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_KITS,
                    mValue = teamJerseyCounts[teamId]
                });
            }
        }


        return new StickerBookStats2Response { mStats = stats };
    }

    public static async Task<Dictionary<uint, uint>> GetTeamCountsInRangeAsync(long userId, uint leagueId, params CardSubType[] subTypes)
    {
        var counts = new Dictionary<uint, uint>();

        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        string sql = @"
            SELECT team_id, COUNT(*) 
            FROM hut_cards 
            WHERE user_id = @user_id 
            AND team_id >= @startId AND team_id <= @endId 
            AND card_location IN (0, 1, 3, 4)";

        if (subTypes != null && subTypes.Length > 0)
        {
            sql += " AND sub_type = ANY(@sub_types)";
        }

        sql += " GROUP BY team_id";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("startId", HutCardFactory.LeagueTeamsMapping[leagueId].Start.Value);
        cmd.Parameters.AddWithValue("endId", HutCardFactory.LeagueTeamsMapping[leagueId].End.Value);

        if (subTypes != null && subTypes.Length > 0)
        {
            cmd.Parameters.AddWithValue("sub_types", subTypes.Select(s => (short)s).ToArray());
        }

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            counts[(uint)reader.GetInt32(0)] = (uint)reader.GetInt64(1);
        }

        return counts;
    }


    public static async Task<uint> GetCardCountAsync(long userId, params CardSubType[] subTypes)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        string sql = "SELECT COUNT(*) FROM hut_cards WHERE user_id = @user_id";

        if (subTypes != null && subTypes.Length > 0)
        {
            sql += " AND sub_type = ANY(@sub_types)";
        }

        sql += " AND card_location IN (0, 1, 3, 4)";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("user_id", userId);

        if (subTypes != null && subTypes.Length > 0)
        {
            cmd.Parameters.AddWithValue("sub_types", subTypes.Select(s => (short)s).ToArray());
        }

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToUInt32(result);
    }

    public override async Task<StickerBookSearchResponse> StickerBookSearchAsync(StickerBookSearchRequest request, BlazeRpcContext context)
    {
        long userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;

        List<CardData> cardDatas = await GetCardList(userId, request);
        
        return new StickerBookSearchResponse
        {
            mSearchResults = cardDatas
        };
    }

    // public override Task<StickerBookSearchResponse> StickerBookSearchAsync(StickerBookSearchRequest request, BlazeRpcContext context)
    // {
    //     List<CardData> retList = new List<CardData>();
    //     var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
    //
    //     //TODO Clean this horrendous mess
    //     switch (request.mCollectionSearchCardType)
    //     {
    //         case CollectionSearchType.COLLECTION_SEARCH_TYPE_ALL:
    //         {
    //             retList = HutManager.UserInventories[userId].Values.ToList();
    //             break;
    //         }
    //         case CollectionSearchType.COLLECTION_SEARCH_TYPE_HEADCOACH:
    //         {
    //             foreach (var VARIABLE in HutManager.UserInventories[userId].Values.ToList())
    //             {
    //                 if (VARIABLE.mCardSubTypeId.Equals(CardSubType.CARDHOUSE_CARD_TYPE_STAFF_HEADCOACH))
    //                 {
    //                     retList.Add(VARIABLE);
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case CollectionSearchType.COLLECTION_SEARCH_TYPE_BADGE:
    //         {
    //             foreach (var VARIABLE in HutManager.UserInventories[userId].Values.ToList())
    //             {
    //                 if (VARIABLE.mCardSubTypeId.Equals(CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_BADGE))
    //                 {
    //                     retList.Add(VARIABLE);
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case CollectionSearchType.COLLECTION_SEARCH_TYPE_STADIUM:
    //         {
    //             foreach (var VARIABLE in HutManager.UserInventories[userId].Values.ToList())
    //             {
    //                 if (VARIABLE.mCardSubTypeId.Equals(CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_STADIUM))
    //                 {
    //                     retList.Add(VARIABLE);
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case CollectionSearchType.COLLECTION_SEARCH_TYPE_PLAYER_ALL:
    //         {
    //             if (HutManager.UserInventories.TryGetValue(userId, out var inventory))
    //             {
    //                 foreach (var card in inventory.Values)
    //                 {
    //                     if (card.mCardSubTypeId >= (CardSubType)0 && card.mCardSubTypeId <= (CardSubType)3)
    //                     {
    //                         retList.Add(card);
    //                     }
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case CollectionSearchType.COLLECTION_SEARCH_TYPE_PLAYER_C:
    //         {
    //             if (HutManager.UserInventories.TryGetValue(userId, out var inventory))
    //             {
    //                 foreach (var card in inventory.Values)
    //                 {
    //                     if (card.mCardSubTypeId == CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C)
    //                     {
    //                         retList.Add(card);
    //                     }
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case CollectionSearchType.COLLECTION_SEARCH_TYPE_PLAYER:
    //         {
    //             if (HutManager.UserInventories.TryGetValue(userId, out var inventory))
    //             {
    //                 foreach (var card in inventory.Values)
    //                 {
    //                     if (card.mCardSubTypeId >= (CardSubType)0 && card.mCardSubTypeId <= (CardSubType)4)
    //                     {
    //                         retList.Add(card);
    //                     }
    //                 }
    //             }
    //
    //             break;
    //         }
    //         case CollectionSearchType.COLLECTION_SEARCH_TYPE_DEVELOPMENT:
    //         {
    //             if (HutManager.UserInventories.TryGetValue(userId, out var inventory))
    //             {
    //                 foreach (var card in inventory.Values)
    //                 {
    //                     if (card.mCardSubTypeId >= (CardSubType)51 && card.mCardSubTypeId <= (CardSubType)62 || card.mCardSubTypeId == (CardSubType)201)
    //                     {
    //                         retList.Add(card);
    //                     }
    //                 }
    //             }
    //
    //             break;
    //         }
    //         default:
    //         {
    //             throw new NotImplementedException();
    //         }
    //     }
    //
    //
    //     return Task.FromResult(new StickerBookSearchResponse
    //     {
    //         mSearchResults = retList
    //     });
    // }


    public override Task<StickerBookCardResponse> StickerBookCardAsync(StickerBookCardRequest request, BlazeRpcContext context)
    {
        throw new NotImplementedException();
        // CardData cardData = HutManager.GetCard(request.mCardId);
        // HutPlayerInstance playerInstance = HutManager.GetHutPlayerInstance(context.BlazeConnection);
        // var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        // if (HutManager.Escrow[userId].ContainsKey(request.mCardId))
        // {
        //     HutManager.Escrow[userId].Remove(request.mCardId);
        //     HutManager.UserInventories.TryAdd(userId, new Dictionary<long, CardData>());
        //     HutManager.UserInventories[userId].Add(request.mCardId, cardData);
        // }
        //
        // //TODO Does this always mean return to collection?
        // return Task.FromResult(new StickerBookCardResponse
        // {
        //     mTotalCredits = 0,
        //     mVersionInfo = playerInstance.GetVersionInfo()
        // });
    }


    public override async Task<ISSearchResponse> ISSearchAsync(ISSearchRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mExternalId;
        return await HutTradeManager.SearchTradesAsync(request, (long)userId);
    }

    public override Task<ISWatchListResponse> ISWatchListAsync(ISWatchListRequest request, BlazeRpcContext context)
    {
        throw new NotImplementedException();
        // var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mExternalId;
        // List<ISTradeInfo> retlist = new();
        // if (HutTradeManager.Watching.ContainsKey(userId))
        // {
        //     foreach (var VARIABLE in HutTradeManager.Watching[userId].ToArray())
        //     {
        //         retlist.Add(HutTradeManager.Auctions[VARIABLE]);
        //     }
        // }
        //
        // return Task.FromResult(new ISWatchListResponse
        // {
        //     mTradeResults = retlist,
        //     mTotalCount = retlist.Count
        // });
    }

    public override Task<ISWatchTradeResponse> ISWatchTradeAsync(ISWatchTradeRequest request, BlazeRpcContext context)
    {
        throw new NotImplementedException();
        
    }

    public override async Task<ISStartResponse> ISStartAsync(ISStartRequest request, BlazeRpcContext context)
    {
        ServerPlayer serverPlayer = ServerManager.GetServerPlayer(context.BlazeConnection);
        var tradeId = await HutTradeManager.InsertTrade(request, serverPlayer.UserIdentification.mAccountId, serverPlayer.UserIdentification.mName);

        return new ISStartResponse
        {
            mTradeId = tradeId
        };
    }

    public override async Task<ISOfferTradeResponse> ISOfferTradeAsync(ISOfferTradeRequest request, BlazeRpcContext context)
    {
        ServerPlayer serverPlayer = ServerManager.GetServerPlayer(context.BlazeConnection);
        var offerId = await HutTradeManager.InsertOffer(request, serverPlayer.UserIdentification.mAccountId);

        return new ISOfferTradeResponse
        {
            mOfferId = offerId
        };
    }


    public override async Task<ISViewTradeResponse> ISViewTradeAsync(ISViewTradeRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mExternalId;
        return await HutTradeManager.ViewTradeAsync(request, (long)userId);
    }

    public override Task<ISRemoveWatchResponse> ISRemoveWatchAsync(ISRemoveWatchRequest request, BlazeRpcContext context)
    {
        throw new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_UNKNOWN);
    }

    public override Task<ISAdminOfferResponse> ISAdminOfferAsync(ISAdminOfferRequest request, BlazeRpcContext context)
    {
        throw new NotImplementedException();
        
    }

    public override Task<ISGetOffersResponse> ISGetOffersAsync(ISGetOffersRequest request, BlazeRpcContext context)
    {
        throw new NotImplementedException();

        // List<ISOfferInfo> offers = new List<ISOfferInfo>();
        // if (HutTradeManager.TradeIdOfferAssocication.ContainsKey(request.mTradeId))
        // {
        //     foreach (var VARIABLE in HutTradeManager.TradeIdOfferAssocication[request.mTradeId])
        //     {
        //         if (HutTradeManager.Offers[VARIABLE].mOfferState == OfferState.CARDHOUSE_OFFERSTATE_ACTIVE)
        //         {
        //             offers.Add(HutTradeManager.Offers[VARIABLE]);
        //         }
        //     }
        // }
        //
        // return Task.FromResult(new ISGetOffersResponse
        // {
        //     mOfferList = offers,
        //     mTotalCount = offers.Count
        // });
    }

    public override Task<ActivateCardResponse> ActivateCardAsync(ActivateCardRequest request, BlazeRpcContext context)
    {
        //TODO No checks for now
        return Task.FromResult(new ActivateCardResponse
        {
            mCardId = request.mCardId
        });
    }

    public override Task<ApplyCardResponse> ApplyCardAsync(ApplyCardRequest request, BlazeRpcContext context)
    {
        //TODO No checks for now
        //TODO Doesnt work
        throw new NotImplementedException();
        // var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        // CardData cardData = HutManager.GetCard(userId, request.mTargetCards[0]);
        // CardData cardDataB = HutManager.GetCard(userId, request.mCardId);
        // cardData.mListTrainingCards[0] = (int)cardDataB.mCardDbId;
        // cardData.mAttributes[0] = 95;
        // HutManager.UserInventories[userId].TryAdd(request.mTargetCards[0], cardData);
        // return Task.FromResult(new ApplyCardResponse
        // {
        //     mCardId = request.mCardId,
        //     mCardDataList = new List<CardData>
        //     {
        //         cardData
        //     },
        //     mUserId = request.mUserId
        // });
    }

    public override Task<ApplySalaryCapResponse> ApplySalaryCapAsync(ApplySalaryCapRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new ApplySalaryCapResponse
        {
            mPlayerCardId = request.mPlayerCardId,
            mSalaryCap = request.mSalaryCap,
            mUserId = request.mUserId
        });
    }


    public override Task<MatchRegisterStartResponse> MatchRegisterStartAsync(MatchRegisterStartRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new MatchRegisterStartResponse
        {
            mId = 0
        });
    }

    public override Task<NumericResponse> MatchRegisterFinishAsync(MatchRegisterFinishRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new NumericResponse
        {
        });
    }

    public override Task<ChangePlayersResponse> ChangePlayersAsync(ChangePlayersRequest request, BlazeRpcContext context)
    {
        throw new NotImplementedException();
        // var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        // foreach (var VARIABLE in request.mCardDataList)
        // {
        //     var cardData = HutManager.UserInventories[userId][VARIABLE.mCardId];
        //     cardData.mInjuryGames = VARIABLE.mInjuryGames;
        //     cardData.mInjuryType = VARIABLE.mInjuryType;
        //     cardData.mListStats = VARIABLE.mListStats;
        //     HutManager.UserInventories[userId].TryAdd(VARIABLE.mCardId, cardData);
        // }
        //
        // return Task.FromResult(new ChangePlayersResponse
        // {
        //     mVal = 0
        // });
    }


    public override async Task<PlayGameResponse> PlayGameAsync(PlayGameRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        return new PlayGameResponse
        {
            mBonusAwarded = 10,
            mCredits = 10,
            mGoldenTickets = 10,
            mPrestige = 10,
            mTrophyCardCreated = 10,
            mVersionInfo = await GetOrCreateVersionInfo(userId)
        };
    }


    public override async Task<SquadLoadActiveResponse> SquadLoadActiveAsync(SquadLoadActiveRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        // HutPlayerInstance hutPlayerInstance = HutManager.HutPlayerInstances[userId];
        // List<CardData> retList = new();
        // foreach (var VARIABLE in HutManager.UserInventories[userId].Values.ToList())
        // {
        //     retList.Add(VARIABLE);
        // }
        // foreach (var VARIABLE in HutManager.UserInventories[userId].Values.ToList())
        // {
        //     if (VARIABLE.mCardStateId >= (CardState)100 && VARIABLE.mCardStateId <= (CardState)104)
        //     {
        //         retList.Add(VARIABLE);
        //     }
        // }
        var squadInfo = await GetSquadInfo(userId);

        return new SquadLoadActiveResponse
        {
            mActiveCards = await GetCardList(userId, CardLocation.ACTIVE_UTILITY),
            mSquadInfo = squadInfo.Value,
            mTargetUserId = 0
        };
    }

    public override async Task<CreatePackResponse> CreatePackAsync(CreatePackRequest request, BlazeRpcContext context)
    {
        long userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        var versionInfo = await GetOrCreateVersionInfo(userId);

        List<CardData> cardDataList = new List<CardData>();

        cardDataList.Add(await HutCardFactory.CreateRandomJerseyCard(userId, true, false));
        cardDataList.Add(await HutCardFactory.CreateRandomJerseyCard(userId, false, false));
        cardDataList.Add(await HutCardFactory.CreateRandomLogoCard(userId));
        cardDataList.Add(await HutCardFactory.CreateRandomStadiumCard(userId));
        cardDataList.Add(await HutCardFactory.CreateRandomHeadCoachCard(userId));
        cardDataList.Add(await HutCardFactory.CreateRandomTrainingCard(userId));
        cardDataList.Add(await HutCardFactory.CreateRandomContractCard(userId));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));
        cardDataList.Add(await HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C));

        return new CreatePackResponse
        {
            mCardDataList = cardDataList,
            mDuplicateCardIdPairList = new List<CardIdPair>(),
            mNumCards = (uint)cardDataList.Count,
            mNumPackPurchased = 0,
            mRandPackType = 0,
            mVersionInfo = versionInfo
        };
    }

    // public static uint _syncCounter;


    //
    // public static CardData GetOrCreateCard(uint val)
    // {
    //     if (HutManager.CardData.ContainsKey(val))
    //     {
    //         return HutManager.CardData[(ulong)val];
    //     }
    //     else
    //     {
    //         CardData cardData = new CardData
    //         {
    //             mAttributes = new List<byte>
    //             {
    //                 51, 52, 53, 54, 55
    //             },
    //             mCardStateId = 1,
    //             mCardId = val,
    //             mCardDbId = val,
    //             mFormationId = 2,
    //             mFREE = 0,
    //             mCareerRemaining = 2,
    //             mInjuryGames = 0,
    //             mInjuryType = 0,
    //             mMaxTrainingCardsCanApply = 1,
    //             mNumberOfOwners = 1,
    //             mPreferredPositionId = 1,
    //             mDiscardPrice = 2,
    //             mRareFlag = 1,
    //             mRating = 2,
    //             mSalaryCap = 1,
    //             mListStats = new List<byte>
    //             {
    //                 1, //Games Played 
    //                 62, //Goals 
    //                 63, //Assists 
    //                 64, //Plus/Minus
    //                 65, //Penalty Minutes
    //                 66,
    //                 67,
    //                 68,
    //                 69,
    //                 70
    //             },
    //             mCardSubTypeId = CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C,
    //             mDateIssued = 1767011983,
    //             mTeamId = 0,
    //             mListTrainingCards = new List<byte>
    //             {
    //                 0, 72, 73, 74, 75, 78, 79, 255
    //             },
    //             mUsesRemaining = 5
    //         };
    //         HutManager.CardData.Add(val, cardData);
    //         return cardData;
    //     }
    // }
    //
    // public static CardData GetOrCreateLogoCard(uint val = 6000011)
    // {
    //     if (HutManager.CardData.ContainsKey(val))
    //     {
    //         return HutManager.CardData[(ulong)val];
    //     }
    //     else
    //     {
    //         CardData cardData = new CardData
    //         {
    //             // mAttributes = new List<byte>
    //             // {
    //                 // 51, 52, 53, 54, 55
    //             // },
    //             mCardStateId = 1, // 1 tai 7
    //             mCardId = val,
    //             mCardDbId = val,
    //             mFormationId = 2,
    //             mFREE = 0,
    //             mCareerRemaining = 2,
    //             mInjuryGames = 0,
    //             mInjuryType = 0,
    //             mMaxTrainingCardsCanApply = 1,
    //             mNumberOfOwners = 1,
    //             mPreferredPositionId = 1,
    //             mDiscardPrice = 2,
    //             mRareFlag = 1,
    //             mRating = 2,
    //             mSalaryCap = 1,
    //             mListStats = new List<byte>
    //             {
    //                 1, //Games Played 
    //                 62, //Goals 
    //                 63, //Assists 
    //                 64, //Plus/Minus
    //                 65, //Penalty Minutes
    //                 66,
    //                 67,
    //                 68,
    //                 69,
    //                 70
    //             },
    //             mCardSubTypeId = CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_BADGE,
    //             mDateIssued = 1767011983,
    //             mTeamId = 0,
    //             mListTrainingCards = new List<byte>
    //             {
    //                 0, 72, 73, 74, 75, 78, 79, 255
    //             },
    //             mUsesRemaining = 5
    //         };
    //         HutManager.CardData.Add(val, cardData);
    //         return cardData;
    //     }
    // }
    // public static CardData GetOrCreateKitCard(uint val = 6300011)//10 redwing //11 oiler
    // {
    //     if (HutManager.CardData.ContainsKey(val))
    //     {
    //         return HutManager.CardData[(ulong)val];
    //     }
    //     else
    //     {
    //         CardData cardData = new CardData
    //         {
    //             // mAttributes = new List<byte>
    //             // {
    //             // 51, 52, 53, 54, 55
    //             // },
    //             mCardStateId = 1, // 1 tai 7
    //             mCardId = val,
    //             mCardDbId = val,
    //             mFormationId = 2,
    //             mFREE = 0,
    //             mCareerRemaining = 2,
    //             mInjuryGames = 0,
    //             mInjuryType = 0,
    //             mMaxTrainingCardsCanApply = 1,
    //             mNumberOfOwners = 1,
    //             mPreferredPositionId = 1,
    //             mDiscardPrice = 2,
    //             mRareFlag = 1,
    //             mRating = 2,
    //             mSalaryCap = 1,
    //             mListStats = new List<byte>
    //             {
    //                 1, //Games Played 
    //                 62, //Goals 
    //                 63, //Assists 
    //                 64, //Plus/Minus
    //                 65, //Penalty Minutes
    //                 66,
    //                 67,
    //                 68,
    //                 69,
    //                 70
    //             },
    //             mCardSubTypeId = CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT,
    //             mDateIssued = 1767011983,
    //             mTeamId = 0,
    //             mListTrainingCards = new List<byte>
    //             {
    //                 0, 72, 73, 74, 75, 78, 79, 255
    //             },
    //             mUsesRemaining = 5
    //         };
    //         HutManager.CardData.Add(val, cardData);
    //         return cardData;
    //     }
    // }
}