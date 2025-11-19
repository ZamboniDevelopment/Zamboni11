using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct DATADURN
{
    [TdfMember("DATA")] 
    public string mData;

    [TdfMember("DURN")] 
    public int mDurn;
}