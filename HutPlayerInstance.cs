using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutPlayerInstance
{
    public ServerPlayer ServerPlayer { get; set; }
    public GamerInfo GamerInfo { get; set; }

    public HutPlayerInstance(ServerPlayer serverPlayer, GamerInfo gamerInfo)
    {
        ServerPlayer = serverPlayer;
        GamerInfo = gamerInfo;
    }
}