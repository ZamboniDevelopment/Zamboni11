using System;
using System.Threading.Tasks;
using Blaze3SDK;
using BlazeCommon;
using NLog;
using Zamboni11.Components.NHL11.Requests;
using Zamboni11.Components.NHL11.Responses;
using Zamboni11.Components.NHL11.Structs;

namespace Zamboni11.Components.NHL11.Bases;

public static class OsdkTicker2ComponentBase
{
    public enum OsdkTicker2ComponentCommand : ushort
    {
        registerArgs = 2,
        getMessages = 3,
        updateFilters = 4,
    }

    public enum OsdkTicker2ComponentNotification : ushort
    {
    }

    public const ushort Id = 2262;
    public const string Name = "OsdkTicker2Component";

    public static Type GetCommandRequestType(OsdkTicker2ComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            OsdkTicker2ComponentCommand.registerArgs => typeof(RegisterArgs),
            OsdkTicker2ComponentCommand.getMessages => typeof(GetMessagesRequest),
            OsdkTicker2ComponentCommand.updateFilters => typeof(UpdateFiltersRequest),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetCommandResponseType(OsdkTicker2ComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            OsdkTicker2ComponentCommand.registerArgs => typeof(RegisterResponse),
            OsdkTicker2ComponentCommand.getMessages => typeof(GetMessagesResponse),
            OsdkTicker2ComponentCommand.updateFilters => typeof(TickerFilter),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetCommandErrorResponseType(OsdkTicker2ComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            OsdkTicker2ComponentCommand.registerArgs => typeof(NullStruct),
            OsdkTicker2ComponentCommand.getMessages => typeof(NullStruct),
            OsdkTicker2ComponentCommand.updateFilters => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetNotificationType(OsdkTicker2ComponentNotification notification)
    {
        return notification switch
        {
            _ => typeof(NullStruct)
        };
    }

    public class Server : BlazeServerComponent<OsdkTicker2ComponentCommand, OsdkTicker2ComponentNotification, Blaze3RpcError>
    {
        public Server() : base(OsdkTicker2ComponentBase.Id, OsdkTicker2ComponentBase.Name)
        {
        }

        [BlazeCommand((ushort)OsdkTicker2ComponentCommand.registerArgs)]
        public virtual Task<RegisterResponse> RegisterArgsAsync(RegisterArgs request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)OsdkTicker2ComponentCommand.getMessages)]
        public virtual Task<GetMessagesResponse> GetMessagesAsync(GetMessagesRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }
        
        [BlazeCommand((ushort)OsdkTicker2ComponentCommand.updateFilters)]
        public virtual Task<TickerFilter> UpdateFiltersAsync(UpdateFiltersRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }
        public override Type GetCommandRequestType(OsdkTicker2ComponentCommand componentCommand)
        {
            return OsdkTicker2ComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(OsdkTicker2ComponentCommand componentCommand)
        {
            return OsdkTicker2ComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(OsdkTicker2ComponentCommand componentCommand)
        {
            return OsdkTicker2ComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(OsdkTicker2ComponentNotification notification)
        {
            return OsdkTicker2ComponentBase.GetNotificationType(notification);
        }
    }

    public class Client : BlazeClientComponent<OsdkTicker2ComponentCommand, OsdkTicker2ComponentNotification, Blaze3RpcError>
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public Client(BlazeClientConnection connection) : base(OsdkTicker2ComponentBase.Id, OsdkTicker2ComponentBase.Name)
        {
            Connection = connection;
            if (!Connection.Config.AddComponent(this))
                throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
        }

        private BlazeClientConnection Connection { get; }

        public RegisterResponse RegisterArgs(RegisterArgs request)
        {
            return Connection.SendRequest<RegisterArgs, RegisterResponse, NullStruct>(this, (ushort)OsdkTicker2ComponentCommand.registerArgs, request);
        }

        public Task<RegisterResponse> RegisterArgsAsync(RegisterArgs request)
        {
            return Connection.SendRequestAsync<RegisterArgs, RegisterResponse, NullStruct>(this, (ushort)OsdkTicker2ComponentCommand.registerArgs, request);
        }
        
        public GetMessagesResponse GetMessages(GetMessagesRequest request)
        {
            return Connection.SendRequest<GetMessagesRequest, GetMessagesResponse, NullStruct>(this, (ushort)OsdkTicker2ComponentCommand.getMessages, request);
        }

        public Task<GetMessagesResponse> GetMessagesAsync(GetMessagesRequest request)
        {
            return Connection.SendRequestAsync<GetMessagesRequest, GetMessagesResponse, NullStruct>(this, (ushort)OsdkTicker2ComponentCommand.getMessages, request);
        }
        
        public TickerFilter UpdateFilters(UpdateFiltersRequest request)
        {
            return Connection.SendRequest<UpdateFiltersRequest, TickerFilter, NullStruct>(this, (ushort)OsdkTicker2ComponentCommand.updateFilters, request);
        }

        public Task<TickerFilter> UpdateFiltersAsync(UpdateFiltersRequest request)
        {
            return Connection.SendRequestAsync<UpdateFiltersRequest, TickerFilter, NullStruct>(this, (ushort)OsdkTicker2ComponentCommand.updateFilters, request);
        }

        public override Type GetCommandRequestType(OsdkTicker2ComponentCommand componentCommand)
        {
            return OsdkTicker2ComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(OsdkTicker2ComponentCommand componentCommand)
        {
            return OsdkTicker2ComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(OsdkTicker2ComponentCommand componentCommand)
        {
            return OsdkTicker2ComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(OsdkTicker2ComponentNotification notification)
        {
            return OsdkTicker2ComponentBase.GetNotificationType(notification);
        }
    }
    
}