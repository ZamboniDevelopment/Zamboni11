using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blaze3SDK;
using Blaze3SDK.Blaze.Example;
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
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        var gamerInfo = await HutManager.GetGamerInfo(userId);
        if (gamerInfo == null) return new LoginResponse();
        return new LoginResponse
        {
            mTeamAbbreviation = gamerInfo.Value.mTeamAbbreviation,
            mBonusAwarded = 0,
            mTeamName = gamerInfo.Value.mTeamName,
            mRewardType = 0,
            mRewardValue = 0,
            mUserId = 0
        };
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
        CardData cardData = await HutManager.GetCard(request.mCardId);
        var versionInfo = await HutManager.GetVersionInfo(userId);
        switch (request.mDeckType)
        {
            case DeckType.CARDHOUSE_DECK_ESCROW:
                await HutCardFactory.CreateOrUpdateCard(cardData, userId, DeckType.CARDHOUSE_DECK_ESCROW);
                versionInfo = await HutManager.IncrementVersionInfo(userId, HutManager.VersionType.Escrow);
                break;
            default:
                throw new NotImplementedException();
        }

        return new MoveCardResponse
        {
            mDisplacedCardId = request.mCardId,
            mDisplacedDeckType = request.mDeckType,
            mDisplacedCardPosition = 0,
            mVersionInfo = versionInfo.Value
        };
    }

    public override async Task<GamerGetInfoResponse> GetGamerInfoRequestAsync(GamerGetInfoRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        var gamerInfo = await HutManager.GetGamerInfo(userId);
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
        await HutManager.SetGamerInfo(request.mGamerInfo, userId);
        return new NumericResponse
        {
            mNumber = 0
        };
    }

    public async Task<List<CardData>> GetCardList(long userId, StickerBookSearchRequest request)
    {
        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        var sql = new StringBuilder(@"
        SELECT * 
              FROM hut_cards
        WHERE user_id = @user_id");

        sql.Append(" AND deck_type IN (0, 1, 3, 4)");
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

        if (request.mLeagueId >= 0)
        {
            var range = HutCardFactory.LeagueTeamsMapping[request.mLeagueId];
            sql.Append(" AND team_id BETWEEN " + range.Start.Value + " AND " + range.End.Value + "");
        }

        if (request.mTeamId >= 0)
        {
            sql.Append(" AND team_id =" + request.mTeamId + "");
        }

        await using var cmd = new NpgsqlCommand(sql.ToString(), conn);
        cmd.Parameters.AddWithValue("user_id", userId);

        await using var reader = await cmd.ExecuteReaderAsync();

        List<CardData> cardDataList = new List<CardData>();

        while (await reader.ReadAsync())
        {
            cardDataList.Add(HutHelper.ReadCardData(reader));
        }

        return cardDataList;
    }

    public override async Task<DeckInfoResponse> GetDeckInfoAsync(DeckInfoRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        
        var generalInfo = await HutManager.GetGeneralInfo(userId);
        if (generalInfo == null) generalInfo = await HutManager.SetGeneralInfo(new GeneralInfo
        {
            mCredits = 1000,
            mStats = new List<byte>()
        }, userId); 
        
        var squadInfo = await HutManager.GetSquadInfo(userId);
        uint teamRating = 0;
        if (squadInfo != null) teamRating = squadInfo.Value.mStarRating;
        
        var versionInfo = await HutManager.GetVersionInfo(userId);
        if (versionInfo == null) versionInfo = await HutManager.CreateVersionInfo(new VersionInfo
        {
            mVersionEscrow = 1,
            mVersionGeneral = 1,
            mVersionUnassigned = 1
        }, userId);
        
        var escrowList = await HutManager.GetCardList(userId, DeckType.CARDHOUSE_DECK_ESCROW, CardState.CARDHOUSE_CARDSTATE_FREE);
        var unassignedList = await HutManager.GetCardList(userId, DeckType.CARDHOUSE_DECK_UNASSIGNED, CardState.CARDHOUSE_CARDSTATE_FREE);

        return new DeckInfoResponse
        {
            mDuplicateEscrowCardIdPairList = new List<CardIdPair>(),
            mDuplicateUnassignedCardIdPairList = new List<CardIdPair>(),
            mEscrowCardDataList = escrowList,
            mEscrowCount = (byte)escrowList.Count,
            mGeneralInfo = generalInfo.Value,
            mTeamRating = teamRating,
            mUnassignedCardDataList = unassignedList,
            mUserId = 0,
            mVersionInfo = versionInfo.Value
        };
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

    public override Task<StoreGetPackTypesResponse> StoreGetPackTypesAsync(StoreGetPackTypesRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new StoreGetPackTypesResponse
        {
            mFreePack = 0,
            mPremiumPacksHidden = 0,
            mPackTypeList = new List<StorePackTypeData>()
            {
                new StorePackTypeData
                {
                    mAttributes = StorePackAttribute.CARDHOUSE_STOREPACKATTRIBUTES_SAVINGS_COINS,
                    mAvailability = StorePackAvailability.CARDHOUSE_STOREPACKAVAILABILITY_COINS,
                    mCoinCost = 1,
                    mEndDate = 0,
                    mId = StorePackId.CARDHOUSE_CARD_PACK_TYPE_PEEWEE,
                    mQuantity = 0,
                    mSaleType = StoreSaleType.CARDHOUSE_STORESALETYPE_NONE,
                    mStartDate = 0,
                    mState = StorePackState.CARDHOUSE_STOREPACKSTATE_ACTIVE
                }
            },
            mServerTime = 0
        });
    }

    public override Task<StorePackQuantitiesResponse> StorePackQuantitiesAsync(StorePackQuantitiesRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new StorePackQuantitiesResponse
        {
            mPackQuantitiesList = new List<int>
            {
                10, 20
            }
        });
    }

    //
    public override async Task<DiscardCardResponse> DiscardCardAsync(DiscardCardRequest request, BlazeRpcContext context)
    {
        //TODO Maybe checks
        long userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        CardData cardData = await HutManager.GetCard(request.mCardId);
        await HutCardFactory.CreateOrUpdateCard(cardData, userId, DeckType.CARDHOUSE_DECK_INVALID);
        var generalInfo = await HutManager.GetGeneralInfo(userId);
        await HutManager.SetGeneralInfo(new GeneralInfo
        {
            mCredits = request.mCredits + generalInfo.Value.mCredits,
            mStats = generalInfo.Value.mStats
        }, userId);
        VersionInfo versionInfo = await HutManager.IncrementVersionInfo(userId, HutManager.VersionType.General);
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
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        foreach (var assignCardCard in request.mList)
        {
            CardData cardData = await HutManager.GetCard(assignCardCard.mCardId);
            cardData.mCardStateId = assignCardCard.mCardStateId;
            await HutCardFactory.CreateOrUpdateCard(cardData, userId, assignCardCard.mDeckType);
        }
        await HutManager.IncrementVersionInfo(userId, HutManager.VersionType.Unassigned);
        var versionInfo = HutManager.GetVersionInfo(userId);
        return new AssignCardsResponse
        {
            mVersionInfo = versionInfo.Result.Value
        };
    }


    public override async Task<UserReliabilityInfoResponse> GetUserReliabilityInfoAsync(ProvidedUID request, BlazeRpcContext context)
    {
        return new UserReliabilityInfoResponse
        {
            mPreviousMatchUnfinished = 0,
            mMatchesFinished = 10,
            mMatchesStarted = 10,
            mReliability = 0,
            mUserId = 0
        };
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

    public override async Task<SquadListResponse> GetSquadListAsync(ProvidedUID request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        var squadInfo = await HutManager.GetSquadInfo(userId);
        if (squadInfo == null) return new SquadListResponse();
        
        return new SquadListResponse
        {
            mActiveSquad = 1,
            mSquads = new List<SquadSmall>
            {
                new SquadSmall
                {
                    mChemistry = squadInfo.Value.mChemistry,
                    mFormation = squadInfo.Value.mFormationId,
                    mRating = squadInfo.Value.mStarRating,
                    mSquadId = 0,
                    mSquadName = squadInfo.Value.mSquadName
                }
            }
        };
    }

    public override async Task<ViewCardsResponse> ViewCardsAsync(ViewCardsRequest request, BlazeRpcContext context)
    {
        var cardDataList = new List<CardData>();
        foreach (var cardId in request.mCardIdList)
        {
            cardDataList.Add(await HutManager.GetCard(cardId));
        }

        return new ViewCardsResponse
        {
            mCardDataList = cardDataList
        };
    }

    public override async Task<SquadSaveResponse> SquadSaveAsync(SquadSaveRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        await HutManager.SetSquadInfo(request, userId);
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
            int leagueId = request.mValue;
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

    public static async Task<Dictionary<uint, uint>> GetTeamCountsInRangeAsync(long userId, int leagueId, params CardSubType[] subTypes)
    {
        var counts = new Dictionary<uint, uint>();

        await using var conn = new NpgsqlConnection(Database.ConnectionString);
        await conn.OpenAsync();

        string sql = @"
            SELECT team_id, COUNT(*) 
            FROM hut_cards 
            WHERE user_id = @user_id 
            AND team_id >= @startId AND team_id <= @endId 
            AND deck_type IN (0, 1, 3, 4)";

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

        sql += " AND deck_type IN (0, 1, 3, 4)";
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

    public override async Task<ChangePlayersResponse> ChangePlayersAsync(ChangePlayersRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;

        foreach (var loopVar in request.mCardDataList)
        {
            CardData cardData = await HutManager.GetCard(loopVar.mCardId);
            cardData.mUsesRemaining--;
            //TODO Having injures haven't happened so far, so this is not confirmed to work correctly
            cardData.mInjuryGames = loopVar.mInjuryGames;
            cardData.mInjuryType = loopVar.mInjuryType;
            cardData.mListStats = loopVar.mListStats;
            await HutCardFactory.CreateOrUpdateCard(cardData, userId);

        }

        return new ChangePlayersResponse();
    }


    public override async Task<PlayGameResponse> PlayGameAsync(PlayGameRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        var versionInfo = await HutManager.GetVersionInfo(userId);
        
        return new PlayGameResponse
        {
            mBonusAwarded = 10,
            mCredits = 10,
            mGoldenTickets = 10,
            mPrestige = 10,
            mTrophyCardCreated = 10,
            mVersionInfo = versionInfo.Value
        };
    }


    public override async Task<SquadLoadActiveResponse> SquadLoadActiveAsync(SquadLoadActiveRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        var squadInfo = await HutManager.GetSquadInfo(userId);
        if (squadInfo == null) throw new Exception();

        List<CardData> activeCards = new();
        activeCards.AddRange(await HutManager.GetCardList(userId, DeckType.CARDHOUSE_DECK_STICKERBOOK, CardState.CARDHOUSE_CARDSTATE_ACTIVE_BADGE));
        activeCards.AddRange(await HutManager.GetCardList(userId, DeckType.CARDHOUSE_DECK_STICKERBOOK, CardState.CARDHOUSE_CARDSTATE_ACTIVE_AWAY_KIT));
        activeCards.AddRange(await HutManager.GetCardList(userId, DeckType.CARDHOUSE_DECK_STICKERBOOK, CardState.CARDHOUSE_CARDSTATE_ACTIVE_HOME_KIT));
        activeCards.AddRange(await HutManager.GetCardList(userId, DeckType.CARDHOUSE_DECK_STICKERBOOK, CardState.CARDHOUSE_CARDSTATE_ACTIVE_STADIUM));
        
        return new SquadLoadActiveResponse
        {
            mActiveCards = activeCards,
            mSquadInfo = squadInfo.Value,
            mTargetUserId = (long)request.mTargetUserId
        };
    }

    public override async Task<CreatePackResponse> CreatePackAsync(CreatePackRequest request, BlazeRpcContext context)
    {
        long userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        var versionInfo = await HutManager.GetVersionInfo(userId);

        List<CardData> cardDataList = await HutPackFactory.CreatePack(userId, request.mPackType);

        return new CreatePackResponse
        {
            mCardDataList = cardDataList,
            mDuplicateCardIdPairList = new List<CardIdPair>(),
            mNumCards = (uint)cardDataList.Count,
            mNumPackPurchased = 0,
            mRandPackType = 0,
            mVersionInfo = versionInfo.Value
        };
    }

}