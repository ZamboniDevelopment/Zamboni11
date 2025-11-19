using System.Collections.Generic;
using System.Threading.Tasks;
using Blaze3SDK.Blaze.Example;
using BlazeCommon;
using Zamboni11.Components.NHL11.Bases;
using Zamboni11.Components.NHL11.Requests;
using Zamboni11.Components.NHL11.Responses;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11;

internal class OsdkDynamicMessagingComponent : OsdkDynamicMessagingComponentBase.Server
{
    public override Task<ExampleResponse> GetConfigAsync(NullStruct request, BlazeRpcContext context)
    {
        return Task.FromResult(new ExampleResponse
        {
            //    CDRD = 0 (0x0000)
            // CERD = 0 (0x0000)
            // CMDI = 0 (0x0000)
            // CMMC = 0 (0x0000)
            //JNE
            mMessage = "null"
        });
    }

    public override Task<MessageResponse> GetMessagesAsync(MessageRequest request, BlazeRpcContext context)
    {
        // throw new Exception();
        return Task.FromResult(new MessageResponse
        {
            mEnum = ENUM.DYNAMICMESSAGE_ENUM_SUCCESS,
            mMessagesList = new List<MSGS>
            {
                new()
                {
                    mData = "This might be some proprietary ea format. Strings just crash",
                    mFormat = FMAT.DYNAMICMESSAGE_FORMAT_PLAINTEXT,
                    mHint = "loadingScreen",
                    mMessageUid = 0,
                    mText = new List<DATADURN>
                    {
                        new()
                        {
                            mData = "This might be some proprietary ea format. Strings just crash",
                            mDurn = 10
                        }
                    },
                    mTitle = "NHL11",
                    mType = TYPE.DYNAMICMESSAGE_TYPE_MATCHMAKING
                }
            }
        });
    }
}