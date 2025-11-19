using System.Collections.Generic;
using Tdf;

namespace Zamboni11.Components.NHL11.Structs;

[TdfStruct]
public struct MSGS
{
    [TdfMember("DATA")] 
    public string mData;

    [TdfMember("FMAT")]
    public FMAT mFormat;

    [TdfMember("HINT")] 
    public string mHint;

    [TdfMember("IMGS")] 
    public List<DATADURN> mImages;

    [TdfMember("MUID")]
    public int mMessageUid;

    [TdfMember("TEXT")] 
    public List<DATADURN> mText;

    [TdfMember("TITL")]
    public string mTitle;

    [TdfMember("TYPE")] 
    public TYPE mType;
}