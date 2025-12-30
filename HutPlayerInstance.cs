using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutPlayerInstance
{
    public ServerPlayer ServerPlayer { get; set; }
    public INFO Info { get; set; }

    public HutPlayerInstance(ServerPlayer serverPlayer, INFO info)
    {
        ServerPlayer = serverPlayer;
        Info = info;
    }
}