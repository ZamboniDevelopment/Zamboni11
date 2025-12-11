using System.Collections.Generic;
using System.Linq;
using Blaze3SDK.Blaze;
using Blaze3SDK.Blaze.GameManager;
using Blaze3SDK.Components;
using Zamboni11.Components.Blaze;

namespace Zamboni11;

public class ServerGame
{
    private readonly object _lockReplicatedPlayers = new();

    // public ServerGame(ServerPlayer host, StartMatchmakingRequest request, string gameMode)
    // {
    //     var gameId = Program.Database.GetNextGameId();
    //
    //     ReplicatedGameData = new ReplicatedGameData
    //     {
    //         mAdminPlayerList = new List<long>
    //         {
    //             host.UserIdentification.mAccountId
    //         },
    //         mGameAttribs = VsGameAttribs(),
    //         mSlotCapacities = Capacities(gameMode),
    //         mEntryCriteriaMap = request.mEntryCriteriaMap,
    //         mGameId = gameId,
    //         mGameName = "game" + gameId,
    //         mGameSettings = request.mGameSettings,
    //         mGameReportingId = gameId,
    //         mGameState = GameState.INITIALIZING,
    //         mHostNetworkAddressList = new List<NetworkAddress>
    //         {
    //             host.ExtendedData.mAddress
    //         },
    //         mTopologyHostSessionId = (uint)host.UserIdentification.mAccountId,
    //         mIgnoreEntryCriteriaWithInvite = true,
    //         mMeshAttribs = new SortedDictionary<string, string>(),
    //         mMaxPlayerCapacity = Capacities(gameMode)[1],
    //         mNetworkQosData = host.ExtendedData.mQosData,
    //         mNetworkTopology = request.mNetworkTopology,
    //         mPlatformHostInfo = new HostInfo
    //         {
    //             mPlayerId = host.UserIdentification.mAccountId,
    //             mSlotId = 0
    //         },
    //         mPingSiteAlias = "qos",
    //         mQueueCapacity = 0,
    //         mTopologyHostInfo = new HostInfo
    //         {
    //             mPlayerId = host.UserIdentification.mAccountId,
    //             mSlotId = 0
    //         },
    //         mTeamCapacity = new List<TeamCapacity>
    //         {
    //             new()
    //             {
    //                 mTeamCapacity = 2,
    //                 mTeamId = 1,
    //                 mTeamIndex = 0
    //             },
    //             new()
    //             {
    //                 mTeamCapacity = 2,
    //                 mTeamId = 2,
    //                 mTeamIndex = 1
    //             }
    //         },
    //         mUUID = "game" + gameId,
    //         mVoipNetwork = VoipTopology.VOIP_DISABLED,
    //         mGameProtocolVersionString = request.mGameProtocolVersionString,
    //         mXnetNonce = new byte[]
    //         {
    //         },
    //         mXnetSession = new byte[]
    //         {
    //         }
    //     };
    //     ServerManager.AddServerGame(this);
    // }

    public ServerGame(ServerPlayer host, CreateGameRequest request)
    {
        var gameId = Program.Database.GetNextGameId();

        ReplicatedGameData = new ReplicatedGameData
        {
            mAdminPlayerList = new List<long>
            {
                host.UserIdentification.mAccountId
            },
            mEntryCriteriaMap = request.mEntryCriteriaMap,
            mGameAttribs = request.mGameAttribs,
            mGameId = gameId,
            mGameName = "game" + gameId,
            mGameProtocolVersionHash = GameManager.GetGameProtocolVersionHash(request.mGameProtocolVersionString),
            mGameProtocolVersionString = request.mGameProtocolVersionString,
            mGameReportingId = gameId,
            mGameSettings = request.mGameSettings,
            mGameState = GameState.INITIALIZING,
            mGameTypeName = request.mGameTypeName,
            mHostNetworkAddressList = new List<NetworkAddress>
            {
                host.ExtendedData.mAddress
            },
            mIgnoreEntryCriteriaWithInvite = request.mIgnoreEntryCriteriaWithInvite,
            mMaxPlayerCapacity = request.mMaxPlayerCapacity,
            mMeshAttribs = request.mMeshAttribs,
            mNetworkQosData = host.ExtendedData.mQosData,
            mNetworkTopology = GameNetworkTopology.PEER_TO_PEER_FULL_MESH, //TODO
            mPersistedGameId = gameId.ToString(),
            mPersistedGameIdSecret = request.mPersistedGameIdSecret,
            mPingSiteAlias = "qos",
            mPlatformHostInfo = new HostInfo
            {
                mPlayerId = host.UserIdentification.mAccountId,
                mSlotId = 0
            },
            mTopologyHostSessionId = (uint)host.UserIdentification.mAccountId,
            mPresenceMode = request.mPresenceMode,
            mQueueCapacity = request.mQueueCapacity,
            mServerNotResetable = request.mServerNotResetable,
            mSharedSeed = gameId,
            mSlotCapacities = request.mSlotCapacities,
            mTeamCapacity = new List<TeamCapacity>
            {
                // new()
                // {
                //     mTeamCapacity = 6,
                //     mTeamId = 1,
                //     mTeamIndex = 0
                // },
                // new()
                // {
                //     mTeamCapacity = 3,
                //     mTeamId = 2,
                //     mTeamIndex = 1
                // }
            },
            mTeamIds = request.mTeamIds,
            mTopologyHostInfo = new HostInfo
            {
                mPlayerId = host.UserIdentification.mAccountId,
                mSlotId = 0
            },
            mUUID = gameId.ToString(),
            mVoipNetwork = VoipTopology.VOIP_DISABLED,
            mXnetNonce = new byte[]
            {
            },
            mXnetSession = new byte[]
            {
            }
        };
        ServerManager.AddServerGame(this);
    }

    public List<ServerPlayer> ServerPlayers { get; } = new();
    public ReplicatedGameData ReplicatedGameData { get; set; }
    public List<ReplicatedGamePlayer> ReplicatedGamePlayers { get; set; } = new();

    public bool HasSpaceForPlayer()
    {
        return ReplicatedGameData.mSlotCapacities[0] > ReplicatedGamePlayers.Count;
    }

    private List<ushort> Capacities(string gameMode)
    {
        if (gameMode.Equals("3"))
            return new List<ushort>
            {
                0, 12
            };

        return new List<ushort>
        {
            0, 2
        };
    }

    public void AddGameParticipant(ServerPlayer serverPlayer, uint matchmakingSessionId = 0)
    {
        //TODO Lobby capacities?
        ServerPlayers.Add(serverPlayer);
        var replicatedGamePlayer = serverPlayer.ToReplicatedGamePlayer((byte)(ServerPlayers.Count - 1), ReplicatedGameData.mGameId);
        ReplicatedGamePlayers.Add(replicatedGamePlayer);
        ReplicatedGameData.mHostNetworkAddressList.Add(serverPlayer.ExtendedData.mAddress);

        GameManagerBase.Server.NotifyGameSetupAsync(serverPlayer.BlazeServerConnection, new NotifyGameSetup
        {
            mGameData = ReplicatedGameData,
            mGameRoster = ReplicatedGamePlayers
            // mGameSetupReason = gameSetupReason
        });

        NotifyParticipants(new NotifyPlayerJoining
        {
            mGameId = ReplicatedGameData.mGameId,
            mJoiningPlayer = replicatedGamePlayer
        });
    }

    public void RemoveGameParticipant(ServerPlayer serverPlayer, PlayerRemovedReason reason)
    {
        //Concurrent exception
        lock (_lockReplicatedPlayers)
        {
            ServerPlayers.Remove(serverPlayer);

            var replicatedPlayerToRemove = ReplicatedGamePlayers.Find(replicatedPlayer => replicatedPlayer.mPlayerName.Equals(serverPlayer.UserIdentification.mName)
            );

            ReplicatedGamePlayers.Remove(replicatedPlayerToRemove);
        }

        NotifyParticipants(new NotifyPlayerRemoved
        {
            mPlayerRemovedTitleContext = 0,
            mGameId = ReplicatedGameData.mGameId,
            mPlayerId = serverPlayer.UserIdentification.mBlazeId,
            mPlayerRemovedReason = reason
        });
    }

    public void NotifyParticipants(NotifyGamePlayerStateChange playerStateChange)
    {
        foreach (var serverPlayer in ServerPlayers.ToList()) GameManagerBase.Server.NotifyGamePlayerStateChangeAsync(serverPlayer.BlazeServerConnection, playerStateChange);
    }

    public void NotifyParticipants(NotifyPlayerJoinCompleted playerJoinCompleted)
    {
        foreach (var serverPlayer in ServerPlayers.ToList()) GameManagerBase.Server.NotifyPlayerJoinCompletedAsync(serverPlayer.BlazeServerConnection, playerJoinCompleted);
    }

    public void NotifyParticipants(NotifyPlayerRemoved playerRemoved)
    {
        foreach (var serverPlayer in ServerPlayers.ToList()) GameManagerBase.Server.NotifyPlayerRemovedAsync(serverPlayer.BlazeServerConnection, playerRemoved);
    }

    private void NotifyParticipants(NotifyPlayerJoining playerJoining)
    {
        foreach (var serverPlayer in ServerPlayers.ToList()) GameManagerBase.Server.NotifyPlayerJoiningAsync(serverPlayer.BlazeServerConnection, playerJoining);
    }


    private SortedDictionary<string, string> VsGameAttribs()
    {
        return new SortedDictionary<string, string>
        {
            { "CreatedPlays", "1" },
            { "Fighting", "1" },
            { "Injuries", "1" },
            { "OSDK_categoryId", "0" },
            { "OSDK_coop", "1" },
            { "OSDK_gameMode", "1" },
            { "OSDK_matchupHash", "0" },
            { "OSDK_roomId", "0" },
            { "OSDK_rosterURL", "" },
            { "OSDK_rosterVersion", "1LuRFj1HEciO3Cn22f1PWbzP3Sf6vF1dzeTg4HOr3l1FcL9G1bjJEj" },
            { "OSDK_sponsoredEventId", "0" },
            { "Penalties", "1" },
            { "PeriodLength", "5" },
            { "Rules", "1" }
        };
    }

    public override string ToString()
    {
        return "Players: " +
               string.Join(", ", ServerPlayers.Select(serverPlayer => serverPlayer.UserIdentification.mName)) +
               " gameId:" + ReplicatedGameData.mGameId +
               " state: " + ReplicatedGameData.mGameState +
               " type (1 vs game 2 shootout, 3 otp): " + ReplicatedGameData.mGameAttribs["OSDK_gameMode"];
    }
}