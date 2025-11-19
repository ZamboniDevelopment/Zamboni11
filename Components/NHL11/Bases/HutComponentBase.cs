using System;
using System.Threading.Tasks;
using Blaze3SDK;
using Blaze3SDK.Blaze.Authentication;
using Blaze3SDK.Blaze.Example;
using BlazeCommon;
using NLog;
using Zamboni11.Components.NHL11.Requests;
using Zamboni11.Components.NHL11.Responses;

namespace Zamboni11.Components.NHL11.Bases;

public static class HutComponentBase
{
    public enum HutComponentCommand : ushort
    {
        login = 101,
        logout = 102,
        gamerGetInfo = 104,
        getDeckInfo = 301,
        createPack = 401,
    }

    public enum HutComponentNotification : ushort
    {
    }

    public const ushort Id = 2148;
    public const string Name = "HutComponent";

    public static Type GetCommandRequestType(HutComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            HutComponentCommand.login => typeof(HutLoginRequest),
            HutComponentCommand.logout => typeof(HutLogoutRequest),
            HutComponentCommand.gamerGetInfo => typeof(GamerGetInfoRequest),
            HutComponentCommand.getDeckInfo => typeof(DeckInfoRequest),
            HutComponentCommand.createPack => typeof(CreatePackRequest),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetCommandResponseType(HutComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            HutComponentCommand.login => typeof(HutLoginResponse),
            HutComponentCommand.logout => typeof(ExampleResponse),
            HutComponentCommand.gamerGetInfo => typeof(GamerGetInfoResponse),
            HutComponentCommand.getDeckInfo => typeof(DeckInfoResponse),
            HutComponentCommand.createPack => typeof(ExampleResponse),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetCommandErrorResponseType(HutComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            HutComponentCommand.login => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetNotificationType(HutComponentNotification notification)
    {
        return notification switch
        {
            _ => typeof(NullStruct)
        };
    }

    public class Server : BlazeServerComponent<HutComponentCommand, HutComponentNotification, Blaze3RpcError>
    {
        public Server() : base(HutComponentBase.Id, HutComponentBase.Name)
        {
        }

        [BlazeCommand((ushort)HutComponentCommand.login)]
        public virtual Task<HutLoginResponse> LoginRequestAsync(HutLoginRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }
        
        [BlazeCommand((ushort)HutComponentCommand.logout)]
        public virtual Task<ExampleResponse> LogoutRequestAsync(HutLogoutRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        
        [BlazeCommand((ushort)HutComponentCommand.gamerGetInfo)]
        public virtual Task<GamerGetInfoResponse> GetGamerInfoRequestAsync(GamerGetInfoRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }
        
        [BlazeCommand((ushort)HutComponentCommand.getDeckInfo)]
        public virtual Task<DeckInfoResponse> GetDeckInfoAsync(DeckInfoRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }
        
        [BlazeCommand((ushort)HutComponentCommand.createPack)]
        public virtual Task<ExampleResponse> CreatePackAsync(CreatePackRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }
        
        public override Type GetCommandRequestType(HutComponentCommand componentCommand)
        {
            return HutComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(HutComponentCommand componentCommand)
        {
            return HutComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(HutComponentCommand componentCommand)
        {
            return HutComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(HutComponentNotification notification)
        {
            return HutComponentBase.GetNotificationType(notification);
        }
    }

    public class Client : BlazeClientComponent<HutComponentCommand, HutComponentNotification, Blaze3RpcError>
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public Client(BlazeClientConnection connection) : base(HutComponentBase.Id, HutComponentBase.Name)
        {
            Connection = connection;
            if (!Connection.Config.AddComponent(this))
                throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
        }

        private BlazeClientConnection Connection { get; }

        public HutLoginResponse LoginRequest(HutLoginRequest request)
        {
            return Connection.SendRequest<HutLoginRequest, HutLoginResponse, NullStruct>(this, (ushort)HutComponentCommand.login, request);
        }

        public Task<HutLoginResponse> LoginRequestAsync(HutLoginRequest request)
        {
            return Connection.SendRequestAsync<HutLoginRequest, HutLoginResponse, NullStruct>(this, (ushort)HutComponentCommand.login, request);
        }
        
        public GamerGetInfoResponse GetGamerInfoRequest(GamerGetInfoRequest request)
        {
            return Connection.SendRequest<GamerGetInfoRequest, GamerGetInfoResponse, NullStruct>(this, (ushort)HutComponentCommand.gamerGetInfo, request);
        }

        public Task<GamerGetInfoResponse> GetGamerInfoRequestAsync(GamerGetInfoRequest request)
        {
            return Connection.SendRequestAsync<GamerGetInfoRequest, GamerGetInfoResponse, NullStruct>(this, (ushort)HutComponentCommand.gamerGetInfo, request);
        }
        
        public DeckInfoResponse GetDeckInfo(DeckInfoRequest request)
        {
            return Connection.SendRequest<DeckInfoRequest, DeckInfoResponse, NullStruct>(this, (ushort)HutComponentCommand.getDeckInfo, request);
        }

        public Task<DeckInfoResponse> GetDeckInfoAsync(DeckInfoRequest request)
        {
            return Connection.SendRequestAsync<DeckInfoRequest, DeckInfoResponse, NullStruct>(this, (ushort)HutComponentCommand.getDeckInfo, request);
        }
        
        public ExampleResponse CreatePack(CreatePackRequest request)
        {
            return Connection.SendRequest<CreatePackRequest, ExampleResponse, NullStruct>(this, (ushort)HutComponentCommand.createPack, request);
        }

        public Task<ExampleResponse> CreatePackAsync(CreatePackRequest request)
        {
            return Connection.SendRequestAsync<CreatePackRequest, ExampleResponse, NullStruct>(this, (ushort)HutComponentCommand.createPack, request);
        }
        
        public override Type GetCommandRequestType(HutComponentCommand componentCommand)
        {
            return HutComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(HutComponentCommand componentCommand)
        {
            return HutComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(HutComponentCommand componentCommand)
        {
            return HutComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(HutComponentNotification notification)
        {
            return HutComponentBase.GetNotificationType(notification);
        }
    }

    public class Proxy : BlazeProxyComponent<HutComponentCommand, HutComponentNotification, Blaze3RpcError>
    {
        public Proxy() : base(HutComponentBase.Id, HutComponentBase.Name)
        {
        }

        [BlazeCommand((ushort)HutComponentCommand.login)]
        public virtual Task<ExampleResponse> LoginRequestAsync(HutLoginRequest request, BlazeProxyContext context)
        {
            return context.ClientConnection.SendRequestAsync<HutLoginRequest, ExampleResponse, NullStruct>(this, (ushort)HutComponentCommand.login, request);
        }
        
        [BlazeCommand((ushort)HutComponentCommand.gamerGetInfo)]
        public virtual Task<GamerGetInfoResponse> GetGamerInfoRequestAsync(GamerGetInfoRequest request, BlazeProxyContext context)
        {
            return context.ClientConnection.SendRequestAsync<GamerGetInfoRequest, GamerGetInfoResponse, NullStruct>(this, (ushort)HutComponentCommand.gamerGetInfo, request);
        }
        
        [BlazeCommand((ushort)HutComponentCommand.getDeckInfo)]
        public virtual Task<DeckInfoResponse> GetDeckInfoAsync(DeckInfoRequest request, BlazeProxyContext context)
        {
            return context.ClientConnection.SendRequestAsync<DeckInfoRequest, DeckInfoResponse, NullStruct>(this, (ushort)HutComponentCommand.getDeckInfo, request);
        }
        
        [BlazeCommand((ushort)HutComponentCommand.createPack)]
        public virtual Task<ExampleResponse> CreatePackAsync(CreatePackRequest request, BlazeProxyContext context)
        {
            return context.ClientConnection.SendRequestAsync<CreatePackRequest, ExampleResponse, NullStruct>(this, (ushort)HutComponentCommand.getDeckInfo, request);
        }
        public override Type GetCommandRequestType(HutComponentCommand componentCommand)
        {
            return HutComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(HutComponentCommand componentCommand)
        {
            return HutComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(HutComponentCommand componentCommand)
        {
            return HutComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(HutComponentNotification notification)
        {
            return HutComponentBase.GetNotificationType(notification);
        }
    }
}