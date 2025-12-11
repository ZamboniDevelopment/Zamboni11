using Blaze3SDK.Blaze.GameManager;

namespace Zamboni11;

public class QueuedPlayer
{
    private static uint _nextId = 1;

    public QueuedPlayer(ServerPlayer serverPlayer, StartMatchmakingRequest startMatchmakingRequest)
    {
        ServerPlayer = serverPlayer;
        MatchmakingSessionId = _nextId++;
        StartMatchmakingRequest = startMatchmakingRequest;
    }

    public ServerPlayer ServerPlayer { get; }
    public uint MatchmakingSessionId { get; }
    public StartMatchmakingRequest StartMatchmakingRequest { get; }
}