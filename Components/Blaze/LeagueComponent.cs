using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blaze3SDK;
using Blaze3SDK.Blaze.League;
using Blaze3SDK.Components;
using BlazeCommon;

namespace Zamboni11.Components.Blaze;

internal class LeagueComponent : LeagueComponentBase.Server
{
    public override Task<FindLeaguesResponse> GetLeaguesByUserAsync(GetLeaguesByUserRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new FindLeaguesResponse
        {
            mLeagues = new List<League>(LeagueManager.Leagues.Values)
        });
    }
    
    public override Task<GetNewsResponse> GetNewsAsync(GetNewsRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new GetNewsResponse
        {
            
        });
    }
    
    public override Task<Roster> GetRosterAsync(GetRosterRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new Roster
        {
        
        });
    }


    public override Task<NullStruct> GetInvitationsAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new NullStruct());
    }
    
    public override Task<GetMembersResponse> GetMembersAsync(GetMembersRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new GetMembersResponse
        {
            mMemberInfo = new List<MemberInfo>
            {
                LeagueManager.Members[ServerManager.GetServerPlayerByConnectionId(context.Connection.ID).UserIdentification.mExternalId]
            }
        });
    }

    
    
    public override Task<League> GetLeagueAsync(GetLeagueRequest request, BlazeRpcContext context)
    {
        if (LeagueManager.Leagues.TryGetValue(request.mLeagueId, out League league))
        {
            return Task.FromResult(league);
        }
        else
        {
            throw new BlazeRpcException(Blaze3RpcError.LEAGUE_ERR_INVALID_LEAGUEID);
        }
    }
    
    public override Task<CreateLeagueResponse> CreateLeagueAsync(CreateLeagueRequest request, BlazeRpcContext context)
    {
        uint id = LeagueManager.LeagueIdCounter++;
        ServerPlayer serverPlayer = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID);
        LeagueManager.Members.Add(serverPlayer.UserIdentification.mExternalId, new MemberInfo
        {
            mGamesRemaining = 0,
            mIsDraftProfileSubmitted = 0,
            mIsGM = 1,
            mIsOnline = 1,
            mIsStringMetadata = request.mIsStringMetadata,
            mLeagueId = id,
            mMember = new LeagueUser
            {
                mBlazeId = serverPlayer.UserIdentification.mBlazeId,
                mPersona = serverPlayer.UserIdentification.mName
            },
            mMetadata = request.mMetadata,
            mStats = new MemberStats
            {
                mGoalsAgainst = 0,
                mGoalsFor = 0,
                mLosses = 0,
                mPersona = serverPlayer.UserIdentification.mName,
                mPoints = 0,
                mRank = 0,
                mTies = 0,
                mWins = 0
            },
            mTeamId = request.mCreatorTeamId
        });
        LeagueManager.Leagues.Add(id, new League
        {
            mAbbrev = request.mAbbrev,
            mChampionNumWins = 0,
            mCreationTime = (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
            mCreator = new LeagueUser
            {
                mBlazeId = serverPlayer.UserIdentification.mBlazeId,
                mPersona = serverPlayer.UserIdentification.mName,
            },
            // mCurrChampion = default,
            mCurrRound = 1,
            mCurrSeason = 1,
            mDescription = request.mDescription,
            mGameTeamRoster = request.mGameTeamRoster,
            mIsGM = 1,
            mIsStringMetadata = request.mIsStringMetadata,
            mLastActiveTime = (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
            mLeagueFlags = request.mLeagueFlags,
            mLeagueId = id,
            mLeagueState = LeagueState.LEAGUE_STATE_REGULAR_SEASON,
            mLogo = request.mLogo,
            mMaxDNF = (short)request.mMaxDNF,
            mMaxMembers = request.mMaxMembers,
            mMetadata = request.mMetadata,
            mMinReputation = (short)request.mMinReputation,
            mName = request.mName,
            mNumGames = request.mNumGames,
            mNumMembers = 1,
            mNumPlayoffGames = request.mNumPlayoffGames,
            mNumRounds = request.mNumRounds,
            mNumberOfMembersOnline = 1,
            mOptions = request.mOptions,
            mPassword = request.mPassword,
            mPlayoffType = request.mPlayoffType,
            mScheduleType = request.mScheduleType,
            mTeamsInUse = new List<uint>
            {
                request.mCreatorTeamId
            },
            mTradeType = request.mTradeType,
            mTrophy = request.mTrophy
        });
        return Task.FromResult(new CreateLeagueResponse
        {
            mLeagueId = id
        });
    }
}