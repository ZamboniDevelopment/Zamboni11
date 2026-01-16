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
            mNumber = 1, //TODO UNKNOWN VALUES
        });
    }

    public override Task<MoveCardResponse> MoveCardAsync(MoveCardRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new MoveCardResponse
        {
        });
    }

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
            mNumber = 1
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
            mEscrowCount = 1,
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
                GetCard(29), GetCard(30), GetCard(35), GetCard(37), GetCard(76), GetCard(99), GetCard(158), GetStadiumCard(1)
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
                mPhysioArmBonus = 11,
                mPhysioBackBonus = 11,
                mContractBonus = 11,
                mFitnessBonus = 11,
                mPhysioFootBonus = 11,
                mGKDivingBonus = 11,
                mGKHandlingBonus = 11,
                mGKKickingBonus = 11,
                mGKOneOnOneBonus = 11,
                mGKPositioningBonus = 11,
                mGKReflexesBonus = 11,
                mPhysioHeadBonus = 11,
                mPhysioHipBonus = 11,
                mPhysioLegBonus = 11,
                mDefendingBonus = 11,
                mDribblingBonus = 1,
                mHeadingBonus = 11,
                mPaceBonus = 11,
                mPassingBonus = 11,
                mShootingBonus = 11,
                mPhysioShoulderBonus = 11,
                mManagerTalkBonus = 11
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

    public override Task<NumericResponse> ResetUserRequestAsync(ProvidedUID request, BlazeRpcContext context)
    {
        return Task.FromResult(new NumericResponse
        {
            mNumber = 1,
        });
    }

    public override Task<SquadListResponse> GetSquadListAsync(ProvidedUID request, BlazeRpcContext context)
    {
        return Task.FromResult(new SquadListResponse
        {
            // mACTV = 1,
            // mSQDS = new List<SQDS>
            // {
            //     new SQDS
            //     {
            //         mCHEM = 1,
            //         mFORM = 1,
            //         mRTNG = 1,
            //         mSQID = 1,
            //         mSQNM = "aaaaaaa"
            //     }
            // }
        });
    }

    public override Task<ViewCardsResponse> ViewCardsAsync(ViewCardsRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new ViewCardsResponse
        {
            mCardDataList = new List<CardData>
            {
            }
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
            mManager = HutManager.CardData[request.mManager],
            mSquadName = request.mSquadName,
            mPlayers = request.mPlayers.Select(variable => HutManager.CardData[variable]).ToList(),
            mStarRating = request.mStarRating,
            mSquadId = request.mSquadId
        };
        return Task.FromResult(new SquadSaveResponse
        {
            mSQID = request.mSquadId
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
                    mContextTypeId = 0,
                    mContextValue = 0,
                    mTypeId = 0,
                    mValue = 0
                },
                new StickerBookStatResult
                {
                    mContextTypeId = 2,
                    mContextValue = 2,
                    mTypeId = 2,
                    mValue = 2
                },
                new StickerBookStatResult
                {
                    mContextTypeId = 3,
                    mContextValue = 3,
                    mTypeId = 3,
                    mValue = 3
                },
                new StickerBookStatResult
                {
                    mContextTypeId = 4,
                    mContextValue = 4,
                    mTypeId = 4,
                    mValue = 4
                },
                new StickerBookStatResult
                {
                    mContextTypeId = 5,
                    mContextValue = 5,
                    mTypeId = 5,
                    mValue = 5
                },
                new StickerBookStatResult
                {
                    mContextTypeId = 6,
                    mContextValue = 6,
                    mTypeId = 6,
                    mValue = 6
                },
                new StickerBookStatResult
                {
                    mContextTypeId = 6,
                    mContextValue = 6,
                    mTypeId = 6,
                    mValue = 6
                },
                new StickerBookStatResult
                {
                    mContextTypeId = 2,
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
                GetCard(29), GetCard(30), GetCard(35), GetCard(37), GetCard(76), GetCard(99), GetCard(158), GetStadiumCard(1)
            }
        });
    }

    //
    // public override Task<SquadLoadActiveResponse> SquadLoadActiveAsync(SquadLoadActiveRequest request, BlazeRpcContext context)
    // {
    //     return Task.FromResult(new SquadLoadActiveResponse
    //     {
    //         mActiveCards = new List<CardData>
    //         {
    //             GetCard(29), GetCard(30), GetCard(35), GetCard(37), GetCard(76), GetCard(99), GetCard(158), GetArenaCard(1)
    //         },
    //         mSquadInfo = new SquadInfo
    //         {
    //             mChemistry = 10,
    //             mCHNG = 0,
    //             mFormationId = 10,
    //             mLines = null,
    //             mManager = default,
    //             mSquadName = null,
    //             mPlayers = null,
    //             mStarRating = 0,
    //             mSquadId = 0
    //         },
    //         mTargetUserId = 0
    //     });
    // }
    //
    //

    //

    //

    //
    public override Task<CreatePackResponse> CreatePackAsync(CreatePackRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new CreatePackResponse
        {
            mCardDataList = new List<CardData>
            {
                GetCard(29)
            },

            mDuplicateCardIdPairList = new List<CardIdPair>
            {
                new CardIdPair
                {
                    mCardId = 1,
                    mDuplicateCardId = 1
                }
            },
            mNumCards = 1,
            mPCNT = 1,
            mPKTY = 1,
            mVersionInfo = HutManager.GetHutPlayerInstance(context.BlazeConnection).GetVersionInfo()
        });
    }

    public static uint _syncCounter;


    //
    public static CardData GetCard(uint val)
    {
        return new CardData
        {
            mAttributes = new List<byte>
            {
                51, 52, 53, 54, 55
            },
            mCardStateId = 1,
            mCardId = val,
            mDatabaseId = val,
            mFormationId = 2,
            mFREE = 0,
            mFitness = 2,
            mInjuryGames = 0,
            mInjuryType = 0,
            mMoral = 1,
            mNumberOfOwners = 1,
            mPreferredPositionId = 1,
            mDiscardPrice = 2,
            mRareFlag = 1,
            mRating = 2,
            mSalaryCap = 1,
            mListStats = new List<byte>
            {
                1, //Games Played 
                62, //Goals 
                63, //Assists 
                64, //Plus/Minus
                65, //Penalty Minutes
                66,
                67,
                68,
                69,
                70
            },
            mCardTypeId = CardType.PLAYER,
            mDateIssued = 1767011983,
            mTeamId = 0,
            mListTrainingCards = new List<byte>
            {
                0, 72, 73, 74, 75, 78, 79, 255
            },
            mUsesRemaining = 5
        };
    }

    public static CardData GetStadiumCard(uint val)
    {
        return new CardData
        {
            mAttributes = new List<byte>
            {
                1,2,3,4,5,6,7,8,9,10
            },
            mCardStateId = 0,
            mCardId = 100,
            mDatabaseId = 10,
            mFormationId = 1,
            mFREE = 1,
            mFitness = 1,
            mInjuryGames = 1,
            mInjuryType = 1,
            mMoral = 1,
            mNumberOfOwners = 1,
            mPreferredPositionId = 1,
            mDiscardPrice = 1,
            mRareFlag = 1,
            mRating = 1,
            mSalaryCap = 1,
            mListStats = new List<byte>
            {
                1,2,3,4,5,6,7,8,9,10
            },
            mCardTypeId = CardType.HEAD_MANAGER,
            mDateIssued = 1,
            mTeamId = 1,
            mListTrainingCards = new List<byte>
            {
                1,2,3,4,5,6,7,8,9,10
            },
            mUsesRemaining = 1
        };
    }
}