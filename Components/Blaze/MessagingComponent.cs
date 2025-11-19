using System.Threading.Tasks;
using Blaze3SDK.Components;
using BlazeCommon;

namespace Zamboni11.Components.Blaze;

internal class MessagingComponent : MessagingComponentBase.Server
{
    public override Task<NullStruct> FetchMessagesAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new NullStruct());
    }
}