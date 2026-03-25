using System.Threading.Tasks;
using Blaze3SDK.Blaze.GameManager;
using BlazeCommon;

namespace Zamboni11;

public class ZamboniCoreServer : BlazeServer
{
    public ZamboniCoreServer(BlazeServerConfiguration settings) : base(settings)
    {
    }

    public override Task OnProtoFireDisconnectAsync(ProtoFireConnection connection)
    {
        var serverPlayer = ServerManager.GetServerPlayerByConnectionId(connection.ID);
        if (serverPlayer == null) return base.OnProtoFireDisconnectAsync(connection);
        ServerManager.RemoveServerPlayerByUserId(serverPlayer.UserIdentification.mAccountId);

        var queuedPlayer = ServerManager.GetQueuedPlayer(serverPlayer);
        if (queuedPlayer != null) ServerManager.RemoveQueuedPlayerByUserId(queuedPlayer.ServerPlayer.UserIdentification.mAccountId);

        var serverGame = ServerManager.GetServerGame(serverPlayer);
        if (serverGame != null) serverGame.RemoveGameParticipant(serverPlayer, PlayerRemovedReason.PLAYER_CONN_LOST);

        return base.OnProtoFireDisconnectAsync(connection);
    }
}