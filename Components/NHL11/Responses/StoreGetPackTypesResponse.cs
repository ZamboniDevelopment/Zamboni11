using System.Collections.Generic;
using Tdf;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Responses;

[TdfStruct]
public struct StoreGetPackTypesResponse
{
    [TdfMember("FRPK")] 
    public short mFreePack;
    
    [TdfMember("PPH")] 
    public byte mPremiumPacksHidden;
    
    [TdfMember("PTPS")] 
    public List<StorePackTypeData> mPackTypeList;
    
    [TdfMember("SVTM")] 
    public uint mServerTime;
    
}