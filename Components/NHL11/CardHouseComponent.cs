using System.Collections.Generic;
using System.Threading.Tasks;
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
        return Task.FromResult(new LoginResponse
        {
            mTeamAbbreviation = "",
            mBNUS = 0,
            mTeamName = "",
            mRWRD = 0,
            mTNOW = 0,
            mUID = 0
        });
    }

    public override Task<NumericResponse> LogoutRequestAsync(LogoutRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new NumericResponse
        {
            mNUM = 1,
        });
    }


    //

    // public override Task<GamerGetInfoResponse> GetGamerInfoRequestAsync(GamerGetInfoRequest request, BlazeRpcContext context)
    // {
    //     HutPlayerInstance hutPlayerInstance = HutManager.HutPlayerInstances.Find(instance => instance.ServerPlayer.Equals(ServerManager.GetServerPlayer(context.Connection)));
    //     if (hutPlayerInstance == null) throw new Exception();
    //     return Task.FromResult(new GamerGetInfoResponse
    //     {
    //         MGamerInfo = hutPlayerInstance.GamerInfo,
    //         mUID = 0
    //     });
    // }
    public override Task<NumericResponse> SetGamerInfoRequestAsync(GamerSetInfoRequest request, BlazeRpcContext context)
    {
        HutManager.HutPlayerInstances.Add(new HutPlayerInstance(ServerManager.GetServerPlayer(context.BlazeConnection), request.mGamerInfo));
        return Task.FromResult(new NumericResponse
        {
            mNUM = 1
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
            mECNT = 1,
            mGeneralInfo = new GeneralInfo
            {
            },
            mRATE = 3,
            mUnassignedCardDataList = new List<CardData> //TODO These are the cards shown to the user.
            {
                GetCards(10)
            },
            mUID = 0,
            mVersionInfo = GetVER()
        });
    }

    public override Task<ConfigResponse> GetConfigRequestAsync(ProvidedUID request, BlazeRpcContext context)
    {
        return Task.FromResult(new ConfigResponse
        {
            mGCFL = new List<uint>
            {
                10
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
                mARM = 1,
                mBACK = 1,
                mCON = 1,
                mFIT = 1,
                mFOOT = 1,
                mGKD = 1,
                mGKH = 1,
                mGKK = 1,
                mGKO = 1,
                mGKP = 1,
                mGKR = 1,
                mHEAD = 1,
                mHIP = 1,
                mLEG = 1,
                mPDEF = 1,
                mPDR = 1,
                mPHE = 1,
                mPPAC = 1,
                mPPAS = 1,
                mPSH = 1,
                mSHOU = 1,
                mTALK = 1
            }
        });
    }

    public override Task<AssignCardsResponse> AssignCardsAsync(AssignCardsRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new AssignCardsResponse
        {
            mVersionInfo = GetVER()
        });
    }


    public override Task<UserReliabilityInfoResponse> GetUserReliabilityInfoAsync(ProvidedUID request, BlazeRpcContext context)
    {
        return Task.FromResult(new UserReliabilityInfoResponse
        {
            mDISC = 0,
            mMFI = 0,
            mMST = 0,
            mREL = 0,
            mUID = 0
        });
    }

    public override Task<NumericResponse> ResetUserRequestAsync(ProvidedUID request, BlazeRpcContext context)
    {
        return Task.FromResult(new NumericResponse
        {
            mNUM = 1,
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
                GetCards(10),
            }
        });
    }

    //
    public override Task<SquadSaveResponse> SquadSaveAsync(SquadSaveRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new SquadSaveResponse
        {
            mSQID = request.mSQID
        });
    }
    
    public override Task<StickerBookStats2Response> StickerBookStats2Async(StickerBookStats2Request request, BlazeRpcContext context)
    {
        return Task.FromResult(new StickerBookStats2Response
        {
            mStats = new List<Stats>
            {
                new Stats
                {
                    mCTTP = 2,
                    mCTVL = 2,
                    mTYPE = 2,
                    mVALU = 2
                }
            }
        });
    }
    //
    // public override Task<ConfigResponse> GetConfigRequestAsync(ProvidedUID request, BlazeRpcContext context)
    // {
    //     return Task.FromResult(new ConfigResponse
    //     {
    //         mGCFL = new List<uint>
    //         {
    //             10
    //         }
    //     });
    // }
    //

    //
    public override Task<StickerBookSearchResponse> StickerBookSearchAsync(StickerBookSearchRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new StickerBookSearchResponse
        {
            mSearchResults = new List<CardData>
            {
                // GetCards(10),GetCards(20),GetCards(30)
            }
        });
    }
    //
    // public override Task<SquadLoadActiveResponse> SquadLoadActiveAsync(SquadLoadActiveRequest request, BlazeRpcContext context)
    // {
    //     return Task.FromResult(new SquadLoadActiveResponse
    //     {
    //         mACTV = new List<Card>
    //         {
    //             GetCards(10)
    //         },
    //         mSquad = new Squad()
    //         {
    //             mCHEM = 1,
    //             mCHNG = 1,
    //             mFORM = 1,
    //             mLINE = new List<uint>
    //             {
    //                 10
    //             },
    //             mMNGR = GetCards(1),
    //             mNAME = "aaaaa",
    //             mPLRS = new List<Card>
    //             {
    //                 GetCards(1)
    //             },
    //             mRTNG = 1,
    //             mSQID = 0
    //         },
    //         mTUID = 0
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
                GetCards(1)
            },
    
            mDuplicateCardIdPairList = new List<CardIdPair>
            {
                new CardIdPair
                {
                    mCID = 1,
                    mDCID = 1
                }
            },
            mNUM = 1,
            mPCNT = 1,
            mPKTY = 1,
            mVersionInfo = GetVER()
        });
    }
    
    public static VersionInfo GetVER()
    {
        return new VersionInfo()
        {
            mVESC = 0,
            mVGEN = 0,
            mVUNA = 0
        };
    }

    //
    public static CardData GetCards(uint val)
    {
        return new CardData
        {
            mAttributes = new List<byte>
            {
                51, 52, 53, 54, 55
            },
            mCDST = 2,
            mCID = 3,
            mDatabaseId = val,
            mFORM = 2,
            mFREE = 1,
            mFTNS = 2,
            mINJG = 3,
            mINJT = 2,
            mMORL = 1,
            mNumberOfOwners = 4,
            mPOSI = 3,
            mPRIC = 2,
            mRARE = 1,
            mRTNG = 2,
            mSCAP = 4,
            mSTAT = new List<byte>
            {
                61, 62, 63, 64, 65
            },
            mSUB = 2,
            mDateIssued = 1767011983,
            mTMID = 1,
            mTRNG = new List<byte>
            {
                0, 72, 73, 74, 75, 78, 79, 255
            },
            mContractLenght = 5
        };
    }
}