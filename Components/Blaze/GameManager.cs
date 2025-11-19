using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Blaze3SDK.Blaze.GameManager;
using Blaze3SDK.Components;
using BlazeCommon;
using NLog;
using Zamboni;
using Timer = System.Timers.Timer;

namespace Zamboni11.Components.Blaze;

internal class GameManager : GameManagerBase.Server
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static readonly Timer Timer;

    static GameManager()
    {
        Timer = new Timer(2000);
        Timer.Elapsed += OnTimedEvent;
        Timer.AutoReset = true;
        Timer.Enabled = true;
    }

    private static void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        if (ServerManager.GetQueuedPlayers().Count <= 1) return;

        var grouped = ServerManager.GetQueuedPlayers().GroupBy(u => u.StartMatchmakingRequest.mCriteriaData.mGenericRulePrefsList.Find(prefs => prefs.mRuleName.Equals("OSDK_gameMode")).mDesiredValues[0]);

        foreach (var group in grouped)
        {
            var users = group.ToList();

            while (users.Count >= 2)
            {
                var queuedPlayerA = users[0];
                var queuedPlayerB = users[1];

                users.RemoveRange(0, 2);
                ServerManager.RemoveQueuedPlayer(queuedPlayerA);
                ServerManager.RemoveQueuedPlayer(queuedPlayerB);

                SendToMatchMakingGame(queuedPlayerA, queuedPlayerB, queuedPlayerA.StartMatchmakingRequest, group.Key);
            }
        }
    }

    private static void SendToMatchMakingGame(QueuedPlayer host, QueuedPlayer notHost, StartMatchmakingRequest startMatchmakingRequest, string gameMode)
    {
        var zamboniGame = new ServerGame(host.ServerPlayer, startMatchmakingRequest, gameMode);

        zamboniGame.AddGameParticipant(host.ServerPlayer, host.MatchmakingSessionId);
        zamboniGame.AddGameParticipant(notHost.ServerPlayer, notHost.MatchmakingSessionId);
    }

    public override Task<StartMatchmakingResponse> StartMatchmakingAsync(StartMatchmakingRequest request, BlazeRpcContext context)
    {
        var serverPlayer = ServerManager.GetServerPlayer(context.BlazeConnection);

        var queuedPlayer = new QueuedPlayer(serverPlayer, request);
        ServerManager.AddQueuedPlayer(queuedPlayer);

        return Task.FromResult(new StartMatchmakingResponse
        {
            mSessionId = queuedPlayer.MatchmakingSessionId
        });
    }

    public override Task<NullStruct> CancelMatchmakingAsync(CancelMatchmakingRequest request, BlazeRpcContext context)
    {
        var serverPlayer = ServerManager.GetServerPlayer(context.BlazeConnection);
        var queuedPlayer = ServerManager.GetQueuedPlayer(serverPlayer);
        if (queuedPlayer != null) ServerManager.RemoveQueuedPlayer(queuedPlayer);
        return Task.FromResult(new NullStruct());
    }

    public override Task<NullStruct> FinalizeGameCreationAsync(UpdateGameSessionRequest request, BlazeRpcContext context)
    {
        var serverGame = ServerManager.GetServerGame(request.mGameId);
        if (serverGame == null) return Task.FromResult(new NullStruct());

        var replicatedGameData = serverGame.ReplicatedGameData;
        replicatedGameData.mXnetNonce = request.mXnetNonce;
        replicatedGameData.mXnetSession = request.mXnetSession;

        serverGame.ReplicatedGameData = replicatedGameData;

        foreach (var serverPlayer in serverGame.ServerPlayers)
            NotifyGameSessionUpdatedAsync(serverPlayer.BlazeServerConnection, new GameSessionUpdatedNotification
            {
                mGameId = request.mGameId,
                mXnetNonce = request.mXnetNonce,
                mXnetSession = request.mXnetSession
            });
        return Task.FromResult(new NullStruct());
    }

    public override Task<NullStruct> AdvanceGameStateAsync(AdvanceGameStateRequest request, BlazeRpcContext context)
    {
        var serverGame = ServerManager.GetServerGame(request.mGameId);
        if (serverGame == null) return Task.FromResult(new NullStruct());

        var replicatedGameData = serverGame.ReplicatedGameData;
        replicatedGameData.mGameState = request.mNewGameState;

        serverGame.ReplicatedGameData = replicatedGameData;

        foreach (var serverPlayer in serverGame.ServerPlayers)
            NotifyGameStateChangeAsync(serverPlayer.BlazeServerConnection, new NotifyGameStateChange
            {
                mGameId = request.mGameId,
                mNewGameState = request.mNewGameState
            });
        return Task.FromResult(new NullStruct());
    }


    public override Task<NullStruct> SetPlayerAttributesAsync(SetPlayerAttributesRequest request, BlazeRpcContext context)
    {
        var zamboniGame = ServerManager.GetServerGame(request.mGameId);

        foreach (var serverPlayer in zamboniGame.ServerPlayers)
            NotifyPlayerAttribChangeAsync(serverPlayer.BlazeServerConnection, new NotifyPlayerAttribChange
            {
                mGameId = zamboniGame.ReplicatedGameData.mGameId,
                mPlayerAttribs = request.mPlayerAttributes,
                mPlayerId = request.mPlayerId
            });

        return Task.FromResult(new NullStruct());
    }

    public override Task<NullStruct> UpdateMeshConnectionAsync(UpdateMeshConnectionRequest request, BlazeRpcContext context)
    {
        var serverGame = ServerManager.GetServerGame(request.mGameId);
        if (serverGame == null) return Task.FromResult(new NullStruct());

        foreach (var playerConnectionStatus in request.mMeshConnectionStatusList)
            switch (playerConnectionStatus.mPlayerNetConnectionStatus)
            {
                case PlayerNetConnectionStatus.CONNECTED:
                {
                    var statePacket = new NotifyGamePlayerStateChange
                    {
                        mGameId = request.mGameId,
                        mPlayerId = playerConnectionStatus.mTargetPlayer,
                        mPlayerState = PlayerState.ACTIVE_CONNECTED
                    };
                    serverGame.NotifyParticipants(statePacket);

                    var joinCompletedPacket = new NotifyPlayerJoinCompleted
                    {
                        mGameId = request.mGameId,
                        mPlayerId = playerConnectionStatus.mTargetPlayer
                    };
                    serverGame.NotifyParticipants(joinCompletedPacket);
                    break;
                }
                case PlayerNetConnectionStatus.ESTABLISHING_CONNECTION:
                {
                    var statePacket = new NotifyGamePlayerStateChange
                    {
                        mGameId = request.mGameId,
                        mPlayerId = playerConnectionStatus.mTargetPlayer,
                        mPlayerState = PlayerState.ACTIVE_CONNECTING
                    };
                    serverGame.NotifyParticipants(statePacket);
                    break;
                }
                case PlayerNetConnectionStatus.DISCONNECTED:
                {
                    var serverPlayer = ServerManager.GetServerPlayer((uint)playerConnectionStatus.mTargetPlayer);
                    serverGame.RemoveGameParticipant(serverPlayer);
                    break;
                }
                default:
                    Logger.Debug("Unknown player connection status: " + playerConnectionStatus.mPlayerNetConnectionStatus);
                    break;
            }

        return Task.FromResult(new NullStruct());
    }

    public override Task<NullStruct> RemovePlayerAsync(RemovePlayerRequest request, BlazeRpcContext context)
    {
        var serverGame = ServerManager.GetServerGame(request.mGameId);
        var serverPlayer = ServerManager.GetServerPlayer((uint)request.mPlayerId);

        if (serverGame == null || serverPlayer == null) return Task.FromResult(new NullStruct());

        serverGame.RemoveGameParticipant(serverPlayer);
        return Task.FromResult(new NullStruct());
    }

    public override Task<NullStruct> UpdateGameSessionAsync(UpdateGameSessionRequest request, BlazeRpcContext context)
    {
        var serverGame = ServerManager.GetServerGame(request.mGameId);
        if (serverGame == null) return Task.FromResult(new NullStruct());

        var replicatedGameData = serverGame.ReplicatedGameData;
        replicatedGameData.mXnetNonce = request.mXnetNonce;
        replicatedGameData.mXnetSession = request.mXnetSession;

        serverGame.ReplicatedGameData = replicatedGameData;

        foreach (var serverPlayer in serverGame.ServerPlayers)
            NotifyGameSessionUpdatedAsync(serverPlayer.BlazeServerConnection, new GameSessionUpdatedNotification
            {
                mGameId = request.mGameId,
                mXnetNonce = request.mXnetNonce,
                mXnetSession = request.mXnetSession
            });
        return Task.FromResult(new NullStruct());
    }


    public override Task<NullStruct> SetGameSettingsAsync(SetGameSettingsRequest request, BlazeRpcContext context)
    {
        var serverGame = ServerManager.GetServerGame(request.mGameId);
        if (serverGame == null) return Task.FromResult(new NullStruct());

        var replicatedGameData = serverGame.ReplicatedGameData;
        replicatedGameData.mGameSettings = request.mGameSettings;

        serverGame.ReplicatedGameData = replicatedGameData;

        foreach (var serverPlayer in serverGame.ServerPlayers)
            NotifyGameSettingsChangeAsync(serverPlayer.BlazeServerConnection, new NotifyGameSettingsChange
            {
                mGameSettings = request.mGameSettings,
                mGameId = request.mGameId
            });
        return Task.FromResult(new NullStruct());
    }

    // https://github.com/PocketRelay/Server/issues/59
    public static ulong GetGameProtocolVersionHash(string protocolVersion)
    {
        protocolVersion ??= string.Empty;
        //FNV1 HASH - the same hashing logic is used in ea blaze for game protocol versions
        var buf = Encoding.UTF8.GetBytes(protocolVersion);
        var hash = 2166136261UL;
        foreach (var c in buf)
            hash = (hash * 16777619) ^ c;
        return hash;
    }
}