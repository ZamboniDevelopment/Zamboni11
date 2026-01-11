using System.Threading.Tasks;
using Blaze3SDK.Blaze.Example;
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
        });
    }

    public override Task<NullStruct> GetInvitationsAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new NullStruct());
    }
}