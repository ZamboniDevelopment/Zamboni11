using System.Collections.Generic;
using System.Threading.Tasks;
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
            mInstanceConfigList = new List<SeasonConfiguration>()
            {
                {
                    new SeasonConfiguration
                    {
                        mDivisionList = new List<Division>
                        {
                            new Division
                            {
                                mNumber = 1,
                                mSize = 21,
                                mTournamentRule = TournamentRule.SEASONALPLAY_TOURNAMENTRULE_UNLIMITED
                            },
                            new Division
                            {
                                mNumber = 2,
                                mSize = 21,
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
                    mDivisionList = new List<Division>
                    {
                        new Division
                        {
                            mNumber = 2,
                            mSize = 22,
                            mTournamentRule = TournamentRule.SEASONALPLAY_TOURNAMENTRULE_UNLIMITED
                        },
                        new Division
                        {
                            mNumber = 3,
                            mSize = 21,
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

    public override Task<SeasonDetails> SeasonDetailsRequestAsync(SeasonDetailsRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new SeasonDetails
        {
            mNextRegularSeasonStart = 10,
            mPlayOffEnd = 110,
            mPlayOffStart = 1230,
            mRegularSeasonEnd = 12320,
            mRegularSeasonStart = 4210,
            mSeasonID = request.mSeasonId,
            mSeasonNumber = request.mSeasonId,
            mSeasonState = SeasonState.SEASONALPLAY_SEASON_STATE_PLAYOFF
        });
    }
}