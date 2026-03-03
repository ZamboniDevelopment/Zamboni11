using System.Collections.Generic;
using System.Threading.Tasks;
using Blaze3SDK;
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
                                mSize = 1,
                                mTournamentRule = TournamentRule.SEASONALPLAY_TOURNAMENTRULE_ONE_ATTEMPT
                            },
                        },
            
            
                        mLeagueID = 1,
                        mLeagueName = "aaaaa",
                        mMemberType = MemberType.SEASONALPLAY_MEMBERTYPE_CLUB,
                        mSeasonID = 1,
                        mStatPeriodEnum = StatPeriod.STAT_PERIOD_ALLTIME,
                        mTeamID = 1
                    }
                },
            }
        });
    }

    public override Task<SeasonDetails> SeasonDetailsRequestAsync(SeasonDetailsRequest request, BlazeRpcContext context)
    {
        // throw new BlazeRpcException(Blaze3RpcError.OSDKSEASONALPLAY_ERR_SEASON_NOT_FOUND);
        return Task.FromResult(new SeasonDetails
        {
        mNextRegularSeasonStart = 2771187172,
        mPlayOffEnd = 1971187172,
        mPlayOffStart = 1871187172,
        mRegularSeasonEnd = 1791187172,
        mRegularSeasonStart = 1771187170,
        mSeasonID = request.mSeasonId,
        mSeasonNumber = request.mSeasonId,
        mSeasonState = SeasonState.SEASONALPLAY_SEASON_STATE_REGULARSEASON
        });
    }
}