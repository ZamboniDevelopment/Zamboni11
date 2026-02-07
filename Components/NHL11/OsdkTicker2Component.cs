using System.Collections.Generic;
using System.Threading.Tasks;
using BlazeCommon;
using Zamboni11.Components.NHL11.Bases;
using Zamboni11.Components.NHL11.Requests;
using Zamboni11.Components.NHL11.Responses;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11;

internal class OsdkTicker2Component : OsdkTicker2ComponentBase.Server
{
    public override Task<RegisterResponse> RegisterArgsAsync(RegisterArgs request, BlazeRpcContext context)
    {
        return Task.FromResult(new RegisterResponse()
        {
            mNumMessages = 1
        });
    }

    public override Task<GetMessagesResponse> GetMessagesAsync(GetMessagesRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new GetMessagesResponse()
        {
            mData = new List<TickerMessage>
            {
                new TickerMessage
                {
                    mData = new List<string>
                    {
                        "Join Zamboni.gg/discord"
                    },
                    mENDT = 100,
                    mFilterIndex = 100,
                    mBlazeId = 100,
                    mPRIO = 100,
                    mPROV = "Kaap0",
                    mSTRT = 100,
                    mType = TickerMessageType.TYPE_NEWS
                }
            }
        });
    }
    
    public override Task<TickerFilter> UpdateFiltersAsync(UpdateFiltersRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new TickerFilter()
        {
            mBottom = request.mTickerFilter.mBottom,
            mTop = request.mTickerFilter.mTop
        });
    }
}