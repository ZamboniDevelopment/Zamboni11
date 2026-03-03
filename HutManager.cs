using System.Collections.Generic;
using BlazeCommon;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutManager
{
    //TODO: OF COURSE SAVE TO DISK
    public static Dictionary<long, HutPlayerInstance> HutPlayerInstances = new();
    public static Dictionary<long, Dictionary<long, CardData>> UserInventories = new();

    public static HutPlayerInstance? GetHutPlayerInstance(BlazeServerConnection blazeServerConnection)
    {
        var player = ServerManager.GetServerPlayer(blazeServerConnection);
        if (player == null) return null;
        var userId = player.UserIdentification.mAccountId;
        return HutPlayerInstances.TryGetValue(userId, out var instance) ? instance : null;
    }
    public static void AddHutPlayerInstance(long userId, HutPlayerInstance hutPlayerInstance)
    {
        HutPlayerInstances.Add(userId, hutPlayerInstance);
    }

    public static CardData GetCard(long owner, long cardId)
    {
        return cardId == 0 ? new CardData() : UserInventories[owner][cardId];
    }
}