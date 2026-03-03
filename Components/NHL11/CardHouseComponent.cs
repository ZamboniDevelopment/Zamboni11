using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaze3SDK;
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

    // public override Task<NumericResponse> LogoutRequestAsync(LogoutRequest request, BlazeRpcContext context)
    // {
    //     return Task.FromResult(new NumericResponse
    //     {
    //         mNumber = 1, //TODO UNKNOWN VALUES
    //     });
    // }

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
        var hutPlayerInstance = HutManager.GetHutPlayerInstance(context.BlazeConnection);
        if (hutPlayerInstance == null)
        {
            HutManager.AddHutPlayerInstance(new HutPlayerInstance(ServerManager.GetServerPlayer(context.BlazeConnection), request.mGamerInfo));
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
        List<CardData> retlist = new List<CardData>();
    
        foreach (long cardId in request.mCardIdList)
        {
            retlist.Add(HutManager.GetCard(cardId));
        }
    
        return Task.FromResult(new ViewCardsResponse
        {
            mCardDataList = retlist
        });
    }

    public override Task<SquadSaveResponse> SquadSaveAsync(SquadSaveRequest request, BlazeRpcContext context)
    {
        var hutPlayerInstance = HutManager.GetHutPlayerInstance(context.BlazeConnection);
        if (hutPlayerInstance == null) throw new BlazeRpcException(Blaze3RpcError.CARDHOUSE_ERR_NO_PLAYER_INFO);
        hutPlayerInstance.SquadInfo = new SquadInfo
        {
            mChemistry = request.mChemistry,
            mFormationId = request.mFormation,
            mLines = request.mLines,
            // mManager = GetOrCreateCard(10000112),
            mSquadName = request.mSquadName,
            mPlayers = request.mPlayers.Select(variable => HutManager.GetCard((uint)variable)).ToList(),
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
        var hut = HutManager.GetHutPlayerInstance(context.BlazeConnection);
        return Task.FromResult(new StickerBookSearchResponse
        {
            mSearchResults = new List<CardData>
            {
                // GetOrCreateCard(29),
            }
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

    //
    public override Task<SquadLoadActiveResponse> SquadLoadActiveAsync(SquadLoadActiveRequest request, BlazeRpcContext context)
    {
        HutPlayerInstance hutPlayerInstance = HutManager.HutPlayerInstances[0];
        return Task.FromResult(new SquadLoadActiveResponse
        {
            mActiveCards = hutPlayerInstance.ActiveCards,
            mSquadInfo = hutPlayerInstance.SquadInfo,
            mTargetUserId = 0
        });
    }

    public override Task<CreatePackResponse> CreatePackAsync(CreatePackRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new CreatePackResponse
        {
            mCardDataList = new List<CardData>
            {
                HutCardFactory.CreateCard(2, CardType.CARDHOUSE_CARD_TYPE_PLAYER_GK),
                HutCardFactory.CreateRandomJerseyCard(true, false),
                HutCardFactory.CreateRandomJerseyCard(false, false),
                HutCardFactory.CreateRandomLogoCard(),
                HutCardFactory.CreateRandomStadiumCard(),
                HutCardFactory.CreateRandomHeadCoachCard(),
                HutCardFactory.CreateRandomTrainingCard(),
                HutCardFactory.CreateRandomContractCard(),
                
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
    //             mFitness = 2,
    //             mInjuryGames = 0,
    //             mInjuryType = 0,
    //             mMoral = 1,
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
    //             mCardTypeId = CardType.CARDHOUSE_CARD_TYPE_PLAYER_C,
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
    //             mFitness = 2,
    //             mInjuryGames = 0,
    //             mInjuryType = 0,
    //             mMoral = 1,
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
    //             mCardTypeId = CardType.CARDHOUSE_CARD_TYPE_CUSTOM_BADGE,
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
    //             mFitness = 2,
    //             mInjuryGames = 0,
    //             mInjuryType = 0,
    //             mMoral = 1,
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
    //             mCardTypeId = CardType.CARDHOUSE_CARD_TYPE_CUSTOM_KIT,
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