using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaze3SDK;
using BlazeCommon;
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
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
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


    public override async Task<NumericResponse> LogoutRequestAsync(LogoutRequest request, BlazeRpcContext context)
    {
        return new NumericResponse
        {
            mNumber = 0,
        };
    }

    public override async Task<MoveCardResponse> MoveCardAsync(MoveCardRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
        CardData cardData = (await HutManager.GetCard(request.mCardId, userId)).Card;
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
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
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
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
        await HutManager.SetGamerInfo(request.mGamerInfo, userId);
        return new NumericResponse
        {
            mNumber = 0
        };
    }

    public override async Task<DeckInfoResponse> GetDeckInfoAsync(DeckInfoRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;

        var generalInfo = await HutManager.GetGeneralInfo(userId);
        if (generalInfo == null)
            generalInfo = await HutManager.SetGeneralInfo(new GeneralInfo
            {
                mCredits = 1000,
                mStats = new List<int>()
                {
                    //Todo figure if other placeholders have meaning
                    1,2,3,4,5,6,7,8,
                    0, //WINS
                    0, //LOSES
                    0, //OTL
                    12,13,14,15,16,17,18,19,20,
                    21,22,23,24,25,26,27,28,29,30
                }
            }, userId);

        var squadInfo = await HutManager.GetSquadInfo(userId);
        uint teamRating = 0;
        if (squadInfo != null) teamRating = squadInfo.Value.mStarRating;

        var versionInfo = await HutManager.GetVersionInfo(userId);
        if (versionInfo == null)
        {
            versionInfo = await HutManager.CreateVersionInfo(new VersionInfo
            {
                mVersionEscrow = 1,
                mVersionGeneral = 1,
                mVersionUnassigned = 1
            }, userId);
        }


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

    public override async Task<GetConfigResponse> GetConfigRequestAsync(NumericRequest request, BlazeRpcContext context)
    {
        return new GetConfigResponse
        {
            mConfigList = new List<int>
            {
                //Todo figure if other placeholders have meaning
                11, 21, 22, 33, 44, 55, 66, 77, 88, 99, 10,
                11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                21, 22, 23, 24, 25, 26, 27, 
                100, //Game End: Completion award
                29, 
                50, //Game End Positive: Pucks per Goal
                31, 
                5, //Game End Positive: Pucks per Shots on goal
                33, 
                5, //Game End Positive: Pucks per Hits
                35, 
                5, //Game End Positive: Pucks per Faceoffs Won
                37, 
                20, //Game End Positive: Pucks per Time on attack MINUTES
                39, 
                1, //Game End Positive: Pucks per Powerplay % something?
                41, 
                5, //Game End Positive: Pucks per successful pass
                43, 
                -50, //Game End Negative: Puck deduction per opponent goals
                45, 
                -25, //Game End Negative: Puck deduction per penalty minute
                47, 
                -25, //Game End Negative: Puck deduction per icing
                49, 
                -10, //Game End Negative: Puck deduction per offside
                51,
                50, //Bid increment
                53,
                54, 
                55, 
                56, 
                10, //Difficulty Rookie Multiplier
                10, //Difficulty Pro Multiplier
                10, //Difficulty All-Star Multiplier
                11, //Difficulty Super-Star Multiplier
                61, 62, 63, 64, 65, 66, 67, 68, 69, 70,
                71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
                81, 82, 83, 84, 85, 86, 87, 88, 89, 90,
                91, 92, 93, 94, 95, 96, 97, 98, 99, 100,
                101, 102, 103, 104, 105, 106, 107, 108, 109, 110,
                111, 112, 113, 114, 115, 116, 117, 118, 119, 120,
                121, 122, 123, 124, 125, 126, 127, 128, 129, 130,
                131, 132, 133, 134, 135, 136, 137, 138, 139, 140,
                141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
                151, 152, 153, 154, 155, 156, 157, 158, 159, 160,
                161, 162, 163, 164, 165, 166, 167, 168, 169, 170,
                171, 172, 173, 174, 175, 176, 177, 178, 179, 180,
                181, 182, 183, 184, 185, 186, 187, 188, 189, 190,
                191, 192, 193, 194, 195, 196, 197, 198, 199, 200,
                201, 202, 203, 204, 205, 206, 207, 208, 209, 210,
                211, 212, 213, 214, 215, 216, 217, 218, 219, 220,
                221, 222, 223, 224, 225, 226, 227, 228, 229, 230,
                231, 232, 233, 234, 235, 236, 237, 238, 239, 240,
                241, 242, 243, 244, 245, 246, 247, 248, 249, 250,
                251, 252, 253, 254, 255, 256, 257, 258, 259, 260,
                261, 262, 263, 264, 265, 266, 267, 268, 269, 270,
                271, 272, 273, 274, 275, 276, 277, 278, 279, 280,
                281, 282, 283, 284, 285, 286, 287, 288, 289, 290,
                291, 292, 293, 294, 295, 296, 297, 298, 299, 300,
                301, 302, 303, 304, 305, 306, 307, 308, 309, 310,
                311, 312, 313, 314, 315, 316, 317, 318, 319, 320,
                321, 322, 323, 324, 325, 326, 327, 328, 329, 330,
                331, 332, 333, 334, 335, 336, 337, 338, 339, 340,
                341, 342, 343, 344, 345, 346, 347, 348, 349, 350,
                351, 352, 353, 354, 355, 356, 357, 358, 359, 360,
                361, 362, 363, 364, 365, 366, 367, 368, 369, 370,
            }
        };
    }

    public override async Task<StoreGetPackTypesResponse> StoreGetPackTypesAsync(StoreGetPackTypesRequest request, BlazeRpcContext context)
    {
        return new StoreGetPackTypesResponse
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
        };
    }

    public override async Task<StorePackQuantitiesResponse> StorePackQuantitiesAsync(StorePackQuantitiesRequest request, BlazeRpcContext context)
    {
        return new StorePackQuantitiesResponse
        {
            mPackQuantitiesList = new List<int>
            {
                10, 20
            }
        };
    }

    public override async Task<DiscardCardResponse> DiscardCardAsync(DiscardCardRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
        var cardData = await HutManager.GetCard(request.mCardId, userId);
        await HutManager.HardDelete(userId, cardData.Card.mCardId);
        await HutHelper.Deposit(userId, request.mCredits);

        switch (cardData.DeckType)
        {
            case DeckType.CARDHOUSE_DECK_ESCROW: await HutManager.IncrementVersionInfo(userId, HutManager.VersionType.Escrow); break;
            case DeckType.CARDHOUSE_DECK_UNASSIGNED: await HutManager.IncrementVersionInfo(userId, HutManager.VersionType.Unassigned); break;
            default: throw new NotImplementedException();
        }

        var versionInfo = await HutManager.GetVersionInfo(userId);

        return new DiscardCardResponse
        {
            mCredits = request.mCredits,
            mVersionInfo = versionInfo.Value
        };
    }

    public override async Task<StaffBonusResponse> GetStaffBonusAsync(NumericRequest request, BlazeRpcContext context)
    {
        return new StaffBonusResponse
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
        };
    }

    public override async Task<AssignCardsResponse> AssignCardsAsync(AssignCardsRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
        foreach (var assignCardCard in request.mList)
        {
            CardData cardData = (await HutManager.GetCard(assignCardCard.mCardId)).Card;
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


    public override async Task<UserReliabilityInfoResponse> GetUserReliabilityInfoAsync(NumericRequest request, BlazeRpcContext context)
    {
        return new UserReliabilityInfoResponse
        {
            mPreviousMatchUnfinished = 0,
            mMatchesFinished = 10,
            mMatchesStarted = 10,
            mReliability = 100,
            mUserId = 0
        };
    }

    public override async Task<NumericResponse> ResetUserRequestAsync(NumericRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;

        await HutManager.HardDelete(userId);
        return new NumericResponse
        {
            mNumber = 0,
        };
    }

    public override async Task<SquadListResponse> GetSquadListAsync(NumericRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
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
            cardDataList.Add((await HutManager.GetCard(cardId)).Card);
        }

        return new ViewCardsResponse
        {
            mCardDataList = cardDataList
        };
    }

    public override async Task<SquadSaveResponse> SquadSaveAsync(SquadSaveRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
        await HutManager.SetSquadInfo(request, userId);
        return new SquadSaveResponse
        {
            mSquadId = request.mSquadId
        };
    }

    private static int debugCounter = 0;

    public override async Task<StickerBookStats2Response> StickerBookStats2Async(StickerBookStats2Request request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;

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
                mValue = await HutManager.GetCardCountAsync(userId, DeckType.CARDHOUSE_DECK_STICKERBOOK, playerTypes)
            });
            stats.Add(new StickerBookStatResult
            {
                mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                mContextValue = 2,
                mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_STAFF_HEADCOACH,
                mValue = await HutManager.GetCardCountAsync(userId, DeckType.CARDHOUSE_DECK_STICKERBOOK, CardSubType.CARDHOUSE_CARD_TYPE_STAFF_HEADCOACH)
            });
            stats.Add(new StickerBookStatResult
            {
                mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                mContextValue = 2,
                mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_STADIA,
                mValue = await HutManager.GetCardCountAsync(userId, DeckType.CARDHOUSE_DECK_STICKERBOOK, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_STADIUM)
            });
            stats.Add(new StickerBookStatResult
            {
                mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                mContextValue = 2,
                mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_KITS,
                mValue = await HutManager.GetCardCountAsync(userId, DeckType.CARDHOUSE_DECK_STICKERBOOK, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT)
            });
            stats.Add(new StickerBookStatResult
            {
                mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                mContextValue = 2,
                mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_BADGES,
                mValue = await HutManager.GetCardCountAsync(userId, DeckType.CARDHOUSE_DECK_STICKERBOOK, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_BADGE)
            });

            //TODO Trophies...
        }

        if (request.mContextId == RequestContext.CARDHOUSE_STICKERBOOK_STATS_REQUEST_CONTEXT_YEAR)
        {
            foreach (var leagueId in HutCardFactory.LeagueTeamsMapping.Keys)
            {
                var correction = 0;
                if (leagueId is 0 or 1 or 2) correction = 2; //why? that's what I am wondering too
                if (leagueId == 3) correction = 1;
                var playerCounts = await HutManager.GetTeamCardCountsAsync(userId, leagueId, DeckType.CARDHOUSE_DECK_STICKERBOOK, playerTypes);

                stats.Add(new StickerBookStatResult
                {
                    mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                    mContextValue = leagueId,
                    mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_PLAYERS_BRONZE,
                    mValue = playerCounts.Values.Sum() + correction
                });

                var jerseyCounts = await HutManager.GetTeamCardCountsAsync(userId, leagueId, DeckType.CARDHOUSE_DECK_STICKERBOOK, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT);

                stats.Add(new StickerBookStatResult
                {
                    mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                    mContextValue = leagueId,
                    mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_KITS,
                    mValue = jerseyCounts.Values.Sum()
                });

                var badgeCounts = await HutManager.GetTeamCardCountsAsync(userId, leagueId, DeckType.CARDHOUSE_DECK_STICKERBOOK, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_BADGE);

                stats.Add(new StickerBookStatResult
                {
                    mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                    mContextValue = leagueId,
                    mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_BADGES,
                    mValue = badgeCounts.Values.Sum()
                });
            }

            stats.Add(new StickerBookStatResult
            {
                mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                mContextValue = 12,
                mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_STADIA,
                mValue = await HutManager.GetCardCountAsync(userId, DeckType.CARDHOUSE_DECK_STICKERBOOK, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_STADIUM)
            });

            stats.Add(new StickerBookStatResult
            {
                mContextId = ResultContext.CARDHOUSE_STICKERBOOK_STAT_RESULT_CONTEXT_YEAR,
                mContextValue = 13,
                mTypeId = ResultType.CARDHOUSE_STICKERBOOK_STAT_RESULT_TYPE_BALLS,
                mValue = await HutManager.GetCardCountAsync(userId, DeckType.CARDHOUSE_DECK_STICKERBOOK, CardSubType.CARDHOUSE_CARD_TYPE_STAFF_HEADCOACH)
            });
            //TODO TROPHIES!
        }

        if (request.mContextId == RequestContext.CARDHOUSE_STICKERBOOK_STATS_REQUEST_CONTEXT_LEAGUE)
        {
            int leagueId = request.mValue;
            var teamPlayerCounts = await HutManager.GetTeamCardCountsAsync(userId, leagueId, DeckType.CARDHOUSE_DECK_STICKERBOOK, playerTypes);
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

            var teamJerseyCounts = await HutManager.GetTeamCardCountsAsync(userId, leagueId, DeckType.CARDHOUSE_DECK_STICKERBOOK, CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT);
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


    public override async Task<StickerBookSearchResponse> StickerBookSearchAsync(StickerBookSearchRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;

        List<CardData> cardDatas = await HutManager.GetCardList(userId, request);

        return new StickerBookSearchResponse
        {
            mSearchResults = cardDatas
        };
    }

    public override async Task<StickerBookCardResponse> StickerBookCardAsync(StickerBookCardRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
        var card = (await HutManager.GetCard(request.mCardId, (long)userId));
        await HutCardFactory.CreateOrUpdateCard(card.Card, (long)userId, DeckType.CARDHOUSE_DECK_STICKERBOOK);
        switch (card.DeckType)
        {
            case DeckType.CARDHOUSE_DECK_ESCROW: await HutManager.IncrementVersionInfo((long)userId, HutManager.VersionType.Escrow); break;
            case DeckType.CARDHOUSE_DECK_UNASSIGNED: await HutManager.IncrementVersionInfo((long)userId, HutManager.VersionType.Unassigned); break;
            default: throw new Exception();
        }
        var versionInfo = await HutManager.GetVersionInfo((long)userId);

        return new StickerBookCardResponse
        {
            mTotalCredits = 0,
            mVersionInfo = versionInfo.Value
        };
    }


    public override async Task<ISSearchResponse> ISSearchAsync(ISSearchRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
        return await HutTradeManager.SearchTradesAsync(request, (long)userId);
    }

    public override async Task<ISWatchListResponse> ISWatchListAsync(ISWatchListRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
        var tradeInfos = await HutTradeManager.GetWatchedTrades(userId);
        return new ISWatchListResponse
        {
            mTradeResults = tradeInfos,
            mTotalCount = tradeInfos.Count
        };
    }

    public override async Task<ISWatchTradeResponse> ISWatchTradeAsync(ISWatchTradeRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
        await HutTradeManager.InsertWatching(request.mTradeId, userId);
        return new ISWatchTradeResponse();
    }

    public override async Task<ISRemoveWatchResponse> ISRemoveWatchAsync(ISRemoveWatchRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
        await HutTradeManager.RemoveWatching(request, userId);
        return new ISRemoveWatchResponse();
    }

    public override async Task<ISStartResponse> ISStartAsync(ISStartRequest request, BlazeRpcContext context)
    {
        ServerPlayer serverPlayer = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!;
        var tradeId = await HutTradeManager.InsertTrade(request, serverPlayer.UserIdentification.mAccountId, serverPlayer.UserIdentification.mName);

        return new ISStartResponse
        {
            mTradeId = tradeId
        };
    }

    public override async Task<ISOfferTradeResponse> ISOfferTradeAsync(ISOfferTradeRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
        var result = await HutTradeManager.InsertOffer(request, userId);

        if (result.Exception != null)
        {
            throw new BlazeRpcException(result.Exception.ErrorCode);
        }

        return new ISOfferTradeResponse
        {
            mOfferId = result.OfferId
        };
    }


    public override async Task<ISViewTradeResponse> ISViewTradeAsync(ISViewTradeRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
        return await HutTradeManager.ViewTradeAsync(request, userId);
    }


    public override async Task<ISAdminOfferResponse> ISAdminOfferAsync(ISAdminOfferRequest request, BlazeRpcContext context)
    {
        throw new NotImplementedException();
    }

    public override async Task<ISGetOffersResponse> ISGetOffersAsync(ISGetOffersRequest request, BlazeRpcContext context)
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

    public override async Task<ActivateCardResponse> ActivateCardAsync(ActivateCardRequest request, BlazeRpcContext context)
    {
        //TODO No checks for now
        return new ActivateCardResponse
        {
            mCardId = request.mCardId
        };
    }
    
    public override async Task<TournamentListResponse> TournamentListRequestAsync(NullStruct request, BlazeRpcContext context)
    {
        return new TournamentListResponse
        {
            mServerTime = Util.TimeNow(),
            mTournaments = new List<TournamentInfo>
            {
                new TournamentInfo
                {
                    mAiGroup = 0,
                    mBlazeTournamentId = 1,
                    mDifficulty = 0,
                    mElg1Data = 0,
                    mElg1Type = 0,
                    mElg12Data = 0,
                    mElg2Type = 0,
                    mEndTime = Util.TimeNow()+10000,
                    // mMatchLenght = -1,
                    mPrize = 1000,
                    mReward1 = 1000,
                    mReward2 = 1100,
                    mReward3 = 1200,
                    mReward4 = 1300,
                    mSalaryCap = 3000,
                    mStartTime = Util.TimeNow(),
                    mTrophyCardDbId = 8200000,
                    mTournamentId = 10,
                    mType = 1, //0=SP 1=SP/MP 2=SPF/SP/MP 3=MPF/SP/MP
                    mUnlock = 0
                }
            }
        };
    }

    public override async Task<TournamentSaveDataResponse> TournamentSaveDataRequestAsync(TournamentSaveDataRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;

        await HutTournamentManager.SaveTournament(request, userId);
        return new TournamentSaveDataResponse();
    }
    
    public override async Task<TournamentLoadDataResponse> TournamentLoadDataRequestAsync(TournamentLoadDataRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;

        byte[] tournamentData = await HutTournamentManager.LoadTournament(request, userId);
        if (tournamentData.Length <= 0) throw new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_NO_TOURNAMENT_DATA);
        
        return new TournamentLoadDataResponse
        {
            mData = tournamentData
        };
    }

    public override async Task<ApplyCardResponse> ApplyCardAsync(ApplyCardRequest request, BlazeRpcContext context)
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

    public override async Task<ApplySalaryCapResponse> ApplySalaryCapAsync(ApplySalaryCapRequest request, BlazeRpcContext context)
    {
        return new ApplySalaryCapResponse
        {
            mPlayerCardId = request.mPlayerCardId,
            mSalaryCap = request.mSalaryCap,
            mUserId = request.mUserId
        };
    }


    public override async Task<MatchRegisterStartResponse> MatchRegisterStartAsync(MatchRegisterStartRequest request, BlazeRpcContext context)
    {
        return new MatchRegisterStartResponse
        {
            mId = 0
        };
    }

    public override async Task<NumericResponse> MatchRegisterFinishAsync(MatchRegisterFinishRequest request, BlazeRpcContext context)
    {
        return new NumericResponse
        {
        };
    }

    public override async Task<ChangePlayersResponse> ChangePlayersAsync(ChangePlayersRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
        
        foreach (var loopVar in request.mCardDataList)
        {
            CardData cardData = (await HutManager.GetCard(loopVar.mCardId)).Card;
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
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;

        switch (request.mMatchResult)
        {
            case MatchResult.CARDHOUSE_MATCHRESULT_INVALID: break;
            case MatchResult.CARDHOUSE_MATCHRESULT_WON: await HutHelper.IncrementGeneralInfo(userId, HutHelper.Outcome.WIN); break;
            case MatchResult.CARDHOUSE_MATCHRESULT_LOST: await HutHelper.IncrementGeneralInfo(userId, HutHelper.Outcome.LOSS); break;
            case MatchResult.CARDHOUSE_MATCHRESULT_DRAW: await HutHelper.IncrementGeneralInfo(userId, HutHelper.Outcome.OTL); break;
            case MatchResult.CARDHOUSE_MATCHRESULT_WON_CUP: await HutHelper.IncrementGeneralInfo(userId, HutHelper.Outcome.WIN); break;
            case MatchResult.CARDHOUSE_MATCHRESULT_LOST_CUP: await HutHelper.IncrementGeneralInfo(userId, HutHelper.Outcome.LOSS); break;
            default: throw new NotImplementedException();
        }
        
        if (request.mState == PlayGameState.CARDHOUSE_PGSTATE_ENDING)
        {
            await HutHelper.Deposit(userId, request.mCredits);
        }

        var generalInfo = await HutManager.GetGeneralInfo(userId);
        var versionInfo = await HutManager.GetVersionInfo(userId);
        
        return new PlayGameResponse
        {
            mBonusAwarded = 1,
            mCredits = generalInfo.Value.mCredits,
            mGoldenTickets = request.mGoldenTickets,
            mPrestige = request.mPrestige,
            mTrophyCardCreated = (byte)(request.mMatchResult == MatchResult.CARDHOUSE_MATCHRESULT_WON_CUP ? 1 : 0),
            mVersionInfo = versionInfo.Value
        };
    }


    public override async Task<SquadLoadActiveResponse> SquadLoadActiveAsync(SquadLoadActiveRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
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
        var userId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
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