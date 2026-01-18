using Tdf;

namespace Zamboni11.Components.NHL11.Responses
{
    [TdfStruct]
    public struct DynamicConfigResponse
    {
        [TdfMember("CDRD")]
        public ushort mDataRequestDelay;
        
        [TdfMember("CERD")]
        public ushort mErrorRetryDelay;
        
        [TdfMember("CMDI")]
        public ushort mMessageDelayInterval;
        
        [TdfMember("CMMC")]
        public ushort mMaximumMessageCount;
    }
}