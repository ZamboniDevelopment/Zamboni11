using System.Collections.Generic;
using Blaze3SDK.Blaze.League;

namespace Zamboni11;

public class LeagueManager
{
    public static uint LeagueIdCounter = 1;
    public static Dictionary<uint, League> Leagues = new Dictionary<uint, League>();
    public static Dictionary<ulong, MemberInfo> Members = new Dictionary<ulong, MemberInfo>();
}