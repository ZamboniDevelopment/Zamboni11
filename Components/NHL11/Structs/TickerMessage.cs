using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct TickerMessage
{
    [TdfMember("DATA")] 
    public List<string> mData;

    [TdfMember("ENDT")] 
    public uint mENDT;
    
    [TdfMember("FILT")] 
    public uint mFilterIndex;
    
    [TdfMember("IDEN")] 
    public uint mBlazeId;
    
    [TdfMember("PRIO")] 
    public int mPRIO;
    
    [TdfMember("PROV")] 
    public string mPROV;
    
    [TdfMember("STRT")] 
    public uint mSTRT;
    
    [TdfMember("TYPE")] 
    public TickerMessageType mType;
    
}