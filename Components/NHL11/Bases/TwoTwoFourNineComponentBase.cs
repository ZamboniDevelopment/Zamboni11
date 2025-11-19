using System;
using System.Threading.Tasks;
using Blaze3SDK;
using BlazeCommon;
using NLog;

namespace Zamboni11.Components.NHL11.Bases;

public static class TwoTwoFourNineComponentBase
{
    public enum TwoTwoFourNineComponentCommand : ushort
    {
        commandOne = 1,
        commandTwo = 2
    }

    public enum TwoTwoFourNineComponentNotification : ushort
    {
    }

    public const ushort Id = 2249;
    public const string Name = "TwoTwoFourNineComponent";

    public static Type GetCommandRequestType(TwoTwoFourNineComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            TwoTwoFourNineComponentCommand.commandOne => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetCommandResponseType(TwoTwoFourNineComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            TwoTwoFourNineComponentCommand.commandOne => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetCommandErrorResponseType(TwoTwoFourNineComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            TwoTwoFourNineComponentCommand.commandOne => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetNotificationType(TwoTwoFourNineComponentNotification notification)
    {
        return notification switch
        {
            _ => typeof(NullStruct)
        };
    }

    public class Server : BlazeServerComponent<TwoTwoFourNineComponentCommand, TwoTwoFourNineComponentNotification, Blaze3RpcError>
    {
        public Server() : base(TwoTwoFourNineComponentBase.Id, TwoTwoFourNineComponentBase.Name)
        {
        }

        [BlazeCommand((ushort)TwoTwoFourNineComponentCommand.commandOne)]
        public virtual Task<NullStruct> CommandOneAsync(NullStruct request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)TwoTwoFourNineComponentCommand.commandTwo)]
        public virtual Task<NullStruct> CommandTwoAsync(NullStruct request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        public override Type GetCommandRequestType(TwoTwoFourNineComponentCommand componentCommand)
        {
            return TwoTwoFourNineComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(TwoTwoFourNineComponentCommand componentCommand)
        {
            return TwoTwoFourNineComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(TwoTwoFourNineComponentCommand componentCommand)
        {
            return TwoTwoFourNineComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(TwoTwoFourNineComponentNotification notification)
        {
            return TwoTwoFourNineComponentBase.GetNotificationType(notification);
        }
    }

    public class Client : BlazeClientComponent<TwoTwoFourNineComponentCommand, TwoTwoFourNineComponentNotification, Blaze3RpcError>
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public Client(BlazeClientConnection connection) : base(TwoTwoFourNineComponentBase.Id, TwoTwoFourNineComponentBase.Name)
        {
            Connection = connection;
            if (!Connection.Config.AddComponent(this))
                throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
        }

        private BlazeClientConnection Connection { get; }

        public NullStruct CommandOne(NullStruct request)
        {
            return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)TwoTwoFourNineComponentCommand.commandOne, request);
        }

        public Task<NullStruct> CommandOneAsync(NullStruct request)
        {
            return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TwoTwoFourNineComponentCommand.commandOne, request);
        }

        public NullStruct CommandTwo(NullStruct request)
        {
            return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)TwoTwoFourNineComponentCommand.commandTwo, request);
        }

        public Task<NullStruct> CommandTwoAsync(NullStruct request)
        {
            return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TwoTwoFourNineComponentCommand.commandTwo, request);
        }

        public override Type GetCommandRequestType(TwoTwoFourNineComponentCommand componentCommand)
        {
            return TwoTwoFourNineComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(TwoTwoFourNineComponentCommand componentCommand)
        {
            return TwoTwoFourNineComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(TwoTwoFourNineComponentCommand componentCommand)
        {
            return TwoTwoFourNineComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(TwoTwoFourNineComponentNotification notification)
        {
            return TwoTwoFourNineComponentBase.GetNotificationType(notification);
        }
    }

    public class Proxy : BlazeProxyComponent<TwoTwoFourNineComponentCommand, TwoTwoFourNineComponentNotification, Blaze3RpcError>
    {
        public Proxy() : base(TwoTwoFourNineComponentBase.Id, TwoTwoFourNineComponentBase.Name)
        {
        }

        [BlazeCommand((ushort)TwoTwoFourNineComponentCommand.commandOne)]
        public virtual Task<NullStruct> CommandOne(NullStruct request, BlazeProxyContext context)
        {
            return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TwoTwoFourNineComponentCommand.commandOne, request);
        }

        [BlazeCommand((ushort)TwoTwoFourNineComponentCommand.commandTwo)]
        public virtual Task<NullStruct> CommandTwo(NullStruct request, BlazeProxyContext context)
        {
            return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TwoTwoFourNineComponentCommand.commandTwo, request);
        }

        public override Type GetCommandRequestType(TwoTwoFourNineComponentCommand componentCommand)
        {
            return TwoTwoFourNineComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(TwoTwoFourNineComponentCommand componentCommand)
        {
            return TwoTwoFourNineComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(TwoTwoFourNineComponentCommand componentCommand)
        {
            return TwoTwoFourNineComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(TwoTwoFourNineComponentNotification notification)
        {
            return TwoTwoFourNineComponentBase.GetNotificationType(notification);
        }
    }
}