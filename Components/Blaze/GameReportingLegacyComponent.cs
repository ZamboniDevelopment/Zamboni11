using System.Threading.Tasks;
using Blaze3SDK.Blaze.GameReportingLegacy;
using Blaze3SDK.Components;
using BlazeCommon;

namespace Zamboni11.Components.Blaze;

internal class GameReportingLegacyComponent : GameReportingLegacyComponentBase.Server
{
    public override async Task<NullStruct> SubmitGameReportAsync(GameReport request, BlazeRpcContext context)
    {
        var reporterUserId = ServerManager.GetServerPlayerByConnectionId(context.Connection.ID)!.UserIdentification.mAccountId;
        if (Program.Database.isEnabled) await Program.Database.InsertLegacyReport(request, reporterUserId);
        return new NullStruct();
    }
}