namespace Zamboni11.Components.NHL11.Structs;

public enum MatchState : uint
{
    //TODO ID'S Are not all confirmed
    CARDHOUSE_MATCHSTART = 0,
    CARDHOUSE_MATCHNOTFINISHED = 1,
    CARDHOUSE_MATCHFINISHED_OK = 2,
    CARDHOUSE_MATCHNOTFINISHED_IGNORE = 3,
}