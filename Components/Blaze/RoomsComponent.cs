using System.Threading.Tasks;
using Blaze3SDK.Blaze.Example;
using Blaze3SDK.Blaze.Rooms;
using Blaze3SDK.Components;
using BlazeCommon;

namespace Zamboni11.Components.Blaze;

internal class RoomsComponent : RoomsComponentBase.Server
{
    public override Task<ExampleResponse> SelectViewUpdatesAsync(SelectViewUpdatesRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new ExampleResponse
        {
            mMessage = "ASD",
        });
    }

    public override Task<NullStruct> SelectCategoryUpdatesAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new NullStruct());
    }

    public override Task<NullStruct> ToggleJoinedRoomNotificationsAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new NullStruct());
    }
}