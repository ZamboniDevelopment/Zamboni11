using System;
using System.Threading.Tasks;
using Blaze3SDK;
using Blaze3SDK.Blaze.Example;
using BlazeCommon;
using NLog;
using Zamboni11.Components.NHL11.Requests;
using Zamboni11.Components.NHL11.Responses;

namespace Zamboni11.Components.NHL11.Bases;

public static class OsdkDynamicMessagingComponentBase
{
    public enum OsdkDynamicMessagingComponentCommand : ushort
    {
        getMessages = 1,
        getConfig = 2
    }

    public enum OsdkDynamicMessagingComponentNotification : ushort
    {
    }

    public const ushort Id = 2250;
    public const string Name = "OsdkDynamicMessagingComponent";

    public static Type GetCommandRequestType(OsdkDynamicMessagingComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            OsdkDynamicMessagingComponentCommand.getMessages => typeof(MessageRequest),
            OsdkDynamicMessagingComponentCommand.getConfig => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetCommandResponseType(OsdkDynamicMessagingComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            OsdkDynamicMessagingComponentCommand.getMessages => typeof(MessageResponse),
            OsdkDynamicMessagingComponentCommand.getConfig => typeof(ExampleResponse),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetCommandErrorResponseType(OsdkDynamicMessagingComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            OsdkDynamicMessagingComponentCommand.getMessages => typeof(NullStruct),
            OsdkDynamicMessagingComponentCommand.getConfig => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetNotificationType(OsdkDynamicMessagingComponentNotification notification)
    {
        return notification switch
        {
            _ => typeof(NullStruct)
        };
    }

    public class Server : BlazeServerComponent<OsdkDynamicMessagingComponentCommand, OsdkDynamicMessagingComponentNotification, Blaze3RpcError>
    {
        public Server() : base(OsdkDynamicMessagingComponentBase.Id, OsdkDynamicMessagingComponentBase.Name)
        {
        }

        [BlazeCommand((ushort)OsdkDynamicMessagingComponentCommand.getConfig)]
        public virtual Task<ExampleResponse> GetConfigAsync(NullStruct request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)OsdkDynamicMessagingComponentCommand.getMessages)]
        public virtual Task<MessageResponse> GetMessagesAsync(MessageRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        public override Type GetCommandRequestType(OsdkDynamicMessagingComponentCommand componentCommand)
        {
            return OsdkDynamicMessagingComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(OsdkDynamicMessagingComponentCommand componentCommand)
        {
            return OsdkDynamicMessagingComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(OsdkDynamicMessagingComponentCommand componentCommand)
        {
            return OsdkDynamicMessagingComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(OsdkDynamicMessagingComponentNotification notification)
        {
            return OsdkDynamicMessagingComponentBase.GetNotificationType(notification);
        }
    }

    public class Client : BlazeClientComponent<OsdkDynamicMessagingComponentCommand, OsdkDynamicMessagingComponentNotification, Blaze3RpcError>
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public Client(BlazeClientConnection connection) : base(OsdkDynamicMessagingComponentBase.Id, OsdkDynamicMessagingComponentBase.Name)
        {
            Connection = connection;
            if (!Connection.Config.AddComponent(this))
                throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
        }

        private BlazeClientConnection Connection { get; }

        public ExampleResponse GetConfig(NullStruct request)
        {
            return Connection.SendRequest<NullStruct, ExampleResponse, NullStruct>(this, (ushort)OsdkDynamicMessagingComponentCommand.getConfig, request);
        }

        public Task<ExampleResponse> GetConfigAsync(NullStruct request)
        {
            return Connection.SendRequestAsync<NullStruct, ExampleResponse, NullStruct>(this, (ushort)OsdkDynamicMessagingComponentCommand.getConfig, request);
        }

        public MessageResponse GetMessages(MessageRequest request)
        {
            return Connection.SendRequest<MessageRequest, MessageResponse, NullStruct>(this, (ushort)OsdkDynamicMessagingComponentCommand.getMessages, request);
        }

        public Task<MessageResponse> GetMessagesAsync(MessageRequest request)
        {
            return Connection.SendRequestAsync<MessageRequest, MessageResponse, NullStruct>(this, (ushort)OsdkDynamicMessagingComponentCommand.getMessages, request);
        }


        public override Type GetCommandRequestType(OsdkDynamicMessagingComponentCommand componentCommand)
        {
            return OsdkDynamicMessagingComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(OsdkDynamicMessagingComponentCommand componentCommand)
        {
            return OsdkDynamicMessagingComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(OsdkDynamicMessagingComponentCommand componentCommand)
        {
            return OsdkDynamicMessagingComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(OsdkDynamicMessagingComponentNotification notification)
        {
            return OsdkDynamicMessagingComponentBase.GetNotificationType(notification);
        }
    }

    public class Proxy : BlazeProxyComponent<OsdkDynamicMessagingComponentCommand, OsdkDynamicMessagingComponentNotification, Blaze3RpcError>
    {
        public Proxy() : base(OsdkDynamicMessagingComponentBase.Id, OsdkDynamicMessagingComponentBase.Name)
        {
        }

        [BlazeCommand((ushort)OsdkDynamicMessagingComponentCommand.getConfig)]
        public virtual Task<ExampleResponse> GetConfigAsync(NullStruct request, BlazeProxyContext context)
        {
            return context.ClientConnection.SendRequestAsync<NullStruct, ExampleResponse, NullStruct>(this, (ushort)OsdkDynamicMessagingComponentCommand.getConfig, request);
        }

        [BlazeCommand((ushort)OsdkDynamicMessagingComponentCommand.getMessages)]
        public virtual Task<MessageResponse> GetMessagesAsync(MessageRequest request, BlazeProxyContext context)
        {
            return context.ClientConnection.SendRequestAsync<MessageRequest, MessageResponse, NullStruct>(this, (ushort)OsdkDynamicMessagingComponentCommand.getMessages, request);
        }

        public override Type GetCommandRequestType(OsdkDynamicMessagingComponentCommand componentCommand)
        {
            return OsdkDynamicMessagingComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(OsdkDynamicMessagingComponentCommand componentCommand)
        {
            return OsdkDynamicMessagingComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(OsdkDynamicMessagingComponentCommand componentCommand)
        {
            return OsdkDynamicMessagingComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(OsdkDynamicMessagingComponentNotification notification)
        {
            return OsdkDynamicMessagingComponentBase.GetNotificationType(notification);
        }
    }
}