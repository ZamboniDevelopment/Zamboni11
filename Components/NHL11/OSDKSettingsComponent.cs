using System.Threading.Tasks;
using BlazeCommon;
using Zamboni11.Components.NHL11.Bases;
using Zamboni11.Components.NHL11.Responses;

namespace Zamboni11.Components.NHL11;

internal class OSDKSettingsComponent : OSDKSettingsComponentBase.Server
{
    public override Task<FetchSettingsResponse> FetchSettingsAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new FetchSettingsResponse());
    }

    public override Task<FetchSettingsGroupsResponse> FetchSettingsGroupsAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new FetchSettingsGroupsResponse());
    }
}