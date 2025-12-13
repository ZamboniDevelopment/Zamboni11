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

public static class EAHockeyLeagueComponentBase
{
    public enum EAHockeyLeagueComponentCommand : ushort
    {
        getSeasonConfiguration = 1,
        getSeasonDetails = 3,
    }

    public enum EAHockeyLeagueComponentNotification : ushort
    {
    }

    public const ushort Id = 2257;
    public const string Name = "EAHockeyLeagueComponent";

    public static Type GetCommandRequestType(EAHockeyLeagueComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            EAHockeyLeagueComponentCommand.getSeasonConfiguration => typeof(NullStruct),
            EAHockeyLeagueComponentCommand.getSeasonDetails => typeof(SeasonDetailsRequest),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetCommandResponseType(EAHockeyLeagueComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            EAHockeyLeagueComponentCommand.getSeasonConfiguration => typeof(GetSeasonConfigurationResponse),
            EAHockeyLeagueComponentCommand.getSeasonDetails => typeof(SeasonDetailsResponse),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetCommandErrorResponseType(EAHockeyLeagueComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            EAHockeyLeagueComponentCommand.getSeasonConfiguration => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetNotificationType(EAHockeyLeagueComponentNotification notification)
    {
        return notification switch
        {
            _ => typeof(NullStruct)
        };
    }

    public class Server : BlazeServerComponent<EAHockeyLeagueComponentCommand, EAHockeyLeagueComponentNotification, Blaze3RpcError>
    {
        public Server() : base(EAHockeyLeagueComponentBase.Id, EAHockeyLeagueComponentBase.Name)
        {
        }

        [BlazeCommand((ushort)EAHockeyLeagueComponentCommand.getSeasonConfiguration)]
        public virtual Task<GetSeasonConfigurationResponse> SeasonConfigurationRequestAsync(NullStruct request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }
        
        [BlazeCommand((ushort)EAHockeyLeagueComponentCommand.getSeasonDetails)]
        public virtual Task<SeasonDetailsResponse> SeasonDetailsRequestAsync(SeasonDetailsRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }


        public override Type GetCommandRequestType(EAHockeyLeagueComponentCommand componentCommand)
        {
            return EAHockeyLeagueComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(EAHockeyLeagueComponentCommand componentCommand)
        {
            return EAHockeyLeagueComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(EAHockeyLeagueComponentCommand componentCommand)
        {
            return EAHockeyLeagueComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(EAHockeyLeagueComponentNotification notification)
        {
            return EAHockeyLeagueComponentBase.GetNotificationType(notification);
        }
    }

    public class Client : BlazeClientComponent<EAHockeyLeagueComponentCommand, EAHockeyLeagueComponentNotification, Blaze3RpcError>
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public Client(BlazeClientConnection connection) : base(EAHockeyLeagueComponentBase.Id, EAHockeyLeagueComponentBase.Name)
        {
            Connection = connection;
            if (!Connection.Config.AddComponent(this))
                throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
        }

        private BlazeClientConnection Connection { get; }

        public GetSeasonConfigurationResponse SeasonConfigurationRequest(NullStruct request)
        {
            return Connection.SendRequest<NullStruct, GetSeasonConfigurationResponse, NullStruct>(this, (ushort)EAHockeyLeagueComponentCommand.getSeasonConfiguration, request);
        }

        public Task<GetSeasonConfigurationResponse> SeasonConfigurationAsync(NullStruct request)
        {
            return Connection.SendRequestAsync<NullStruct, GetSeasonConfigurationResponse, NullStruct>(this, (ushort)EAHockeyLeagueComponentCommand.getSeasonConfiguration, request);
        }
        
        public SeasonDetailsResponse SeasonDetailsRequest(SeasonDetailsRequest request)
        {
            return Connection.SendRequest<SeasonDetailsRequest, SeasonDetailsResponse, NullStruct>(this, (ushort)EAHockeyLeagueComponentCommand.getSeasonDetails, request);
        }

        public Task<SeasonDetailsResponse> SeasonDetailsRequestAsync(SeasonDetailsRequest request)
        {
            return Connection.SendRequestAsync<SeasonDetailsRequest, SeasonDetailsResponse, NullStruct>(this, (ushort)EAHockeyLeagueComponentCommand.getSeasonDetails, request);
        }

        public override Type GetCommandRequestType(EAHockeyLeagueComponentCommand componentCommand)
        {
            return EAHockeyLeagueComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(EAHockeyLeagueComponentCommand componentCommand)
        {
            return EAHockeyLeagueComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(EAHockeyLeagueComponentCommand componentCommand)
        {
            return EAHockeyLeagueComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(EAHockeyLeagueComponentNotification notification)
        {
            return EAHockeyLeagueComponentBase.GetNotificationType(notification);
        }
    }

    public class Proxy : BlazeProxyComponent<EAHockeyLeagueComponentCommand, EAHockeyLeagueComponentNotification, Blaze3RpcError>
    {
        public Proxy() : base(EAHockeyLeagueComponentBase.Id, EAHockeyLeagueComponentBase.Name)
        {
        }

        [BlazeCommand((ushort)EAHockeyLeagueComponentCommand.getSeasonConfiguration)]
        public virtual Task<GetSeasonConfigurationResponse> SeasonConfigurationRequest(NullStruct request, BlazeProxyContext context)
        {
            return context.ClientConnection.SendRequestAsync<NullStruct, GetSeasonConfigurationResponse, NullStruct>(this, (ushort)EAHockeyLeagueComponentCommand.getSeasonConfiguration, request);
        }
        
        [BlazeCommand((ushort)EAHockeyLeagueComponentCommand.getSeasonConfiguration)]
        public virtual Task<SeasonDetailsResponse> ThreeAsync(SeasonDetailsRequest request, BlazeProxyContext context)
        {
            return context.ClientConnection.SendRequestAsync<SeasonDetailsRequest, SeasonDetailsResponse, NullStruct>(this, (ushort)EAHockeyLeagueComponentCommand.getSeasonDetails, request);
        }

        public override Type GetCommandRequestType(EAHockeyLeagueComponentCommand componentCommand)
        {
            return EAHockeyLeagueComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(EAHockeyLeagueComponentCommand componentCommand)
        {
            return EAHockeyLeagueComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(EAHockeyLeagueComponentCommand componentCommand)
        {
            return EAHockeyLeagueComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(EAHockeyLeagueComponentNotification notification)
        {
            return EAHockeyLeagueComponentBase.GetNotificationType(notification);
        }
    }
}