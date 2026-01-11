using System.Collections.Generic;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11;

public class HutPlayerInstance
{
    public ServerPlayer ServerPlayer { get; set; }
    public GamerInfo GamerInfo { get; set; }
    public List<CardData> ActiveCards { get; set; }
    public SquadInfo SquadInfo { get; set; }
    public uint SyncVersion { get; set; } = 1;

    public VersionInfo GetVersionInfo()
    {
        SyncVersion++;
        return new VersionInfo
        {
            mVersionEscrow = SyncVersion,
            mVersionGeneral = SyncVersion,
            mVersionUnassigned = SyncVersion
        };
    }
    public HutPlayerInstance(ServerPlayer serverPlayer, GamerInfo gamerInfo)
    {
        ServerPlayer = serverPlayer;
        GamerInfo = gamerInfo;
    }
}