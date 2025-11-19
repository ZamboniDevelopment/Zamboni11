using System.Threading.Tasks;
using Blaze3SDK.Blaze.GameReportingLegacy;
using Blaze3SDK.Components;
using BlazeCommon;

namespace Zamboni11.Components.Blaze;

internal class GameReportingLegacyComponent : GameReportingLegacyComponentBase.Server
{
    public override Task<NullStruct> SubmitGameReportAsync(GameReport request, BlazeRpcContext context)
    {
        if (Program.Database.isEnabled) Program.Database.InsertReport(request);
        return Task.FromResult(new NullStruct());
    }
}