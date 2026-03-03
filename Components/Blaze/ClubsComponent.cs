using System.Threading.Tasks;
using Blaze3SDK.Blaze.Clubs;
using Blaze3SDK.Components;
using BlazeCommon;

namespace Zamboni11.Components.Blaze;

internal class ClubsComponent : ClubsComponentBase.Server
{
    public override Task<ClubsComponentSettings> GetClubsComponentSettingsAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new ClubsComponentSettings());
    }

    public override Task<GetInvitationsResponse> GetInvitationsAsync(GetInvitationsRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new GetInvitationsResponse());
    }

    public override Task<NullStruct> GetPetitionsAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new NullStruct());
    }
}