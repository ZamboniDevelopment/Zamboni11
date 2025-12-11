using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Blaze3SDK.Blaze;
using Blaze3SDK.Blaze.GameManager;
using Blaze3SDK.Components;
using BlazeCommon;
using NLog;
using Timer = System.Timers.Timer;

namespace Zamboni11.Components.Blaze;

internal class GameManager : GameManagerBase.Server
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static readonly Timer Timer;

    static GameManager()
    {
        Timer = new Timer(5000);
        Timer.Elapsed += OnTimedEvent;
        Timer.AutoReset = true;
        Timer.Enabled = true;
    }

    private static void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        foreach (var serverGame in ServerManager.GetServerGames().ToList()) // How to not fix bugs
            if (serverGame.ServerPlayers.Count == 0)
            {
                ServerManager.RemoveServerGame(serverGame);
                foreach (var serverPlayer in ServerManager.GetServerPlayers())
                    NotifyGameRemovedAsync(serverPlayer.BlazeServerConnection, new NotifyGameRemoved
                    {
                        mDestructionReason = GameDestructionReason.HOST_LEAVING,
                        mGameId = serverGame.ReplicatedGameData.mGameId
                    });
            }

        var time = (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        foreach (var serverPlayer in ServerManager.GetServerPlayers()) // How to not fix bugs pt2
        {
            if (serverPlayer.LastPingedTime == 0) continue;
            if (serverPlayer.LastPingedTime + 3600 >= time) continue;
            if (serverPlayer.BlazeServerConnection != null)
                UserSessionsBase.Server.NotifyUserSessionDisconnectedAsync(serverPlayer.BlazeServerConnection, new UserSessionDisconnectReason
                {
                    mDisconnectReason = UserSessionDisconnectReason.DisconnectReason.DUPLICATE_LOGIN
                });

            ServerManager.RemoveServerPlayer(serverPlayer);
        }

        // if (ServerManager.GetQueuedPlayers().Count <= 1) return;
        //
        // var grouped = ServerManager.GetQueuedPlayers().GroupBy(u => u.StartMatchmakingRequest.mCriteriaData.mGenericRulePrefsList.Find(prefs => prefs.mRuleName.Equals("OSDK_gameMode")).mDesiredValues[0]);
        //
        // foreach (var group in grouped)
        // {
        //     var users = group.ToList();
        //
        //     while (users.Count >= 2)
        //     {
        //         var queuedPlayerA = users[0];
        //         var queuedPlayerB = users[1];
        //
        //         users.RemoveRange(0, 2);
        //         ServerManager.RemoveQueuedPlayer(queuedPlayerA);
        //         ServerManager.RemoveQueuedPlayer(queuedPlayerB);
        //
        //         SendToMatchMakingGame(queuedPlayerA, queuedPlayerB, queuedPlayerA.StartMatchmakingRequest, group.Key);
        //     }
        // }
    }

    // private static void SendToMatchMakingGame(QueuedPlayer host, QueuedPlayer notHost, StartMatchmakingRequest startMatchmakingRequest, string gameMode)
    // {
    //     var zamboniGame = new ServerGame(host.ServerPlayer, startMatchmakingRequest, gameMode);
    //
    //     zamboniGame.AddGameParticipant(host.ServerPlayer, host.MatchmakingSessionId);
    //     zamboniGame.AddGameParticipant(notHost.ServerPlayer, notHost.MatchmakingSessionId);
    // }

    // public override Task<StartMatchmakingResponse> StartMatchmakingAsync(StartMatchmakingRequest request, BlazeRpcContext context)
    // {
    //     var serverPlayer = ServerManager.GetServerPlayer(context.BlazeConnection);
    //
    //     var queuedPlayer = new QueuedPlayer(serverPlayer, request);
    //     ServerManager.AddQueuedPlayer(queuedPlayer);
    //
    //     return Task.FromResult(new StartMatchmakingResponse
    //     {
    //         mSessionId = queuedPlayer.MatchmakingSessionId
    //     });
    // }

    public override Task<JoinGameResponse> ResetDedicatedServerAsync(CreateGameRequest request, BlazeRpcContext context)
    {
        var host = ServerManager.GetServerPlayer(context.BlazeConnection);

        var serverGame = new ServerGame(host, request);

        Task.Run(async () =>
        {
            await Task.Delay(100);
            serverGame.AddGameParticipant(host);
            var lobbies = GetLobbies();

            foreach (var serverPlayer in ServerManager.GetServerPlayers().ToList())
                NotifyGameListUpdateAsync(serverPlayer.BlazeServerConnection, new NotifyGameListUpdate
                {
                    mIsFinalUpdate = 1,
                    mListId = 1,
                    // mRemovedGameList = null, Not sure should we use this
                    mUpdatedGames = lobbies
                });
        });

        return Task.FromResult(new JoinGameResponse
        {
            mGameId = serverGame.ReplicatedGameData.mGameId
        });
    }

    public override Task<JoinGameResponse> JoinGameAsync(JoinGameRequest request, BlazeRpcContext context)
    {
        var serverGame = ServerManager.GetServerGame(request.mGameId);
        var serverPlayer = ServerManager.GetServerPlayer(context.BlazeConnection);

        if (!serverGame.HasSpaceForPlayer()) throw new Exception();

        if (serverGame == null || serverPlayer == null)
            return Task.FromResult(new JoinGameResponse
            {
                mGameId = 0,
                mJoinState = JoinState.JOINED_GAME
            });

        Task.Run(async () =>
        {
            await Task.Delay(100);
            serverGame.AddGameParticipant(serverPlayer);
        });

        return Task.FromResult(new JoinGameResponse
        {
            mGameId = request.mGameId,
            mJoinState = JoinState.JOINED_GAME
        });
    }

    private static List<GameBrowserMatchData> GetLobbies()
    {
        var lobbies = new List<GameBrowserMatchData>();
        foreach (var serverGame in ServerManager.GetServerGames())
        {
            if (serverGame.ReplicatedGameData.mGameState != GameState.PRE_GAME &&
                serverGame.ReplicatedGameData.mGameState != GameState.INITIALIZING) continue;

            if (!serverGame.HasSpaceForPlayer()) continue;

            var participants = new List<GameBrowserPlayerData>();
            foreach (var gamePlayer in serverGame.ReplicatedGamePlayers)
                participants.Add(new GameBrowserPlayerData
                {
                    mAccountLocale = gamePlayer.mAccountLocale,
                    mExternalId = gamePlayer.mExternalId,
                    mPlayerAttribs = gamePlayer.mPlayerAttribs,
                    mPlayerId = gamePlayer.mPlayerId,
                    mPlayerName = gamePlayer.mPlayerName,
                    mPlayerState = gamePlayer.mPlayerState,
                    mTeamIndex = gamePlayer.mTeamIndex
                });

            var teamInfo = new List<GameBrowserTeamInfo>();

            // foreach (var teamCapacity in serverGame.ReplicatedGameData.mTeamCapacity)
            // {
            //     teamInfo.Add(new GameBrowserTeamInfo
            //     {
            //         mTeamId = teamCapacity.mTeamId,
            //         mTeamSize = teamCapacity.mTeamCapacity
            //     });
            // }

            lobbies.Add(new GameBrowserMatchData
            {
                mFitScore = 1,
                mGameData = new GameBrowserGameData
                {
                    mAdminPlayerList = serverGame.ReplicatedGameData.mAdminPlayerList,
                    mEntryCriteriaMap = serverGame.ReplicatedGameData.mEntryCriteriaMap,
                    mExternalSessionId = 1,
                    mGameAttribs = serverGame.ReplicatedGameData.mGameAttribs,
                    mGameBrowserTeamInfoVector = teamInfo,
                    mGameId = serverGame.ReplicatedGameData.mGameId,
                    mGameName = serverGame.ReplicatedGameData.mGameName,
                    mGameProtocolVersionString = serverGame.ReplicatedGameData.mGameProtocolVersionString,
                    mGameRoster = participants,
                    mGameSettings = serverGame.ReplicatedGameData.mGameSettings,
                    mGameState = serverGame.ReplicatedGameData.mGameState,
                    mHostId = serverGame.ReplicatedGameData.mTopologyHostInfo.mPlayerId,
                    mHostNetworkAddressList = serverGame.ReplicatedGameData.mHostNetworkAddressList,
                    mNetworkTopology = serverGame.ReplicatedGameData.mNetworkTopology,
                    mPersistedGameId = serverGame.ReplicatedGameData.mPersistedGameId,
                    mPingSiteAlias = "qos",
                    mPlayerCounts = serverGame.ReplicatedGameData.mTeamIds,
                    mPresenceMode = serverGame.ReplicatedGameData.mPresenceMode,
                    mQueueCapacity = serverGame.ReplicatedGameData.mQueueCapacity,
                    mQueueCount = serverGame.ReplicatedGameData.mQueueCapacity,
                    mSlotCapacities = serverGame.ReplicatedGameData.mSlotCapacities,
                    mTeamCapacity = 5,
                    mVoipTopology = VoipTopology.VOIP_DISABLED
                }
            });
        }

        return lobbies;
    }

    public override Task<GetGameListResponse> GetGameListSubscriptionAsync(GetGameListRequest request, BlazeRpcContext context)
    {
        var lobbies = GetLobbies();

        Task.Run(async () =>
        {
            await Task.Delay(100);
            NotifyGameListUpdateAsync(context.BlazeConnection, new NotifyGameListUpdate
            {
                mIsFinalUpdate = 1,
                mListId = 1,
                // mRemovedGameList = null, Not sure should we use this
                mUpdatedGames = lobbies
            });
        });


        return Task.FromResult(new GetGameListResponse
        {
            mListId = 1,
            mMaxPossibleFitScore = 1
        });
    }

    public override Task<NullStruct> DestroyGameListAsync(DestroyGameListRequest request, BlazeRpcContext context)
    {
        return Task.FromResult(new NullStruct());
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

        foreach (var serverPlayer in serverGame.ServerPlayers.ToList())
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

        foreach (var serverPlayer in serverGame.ServerPlayers.ToList())
            NotifyGameStateChangeAsync(serverPlayer.BlazeServerConnection, new NotifyGameStateChange
            {
                mGameId = request.mGameId,
                mNewGameState = request.mNewGameState
            });
        return Task.FromResult(new NullStruct());
    }


    public override Task<NullStruct> SetPlayerAttributesAsync(SetPlayerAttributesRequest request, BlazeRpcContext context)
    {
        var serverGame = ServerManager.GetServerGame(request.mGameId);
        var serverPlayer = ServerManager.GetServerPlayer((uint)request.mPlayerId);
        var replicatedGamePlayer = serverGame.ReplicatedGamePlayers.Find(player => player.mPlayerId == request.mPlayerId);
        replicatedGamePlayer.mPlayerAttribs = request.mPlayerAttributes;


        foreach (var participant in serverGame.ServerPlayers.ToList())
            NotifyPlayerAttribChangeAsync(participant.BlazeServerConnection, new NotifyPlayerAttribChange
            {
                mGameId = serverGame.ReplicatedGameData.mGameId,
                mPlayerAttribs = request.mPlayerAttributes,
                mPlayerId = request.mPlayerId
            });

        return Task.FromResult(new NullStruct());
    }

    public override Task<NullStruct> UpdateGameHostMigrationStatusAsync(UpdateGameHostMigrationStatusRequest request, BlazeRpcContext context)
    {
        var serverGame = ServerManager.GetServerGame(request.mGameId);

        foreach (var participant in serverGame.ServerPlayers.ToList())
            NotifyHostMigrationFinishedAsync(participant.BlazeServerConnection, new NotifyHostMigrationFinished
            {
                mGameId = serverGame.ReplicatedGameData.mGameId
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
                    if (serverPlayer != null)
                        serverGame.RemoveGameParticipant(serverPlayer, PlayerRemovedReason.PLAYER_CONN_LOST);
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

        //Hack fix
        UserSessionsBase.Server.NotifyUserSessionDisconnectedAsync(context.BlazeConnection, new UserSessionDisconnectReason
        {
            mDisconnectReason = UserSessionDisconnectReason.DisconnectReason.DUPLICATE_LOGIN
        });
        serverGame.RemoveGameParticipant(serverPlayer, request.mPlayerRemovedReason);
        var lobbies = GetLobbies();
        Task.Run(async () =>
        {
            await Task.Delay(100);

            NotifyGameListUpdateAsync(context.BlazeConnection, new NotifyGameListUpdate
            {
                mIsFinalUpdate = 1,
                mListId = 1,
                // mRemovedGameList = null, Not sure should we use this
                mUpdatedGames = lobbies
            });
        });
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

        foreach (var serverPlayer in serverGame.ServerPlayers.ToList())
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

        foreach (var serverPlayer in serverGame.ServerPlayers.ToList())
            NotifyGameSettingsChangeAsync(serverPlayer.BlazeServerConnection, new NotifyGameSettingsChange
            {
                mGameSettings = request.mGameSettings,
                mGameId = request.mGameId
            });
        return Task.FromResult(new NullStruct());
    }

    public override Task<CreateGameResponse> CreateGameAsync(CreateGameRequest request, BlazeRpcContext context)
    {
        var host = ServerManager.GetServerPlayer(context.BlazeConnection);

        var serverGame = new ServerGame(host, request);

        Task.Run(async () =>
        {
            await Task.Delay(100);
            serverGame.AddGameParticipant(host);
            var lobbies = GetLobbies();

            foreach (var serverPlayer in ServerManager.GetServerPlayers().ToList())
                NotifyGameListUpdateAsync(serverPlayer.BlazeServerConnection, new NotifyGameListUpdate
                {
                    mIsFinalUpdate = 1,
                    mListId = 1,
                    // mRemovedGameList = null, Not sure should we use this
                    mUpdatedGames = lobbies
                });
        });

        return Task.FromResult(new CreateGameResponse
        {
            mGameId = serverGame.ReplicatedGameData.mGameId
        });
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