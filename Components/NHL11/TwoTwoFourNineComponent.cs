using System.Threading.Tasks;
using BlazeCommon;
using Zamboni11.Components.NHL11.Bases;

namespace Zamboni11.Components.NHL11;

internal class TwoTwoFourNineComponent : TwoTwoFourNineComponentBase.Server
{
    public override Task<NullStruct> CommandOneAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new NullStruct());
    }

    public override Task<NullStruct> CommandTwoAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new NullStruct());
    }
}