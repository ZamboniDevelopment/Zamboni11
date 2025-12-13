using System.Collections.Generic;
using System.Threading.Tasks;
using Blaze3SDK.Blaze.Example;
using BlazeCommon;
using Zamboni11.Components.NHL11.Bases;
using Zamboni11.Components.NHL11.Requests;
using Zamboni11.Components.NHL11.Responses;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11;

internal class EAHockeyLeagueComponent : EAHockeyLeagueComponentBase.Server
{
    public override Task<GetSeasonConfigurationResponse> SeasonConfigurationRequestAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new GetSeasonConfigurationResponse
        {
            mCFGL = new List<SeasonConfiguration>()
            {
                {
                    new SeasonConfiguration
                    {
                        mDIVL = new List<DIVL>
                        {
                            new DIVL
                            {
                                mNUM = 1,
                                mSTAT = 21,
                                mTournamentRule = TournamentRule.SEASONALPLAY_TOURNAMENTRULE_UNLIMITED
                            },
                            new DIVL
                            {
                                mNUM = 2,
                                mSTAT = 21,
                                mTournamentRule = TournamentRule.SEASONALPLAY_TOURNAMENTRULE_ONE_ATTEMPT
                            }
                        },


                        mLeagueID = 1,
                        mLeagueName = "1",
                        mMemberType = MemberType.SEASONALPLAY_MEMBERTYPE_USER,
                        mSeasonID = 1,
                        mStatPeriodEnum = StatPeriod.STAT_PERIOD_ALLTIME,
                        mTeamID = 1
                    }
                },
                new SeasonConfiguration
                {
                    mDIVL = new List<DIVL>
                    {
                        new DIVL
                        {
                            mNUM = 2,
                            mSTAT = 22,
                            mTournamentRule = TournamentRule.SEASONALPLAY_TOURNAMENTRULE_UNLIMITED
                        },
                        new DIVL
                        {
                            mNUM = 3,
                            mSTAT = 21,
                            mTournamentRule = TournamentRule.SEASONALPLAY_TOURNAMENTRULE_ONE_ATTEMPT
                        }
                    },


                    mLeagueID = 1,
                    mLeagueName = "1",
                    mMemberType = MemberType.SEASONALPLAY_MEMBERTYPE_USER,
                    mSeasonID = 3,
                    mStatPeriodEnum = StatPeriod.STAT_PERIOD_ALLTIME,
                    mTeamID = 4
                }
            }
        });
    }

    public override Task<SeasonDetailsResponse> SeasonDetailsRequestAsync(SeasonDetailsRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new SeasonDetailsResponse
        {
            mNRST = 10,
            mPET = 110,
            mPST = 1230,
            mRET = 12320,
            mRST = 4210,
            mSeasonID = request.mSeasonId,
            mSeasonNumber = request.mSeasonId,
            MSeasonState = SeasonState.SEASONALPLAY_SEASON_STATE_PLAYOFF
        });
    }
}