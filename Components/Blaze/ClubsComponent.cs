using System.Threading.Tasks;
using Blaze3SDK.Components;
using BlazeCommon;

namespace Zamboni11.Components.Blaze;

internal class ClubsComponent : ClubsComponentBase.Server
{
    public override Task<NullStruct> GetClubsComponentSettingsAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new NullStruct());
    }
}