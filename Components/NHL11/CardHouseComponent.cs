using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaze3SDK;
using Blaze3SDK.Blaze.Example;
using BlazeCommon;
using Zamboni11.Components.NHL11.Bases;
using Zamboni11.Components.NHL11.Requests;
using Zamboni11.Components.NHL11.Responses;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11;

internal class CardHouseComponent : CardHouseComponentBase.Server
{
    public override Task<LoginResponse> LoginRequestAsync(LoginRequest request, BlazeRpcContext context)
    {
        var hutPlayerInstance = HutManager.GetHutPlayerInstance(context.BlazeConnection);
        if (hutPlayerInstance == null) return Task.FromResult(new LoginResponse());
        return Task.FromResult(new LoginResponse
        {
            mTeamAbbreviation = hutPlayerInstance.GamerInfo.mTeamAbbreviation,
            mBonusAwarded = 1, //TODO UNKNOWN
            mTeamName = hutPlayerInstance.GamerInfo.mTeamName,
            mRewardType = 0, //TODO UNKNOWN
            mRewardValue = 10, //TODO UNKNOWN
            mUserId = 0 //TODO USE 0 FOR NOW FOR EVERYONE BECAUSE CLIENT SEEMS TO NOT "KNOW" HIS UID
        });
    }

    public override Task<NumericResponse> LogoutRequestAsync(LogoutRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new NumericResponse
        {
            mNumber = 0,
        });
    }

    // public override Task<MoveCardResponse> MoveCardAsync(MoveCardRequest request, BlazeRpcContext context)
    // {
    //     return Task.FromResult(new MoveCardResponse
    //     {
    //     });
    // }

    public override Task<GamerGetInfoResponse> GetGamerInfoRequestAsync(GamerGetInfoRequest request, BlazeRpcContext context)
    {
        var hutPlayerInstance = HutManager.GetHutPlayerInstance(context.BlazeConnection);
        if (hutPlayerInstance == null) throw new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_NO_PLAYER_INFO);
        return Task.FromResult(new GamerGetInfoResponse
        {
            mGamerInfo = hutPlayerInstance.GamerInfo,
            mUserId = 0 //TODO USE 0 FOR NOW FOR EVERYONE BECAUSE CLIENT SEEMS TO NOT "KNOW" ITS UID
        });
    }

    public override Task<NumericResponse> SetGamerInfoRequestAsync(GamerSetInfoRequest request, BlazeRpcContext context)
    {
        var serverPlayer = ServerManager.GetServerPlayer(context.BlazeConnection);
        var hutPlayerInstance = HutManager.GetHutPlayerInstance(context.BlazeConnection);
        if (hutPlayerInstance == null)
        {
            HutManager.AddHutPlayerInstance(serverPlayer.UserIdentification.mAccountId, new HutPlayerInstance(request.mGamerInfo));
        }
        else
        {
            hutPlayerInstance.GamerInfo = request.mGamerInfo;
        }

        return Task.FromResult(new NumericResponse
        {
            mNumber = 0
        });
    }

    public override Task<DeckInfoResponse> GetDeckInfoAsync(DeckInfoRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new DeckInfoResponse
        {
            mDuplicateEscrowCardIdPairList = new List<CardIdPair>
            {
            },
            mDuplicateUnassignedCardIdPairList = new List<CardIdPair>
            {
            },
            mEscrowCardDataList = new List<CardData>
            {
            },
            mEscrowCount = 0,
            mGeneralInfo = new GeneralInfo
            {
                mCredits = 100, //TODO This is EA pucks
                mStats = new List<byte>
                {
                    6, 7, 10, 30, 50, 60, 80
                }
            },
            mTeamRating = 3,
            mUnassignedCardDataList = new List<CardData> //TODO These are the cards shown to the user.
            {
                // GetOrCreateCard(29), GetOrCreateCard(30), GetOrCreateCard(35), GetOrCreateCard(37), GetOrCreateCard(76), GetOrCreateCard(99), GetOrCreateCard(158),
            },
            mUserId = 0,
            mVersionInfo = HutManager.GetHutPlayerInstance(context.BlazeConnection).GetVersionInfo()
        });
    }

    public override Task<GetConfigResponse> GetConfigRequestAsync(ProvidedUID request, BlazeRpcContext context)
    {
        return Task.FromResult(new GetConfigResponse
        {
            mConfigList = new List<uint>
            {
            }
        });
    }

    //
    // public override Task<DiscardCardResponse> DiscardCardAsync(DiscardCardRequest request, BlazeRpcContext context)
    // {
    //     return Task.FromResult(new DiscardCardResponse
    //     {
    //         mCRED = 0,
    //         mVersion = GetVER()
    //     });
    // }
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

    public override Task<AssignCardsResponse> AssignCardsAsync(AssignCardsRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new AssignCardsResponse
        {
            mVersionInfo = HutManager.GetHutPlayerInstance(context.BlazeConnection).GetVersionInfo()
        });
    }


    public override Task<UserReliabilityInfoResponse> GetUserReliabilityInfoAsync(ProvidedUID request, BlazeRpcContext context)
    {
        return Task.FromResult(new UserReliabilityInfoResponse
        {
            mPreviousMatchUnfinished = 0,
            mMatchesFinished = 0,
            mMatchesStarted = 0,
            mReliability = 0,
            mUserId = 0
        });
    }
    //
    // public override Task<NumericResponse> ResetUserRequestAsync(ProvidedUID request, BlazeRpcContext context)
    // {
    //     return Task.FromResult(new NumericResponse
    //     {
    //         mNumber = 1,
    //     });
    // }

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

    public override Task<ViewCardsResponse> ViewCardsAsync(ViewCardsRequest request, BlazeRpcContext context)
    {
        //TODO LIMITED TO USERS OWN CARDS
        long userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        List<CardData> retList = new List<CardData>();
        foreach (var VARIABLE in HutManager.UserInventories[userId].Values.ToList())
        {
            if (request.mCardIdList.Contains(VARIABLE.mCardId))
            {
                retList.Add(VARIABLE);
            }
        }

        return Task.FromResult(new ViewCardsResponse
        {
            mCardDataList = retList
        });
    }

    public override Task<SquadSaveResponse> SquadSaveAsync(SquadSaveRequest request, BlazeRpcContext context)
    {
        long userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        var hutPlayerInstance = HutManager.GetHutPlayerInstance(context.BlazeConnection);
        if (hutPlayerInstance == null) throw new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_NO_PLAYER_INFO);
        List<CardData> retList = new();
        foreach (var VARIABLE in request.mPlayers)
        {
            if (VARIABLE == 0) continue;
            retList.Add(HutManager.UserInventories[userId][VARIABLE]);
        }

        hutPlayerInstance.SquadInfo = new SquadInfo
        {
            mChemistry = request.mChemistry,
            mFormationId = request.mFormation,
            mLines = request.mLines,
            mManager = HutManager.GetCard(userId, request.mManager),
            mSquadName = request.mSquadName,
            mPlayers = retList,
            mStarRating = request.mStarRating,
            mSquadId = request.mSquadId
        };
        return Task.FromResult(new SquadSaveResponse
        {
            mSquadId = request.mSquadId
        });
    }

    public override Task<StickerBookStats2Response> StickerBookStats2Async(StickerBookStats2Request request, BlazeRpcContext context)
    {
        return Task.FromResult(new StickerBookStats2Response
        {
            mStats = new List<StickerBookStatResult>
            {
                new StickerBookStatResult
                {
                    mContextId = request.mContextId,
                    mContextValue = 2,
                    mTypeId = 2,
                    mValue = 2
                },
            }
        });
    }

    public override Task<StickerBookSearchResponse> StickerBookSearchAsync(StickerBookSearchRequest request, BlazeRpcContext context)
    {
        List<CardData> retList = new List<CardData>();
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;

        //TODO Clean this horrendous mess
        switch (request.mCollectionSearchCardType)
        {
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_ALL:
            {
                retList = HutManager.UserInventories[userId].Values.ToList();
                break;
            }
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_HEADCOACH:
            {
                foreach (var VARIABLE in HutManager.UserInventories[userId].Values.ToList())
                {
                    if (VARIABLE.mCardSubTypeId.Equals(CardSubType.CARDHOUSE_CARD_TYPE_STAFF_HEADCOACH))
                    {
                        retList.Add(VARIABLE);
                    }
                }
                break;
            }
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_BADGE:
            {
                foreach (var VARIABLE in HutManager.UserInventories[userId].Values.ToList())
                {
                    if (VARIABLE.mCardSubTypeId.Equals(CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_BADGE))
                    {
                        retList.Add(VARIABLE);
                    }
                }
                break;
            }
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_STADIUM:
            {
                foreach (var VARIABLE in HutManager.UserInventories[userId].Values.ToList())
                {
                    if (VARIABLE.mCardSubTypeId.Equals(CardSubType.CARDHOUSE_CARD_TYPE_CUSTOM_STADIUM))
                    {
                        retList.Add(VARIABLE);
                    }
                }
                break;
            }
            case CollectionSearchType.COLLECTION_SEARCH_TYPE_DEVELOPMENT:
            {
                if (HutManager.UserInventories.TryGetValue(userId, out var inventory))
                {
                    foreach (var card in inventory.Values)
                    {
                        if (card.mCardSubTypeId >= (CardSubType)51 && card.mCardSubTypeId <= (CardSubType)62 || card.mCardSubTypeId == (CardSubType)201)
                        {
                            retList.Add(card);
                        }
                    }
                }
                break;
            }
            default:
            {
                throw new NotImplementedException();
            }
        }
        
        
        return Task.FromResult(new StickerBookSearchResponse
        {
            mSearchResults = retList
        });
    }
    // public override Task<ISWatchListResponse> ISWatchListAsync(ISWatchListRequest request, BlazeRpcContext context)
    // {
    //     throw new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_UNKNOWN);
    // }
    //
    // public override Task<ISViewTradeResponse> ISViewTradeAsync(ISViewTradeRequest request, BlazeRpcContext context)
    // {
    //     throw new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_UNKNOWN);
    // }

    // public override Task<ISRemoveWatchResponse> ISRemoveWatchAsync(ISRemoveWatchRequest request, BlazeRpcContext context)
    // {
    //     throw new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_UNKNOWN);
    // }
    
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
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        CardData cardData = HutManager.GetCard(userId, request.mTargetCards[0]);
        CardData cardDataB = HutManager.GetCard(userId, request.mCardId);
        cardData.mListTrainingCards[0] = (int)cardDataB.mCardDbId;
        HutManager.UserInventories[userId].TryAdd(request.mTargetCards[0],cardData);
        return Task.FromResult(new ApplyCardResponse
        {
            mCardId = request.mCardId,
            mCardDataList = new List<CardData>
            {
                cardData
            },
            mUserId = request.mUserId
        });
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
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        foreach (var VARIABLE in request.mCardDataList)
        {
            var cardData = HutManager.UserInventories[userId][VARIABLE.mCardId];
            cardData.mInjuryGames = VARIABLE.mInjuryGames;
            cardData.mInjuryType = VARIABLE.mInjuryType;
            cardData.mListStats = VARIABLE.mListStats;
            HutManager.UserInventories[userId].TryAdd(VARIABLE.mCardId, cardData);
        }
        return Task.FromResult(new ChangePlayersResponse
        {
            mVal = 0
        });
    }

    
    public override Task<PlayGameResponse> PlayGameAsync(PlayGameRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new PlayGameResponse
        {
            mBonusAwarded = 10,
            mCredits = 10,
            mGoldenTickets = 10,
            mPrestige = 10,
            mTrophyCardCreated = 10,
            mVersionInfo = default
        });
    }


    //
    public override Task<SquadLoadActiveResponse> SquadLoadActiveAsync(SquadLoadActiveRequest request, BlazeRpcContext context)
    {
        var userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        HutPlayerInstance hutPlayerInstance = HutManager.HutPlayerInstances[userId];
        List<CardData> retList = new();
        foreach (var VARIABLE in HutManager.UserInventories[userId].Values.ToList())
        {
            retList.Add(VARIABLE);
        }
        return Task.FromResult(new SquadLoadActiveResponse
        {
            mActiveCards = retList,
            mSquadInfo = hutPlayerInstance.SquadInfo,
            mTargetUserId = 0
        });
    }

    public override Task<CreatePackResponse> CreatePackAsync(CreatePackRequest request, BlazeRpcContext context)
    {
        long userId = ServerManager.GetServerPlayer(context.BlazeConnection).UserIdentification.mAccountId;
        return Task.FromResult(new CreatePackResponse
        {
            mCardDataList = new List<CardData>
            {
                HutCardFactory.CreateRandomJerseyCard(userId, true, false),
                HutCardFactory.CreateRandomJerseyCard(userId, false, false),
                HutCardFactory.CreateRandomLogoCard(userId),
                HutCardFactory.CreateRandomStadiumCard(userId),
                HutCardFactory.CreateRandomHeadCoachCard(userId),
                HutCardFactory.CreateRandomTrainingCard(userId),
                HutCardFactory.CreateRandomContractCard(userId),
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_GK).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_D).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_LW).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_RW).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C).Result,
                HutCardFactory.CreateRandomPlayerCard(userId, CardSubType.CARDHOUSE_CARD_TYPE_PLAYER_C).Result,
            },

            mDuplicateCardIdPairList = new List<CardIdPair>(),
            mNumCards = 0,
            mNumPackPurchased = 0,
            mRandPackType = 0,
            mVersionInfo = HutManager.GetHutPlayerInstance(context.BlazeConnection).GetVersionInfo()
        });
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