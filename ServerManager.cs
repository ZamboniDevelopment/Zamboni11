using System;
using System.Collections.Concurrent;
using System.Linq;
using BlazeCommon;

namespace Zamboni11;

public static class ServerManager
{
    private static readonly ConcurrentDictionary<ulong, ServerPlayer> ServerPlayers = new();
    private static readonly ConcurrentDictionary<ulong, QueuedPlayer> QueuedPlayers = new();
    private static readonly ConcurrentDictionary<uint, ServerGame> ServerGames = new();

    public static void AddServerPlayer(ulong id, ServerPlayer serverPlayer)
    {
        var existing = GetServerPlayer(serverPlayer.UserIdentification.mName);
        if (existing != null) RemoveServerPlayer(existing.UserIdentification.mExternalId);
        ServerPlayers.TryAdd(id, serverPlayer);
    }

    public static void AddQueuedPlayer(ulong id, QueuedPlayer queuedPlayer)
    {
        QueuedPlayers.TryAdd(id, queuedPlayer);
    }

    public static void AddServerGame(uint id, ServerGame serverGame)
    {
        ServerGames.TryAdd(id, serverGame);
    }

    public static bool RemoveServerPlayer(ulong id)
    {
        return ServerPlayers.TryRemove(id, out _);
    }

    public static bool RemoveQueuedPlayer(ulong id)
    {
        return QueuedPlayers.TryRemove(id, out _);
    }

    public static bool RemoveServerGame(uint id)
    {
        return ServerGames.TryRemove(id, out _);
    }

    public static ConcurrentDictionary<ulong, ServerPlayer> GetServerPlayers()
    {
        return ServerPlayers;
    }

    public static ConcurrentDictionary<ulong, QueuedPlayer> GetQueuedPlayers()
    {
        return QueuedPlayers;
    }

    public static ConcurrentDictionary<uint, ServerGame> GetServerGames()
    {
        return ServerGames;
    }

    public static ServerPlayer? GetServerPlayer(BlazeServerConnection blazeServerConnection)
    {
        return ServerPlayers.Values.FirstOrDefault(serverPlayer => serverPlayer.BlazeServerConnection.Equals(blazeServerConnection));
    }

    public static ServerPlayer? GetServerPlayer(ProtoFireConnection protoFireConnection)
    {
        return ServerPlayers.Values.FirstOrDefault(serverPlayer => serverPlayer.BlazeServerConnection.ProtoFireConnection.Equals(protoFireConnection));
    }

    public static ServerPlayer? GetServerPlayer(uint userId)
    {
        var asUlong = (ulong)userId;
        return ServerPlayers[asUlong];
    }

    public static ServerPlayer? GetServerPlayer(string name)
    {
        return ServerPlayers.Values.FirstOrDefault(p => p.UserIdentification.mName.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public static ServerGame? GetServerGame(uint id)
    {
        return ServerGames[id];
    }

    public static ServerGame? GetServerGame(ServerPlayer serverPlayer)
    {
        return ServerGames.Values.FirstOrDefault(serverGame => serverGame.ServerPlayers.Contains(serverPlayer));
    }

    public static QueuedPlayer? GetQueuedPlayer(ServerPlayer serverPlayer)
    {
        return QueuedPlayers.Values.FirstOrDefault(queuedPlayer => queuedPlayer.ServerPlayer.Equals(serverPlayer));
    }
}