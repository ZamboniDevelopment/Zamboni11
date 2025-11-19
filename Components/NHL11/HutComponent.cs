using System;
using System.Threading.Tasks;
using Blaze3SDK.Blaze.Example;
using BlazeCommon;
using Zamboni11.Components.NHL11.Bases;
using Zamboni11.Components.NHL11.Requests;
using Zamboni11.Components.NHL11.Responses;

namespace Zamboni11.Components.NHL11;

internal class HutComponent : HutComponentBase.Server
{
    // public override Task<HutLoginResponse> LoginRequestAsync(LoginRequest request, BlazeRpcContext context)
    // {
    //     return Task.FromResult(new HutLoginResponse
    //     {
    //         //RETURN DATA IF TEAM EXISTS
    //         mABBR = "",
    //         mBNUS = Char.MaxValue,
    //         mName = "",
    //         mRWRD = Char.MaxValue,
    //         mTNOW = 0,
    //         mUID = 0
    //     });
    // }
    
    // public override Task<GamerGetInfoResponse> GetGamerInfoRequestAsync(GamerGetInfoRequest request, BlazeRpcContext context)
    // {
    //     return Task.FromResult(new GamerGetInfoResponse
    //     {
    //         mINFO = default,
    //         mUID = 0
    //     });
    // }
    //
    // public override Task<DeckInfoResponse> GetDeckInfoAsync(DeckInfoRequest request, BlazeRpcContext context)
    // {
    //     return Task.FromResult(new DeckInfoResponse
    //     {
    //         mPersona = null,
    //         mUID = 0,
    //         mVER = default
    //     });
    // }
    //
    // public override Task<ExampleResponse> CreatePackAsync(CreatePackRequest request, BlazeRpcContext context)
    // {
    //     return Task.FromResult(new ExampleResponse
    //     {
    //         mMessage = "null",
    //     });
    // }
    
}