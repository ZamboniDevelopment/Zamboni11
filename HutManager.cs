using System.Collections.Generic;
using Blaze3SDK.Blaze.Rsp;
using BlazeCommon;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutManager
{
    //TODO: OF COURSE SAVE TO DISK
    
    public static List<HutPlayerInstance> HutPlayerInstances = new();
    public static Dictionary<ulong, CardData> CardData = new();
    public static ulong CardIdCounter = 1;
    
    public static HutPlayerInstance? GetHutPlayerInstance(ServerPlayer serverPlayer)
    {
        return HutPlayerInstances.Find(instance => instance.ServerPlayer.Equals(serverPlayer));
    }
    
    public static HutPlayerInstance? GetHutPlayerInstance(BlazeServerConnection blazeServerConnection)
    {
        return HutPlayerInstances.Find(instance => instance.ServerPlayer.Equals(ServerManager.GetServerPlayer(blazeServerConnection)));
    }
    
    public static void AddHutPlayerInstance(HutPlayerInstance hutPlayerInstance)
    {
        HutPlayerInstances.Add(hutPlayerInstance);
    }
}