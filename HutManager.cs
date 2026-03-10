using System.Collections.Concurrent;
using System.Collections.Generic;
using BlazeCommon;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutManager
{
    //TODO: OF COURSE SAVE TO DISK
    // public static ConcurrentDictionary<long, HutPlayerInstance> HutPlayerInstances = new();
    // public static ConcurrentDictionary<long, Dictionary<long, CardData>> UserInventories = new();
    // public static ConcurrentDictionary<long, Dictionary<long, CardData>> UserUnAssigned = new();
    // public static ConcurrentDictionary<long, Dictionary<long, CardData>> Escrow = new();

    // public static HutPlayerInstance? GetHutPlayerInstance(BlazeServerConnection blazeServerConnection)
    // {
    //     var player = ServerManager.GetServerPlayer(blazeServerConnection);
    //     if (player == null) return null;
    //     var userId = player.UserIdentification.mAccountId;
    //     return HutPlayerInstances.TryGetValue(userId, out var instance) ? instance : null;
    // }
    // public static void AddHutPlayerInstance(long userId, HutPlayerInstance hutPlayerInstance)
    // {
    //     HutPlayerInstances.TryAdd(userId, hutPlayerInstance);
    // }
    //
    // public static CardData GetCard(long owner, long cardId)
    // {
    //     return cardId == 0 ? new CardData() : UserInventories[owner][cardId];
    // }
    // public static CardData GetCard(long cardId)
    // {
    //     foreach (var owner in UserInventories)
    //     {
    //         foreach (var VARIABLE in owner.Value.Keys)
    //         {
    //             if (VARIABLE.Equals(cardId))
    //             {
    //                 return UserInventories[owner.Key][VARIABLE];
    //             }
    //         }
    //     }
    //     foreach (var owner in Escrow)
    //     {
    //         foreach (var VARIABLE in owner.Value.Keys)
    //         {
    //             if (VARIABLE.Equals(cardId))
    //             {
    //                 return Escrow[owner.Key][VARIABLE];
    //             }
    //         }
    //     }
    //
    //     return new CardData();
    // }
}