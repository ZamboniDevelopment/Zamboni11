using System;
using System.Threading.Tasks;
using Blaze3SDK;
using Blaze3SDK.Blaze.Example;
using BlazeCommon;
using NLog;
using Zamboni11.Components.NHL11.Requests;
using Zamboni11.Components.NHL11.Responses;

namespace Zamboni11.Components.NHL11.Bases;

public static class CardHouseComponentBase
{
    public enum CardHouseComponentCommand : ushort
    {
        login = 101,
        logout = 102,
        gamerSetInfo = 103,
        gamerGetInfo = 104,
        getConfig = 106,
        getDeckInfo = 301,
        createPack = 401,
        viewCards = 402,
        discardCard = 403,
        getStaffBonus = 408,
        squadSave = 708,
        getSquadList = 709,
        squadLoadActive = 711,
        stickerBookStats2 = 800,
        stickerBookSearch = 802,
        getUserReliabilityInfo = 1002
    }

    public enum CardHouseComponentNotification : ushort
    {
    }

    public const ushort Id = 2148;
    public const string Name = "CardHouseComponent";

    public static Type GetCommandRequestType(CardHouseComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            CardHouseComponentCommand.login => typeof(LoginRequest),
            CardHouseComponentCommand.logout => typeof(LogoutRequest),
            CardHouseComponentCommand.gamerSetInfo => typeof(GamerSetInfoRequest),
            CardHouseComponentCommand.gamerGetInfo => typeof(GamerGetInfoRequest),
            CardHouseComponentCommand.getConfig => typeof(ProvidedUID),
            CardHouseComponentCommand.getDeckInfo => typeof(DeckInfoRequest),
            CardHouseComponentCommand.createPack => typeof(CreatePackRequest),
            CardHouseComponentCommand.viewCards => typeof(ViewCardsRequest),
            CardHouseComponentCommand.discardCard => typeof(DiscardCardRequest),
            CardHouseComponentCommand.getStaffBonus => typeof(ProvidedUID),
            CardHouseComponentCommand.squadSave => typeof(SquadSaveRequest),
            CardHouseComponentCommand.getSquadList => typeof(ProvidedUID),
            CardHouseComponentCommand.squadLoadActive => typeof(SquadLoadActiveRequest),
            CardHouseComponentCommand.stickerBookStats2 => typeof(StickerBookStats2Request),
            CardHouseComponentCommand.stickerBookSearch => typeof(StickerBookSearchRequest),
            CardHouseComponentCommand.getUserReliabilityInfo => typeof(ProvidedUID),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetCommandResponseType(CardHouseComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            CardHouseComponentCommand.login => typeof(LoginResponse),
            CardHouseComponentCommand.logout => typeof(ExampleResponse),
            CardHouseComponentCommand.gamerSetInfo => typeof(NumericResponse),
            CardHouseComponentCommand.gamerGetInfo => typeof(GamerGetInfoResponse),
            CardHouseComponentCommand.getConfig => typeof(ConfigResponse),
            CardHouseComponentCommand.getDeckInfo => typeof(DeckInfoResponse),
            CardHouseComponentCommand.createPack => typeof(CreatePackResponse),
            CardHouseComponentCommand.viewCards => typeof(ViewCardsResponse),
            CardHouseComponentCommand.discardCard => typeof(DiscardCardResponse),
            CardHouseComponentCommand.getStaffBonus => typeof(StaffBonusResponse),
            CardHouseComponentCommand.squadSave => typeof(SquadSaveResponse),
            CardHouseComponentCommand.getSquadList => typeof(SquadListResponse),
            CardHouseComponentCommand.squadLoadActive => typeof(SquadLoadActiveResponse),
            CardHouseComponentCommand.stickerBookStats2 => typeof(StickerBookStats2Response),
            CardHouseComponentCommand.stickerBookSearch => typeof(StickerBookSearchResponse),
            CardHouseComponentCommand.getUserReliabilityInfo => typeof(UserReliabilityInfoResponse),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetCommandErrorResponseType(CardHouseComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            CardHouseComponentCommand.login => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetNotificationType(CardHouseComponentNotification notification)
    {
        return notification switch
        {
            _ => typeof(NullStruct)
        };
    }

    public class Server : BlazeServerComponent<CardHouseComponentCommand, CardHouseComponentNotification, Blaze3RpcError>
    {
        public Server() : base(CardHouseComponentBase.Id, CardHouseComponentBase.Name)
        {
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.login)]
        public virtual Task<LoginResponse> LoginRequestAsync(LoginRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.logout)]
        public virtual Task<ExampleResponse> LogoutRequestAsync(LogoutRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.gamerSetInfo)]
        public virtual Task<NumericResponse> SetGamerInfoRequestAsync(GamerSetInfoRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.gamerGetInfo)]
        public virtual Task<GamerGetInfoResponse> GetGamerInfoRequestAsync(GamerGetInfoRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.getConfig)]
        public virtual Task<ConfigResponse> GetConfigRequestAsync(ProvidedUID request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.getDeckInfo)]
        public virtual Task<DeckInfoResponse> GetDeckInfoAsync(DeckInfoRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.createPack)]
        public virtual Task<CreatePackResponse> CreatePackAsync(CreatePackRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.viewCards)]
        public virtual Task<ViewCardsResponse> ViewCardsAsync(ViewCardsRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.discardCard)]
        public virtual Task<DiscardCardResponse> DiscardCardAsync(DiscardCardRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.getStaffBonus)]
        public virtual Task<StaffBonusResponse> GetStaffBonusAsync(ProvidedUID request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.squadSave)]
        public virtual Task<SquadSaveResponse> SquadSaveAsync(SquadSaveRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.getSquadList)]
        public virtual Task<SquadListResponse> GetSquadListAsync(ProvidedUID request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.squadLoadActive)]
        public virtual Task<SquadLoadActiveResponse> SquadLoadActiveAsync(SquadLoadActiveRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.stickerBookSearch)]
        public virtual Task<StickerBookSearchResponse> StickerBookSearchAsync(StickerBookSearchRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.stickerBookStats2)]
        public virtual Task<StickerBookStats2Response> StickerBookStats2Async(StickerBookStats2Request request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.getUserReliabilityInfo)]
        public virtual Task<UserReliabilityInfoResponse> GetUserReliabilityInfoAsync(ProvidedUID request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        public override Type GetCommandRequestType(CardHouseComponentCommand componentCommand)
        {
            return CardHouseComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(CardHouseComponentCommand componentCommand)
        {
            return CardHouseComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(CardHouseComponentCommand componentCommand)
        {
            return CardHouseComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(CardHouseComponentNotification notification)
        {
            return CardHouseComponentBase.GetNotificationType(notification);
        }
    }

    public class Client : BlazeClientComponent<CardHouseComponentCommand, CardHouseComponentNotification, Blaze3RpcError>
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public Client(BlazeClientConnection connection) : base(CardHouseComponentBase.Id, CardHouseComponentBase.Name)
        {
            Connection = connection;
            if (!Connection.Config.AddComponent(this))
                throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
        }

        private BlazeClientConnection Connection { get; }

        public LoginResponse LoginRequest(LoginRequest request)
        {
            return Connection.SendRequest<LoginRequest, LoginResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.login, request);
        }

        public Task<LoginResponse> LoginRequestAsync(LoginRequest request)
        {
            return Connection.SendRequestAsync<LoginRequest, LoginResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.login, request);
        }

        public NumericResponse SetGamerInfoRequest(GamerSetInfoRequest request)
        {
            return Connection.SendRequest<GamerSetInfoRequest, NumericResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.gamerSetInfo, request);
        }

        public Task<NumericResponse> SetGamerInfoRequestAsync(GamerSetInfoRequest request)
        {
            return Connection.SendRequestAsync<GamerSetInfoRequest, NumericResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.gamerSetInfo, request);
        }

        public GamerGetInfoResponse GetGamerInfoRequest(GamerGetInfoRequest request)
        {
            return Connection.SendRequest<GamerGetInfoRequest, GamerGetInfoResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.gamerGetInfo, request);
        }

        public Task<GamerGetInfoResponse> GetGamerInfoRequestAsync(GamerGetInfoRequest request)
        {
            return Connection.SendRequestAsync<GamerGetInfoRequest, GamerGetInfoResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.gamerGetInfo, request);
        }

        public ConfigResponse GetConfigRequest(ProvidedUID request)
        {
            return Connection.SendRequest<ProvidedUID, ConfigResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.getConfig, request);
        }

        public Task<ConfigResponse> GetConfigRequestAsync(ProvidedUID request)
        {
            return Connection.SendRequestAsync<ProvidedUID, ConfigResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.getConfig, request);
        }

        public DeckInfoResponse GetDeckInfo(DeckInfoRequest request)
        {
            return Connection.SendRequest<DeckInfoRequest, DeckInfoResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.getDeckInfo, request);
        }

        public Task<DeckInfoResponse> GetDeckInfoAsync(DeckInfoRequest request)
        {
            return Connection.SendRequestAsync<DeckInfoRequest, DeckInfoResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.getDeckInfo, request);
        }

        public CreatePackResponse CreatePack(CreatePackRequest request)
        {
            return Connection.SendRequest<CreatePackRequest, CreatePackResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.createPack, request);
        }

        public Task<CreatePackResponse> CreatePackAsync(CreatePackRequest request)
        {
            return Connection.SendRequestAsync<CreatePackRequest, CreatePackResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.createPack, request);
        }

        public ViewCardsResponse ViewCards(ViewCardsRequest request)
        {
            return Connection.SendRequest<ViewCardsRequest, ViewCardsResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.viewCards, request);
        }

        public Task<ViewCardsResponse> ViewCardsAsync(ViewCardsRequest request)
        {
            return Connection.SendRequestAsync<ViewCardsRequest, ViewCardsResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.viewCards, request);
        }

        public DiscardCardResponse DiscardCard(DiscardCardRequest request)
        {
            return Connection.SendRequest<DiscardCardRequest, DiscardCardResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.discardCard, request);
        }

        public Task<DiscardCardResponse> DiscardCardAsync(DiscardCardRequest request)
        {
            return Connection.SendRequestAsync<DiscardCardRequest, DiscardCardResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.discardCard, request);
        }

        public StaffBonusResponse GetStaffBonusRequest(ProvidedUID request)
        {
            return Connection.SendRequest<ProvidedUID, StaffBonusResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.getStaffBonus, request);
        }

        public Task<StaffBonusResponse> GetStaffBonusRequestAsync(ProvidedUID request)
        {
            return Connection.SendRequestAsync<ProvidedUID, StaffBonusResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.getStaffBonus, request);
        }

        public SquadSaveResponse SquadSaveRequest(SquadSaveRequest request)
        {
            return Connection.SendRequest<SquadSaveRequest, SquadSaveResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.squadSave, request);
        }

        public Task<SquadSaveResponse> SquadSaveRequestAsync(SquadSaveRequest request)
        {
            return Connection.SendRequestAsync<SquadSaveRequest, SquadSaveResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.squadSave, request);
        }

        public SquadListResponse GetSquadListRequest(ProvidedUID request)
        {
            return Connection.SendRequest<ProvidedUID, SquadListResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.getSquadList, request);
        }

        public Task<SquadListResponse> GetSquadListRequestAsync(ProvidedUID request)
        {
            return Connection.SendRequestAsync<ProvidedUID, SquadListResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.getSquadList, request);
        }

        public StickerBookSearchResponse StickerBookSearch(StickerBookSearchRequest request)
        {
            return Connection.SendRequest<StickerBookSearchRequest, StickerBookSearchResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.stickerBookSearch, request);
        }

        public Task<StickerBookSearchResponse> StickerBookSearchAsync(StickerBookSearchRequest request)
        {
            return Connection.SendRequestAsync<StickerBookSearchRequest, StickerBookSearchResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.stickerBookSearch, request);
        }


        public StickerBookStats2Response GetStickerBookStats2(StickerBookStats2Request request)
        {
            return Connection.SendRequest<StickerBookStats2Request, StickerBookStats2Response, NullStruct>(this, (ushort)CardHouseComponentCommand.stickerBookStats2, request);
        }

        public Task<StickerBookStats2Response> GetStickerBookStats2Async(StickerBookStats2Request request)
        {
            return Connection.SendRequestAsync<StickerBookStats2Request, StickerBookStats2Response, NullStruct>(this, (ushort)CardHouseComponentCommand.stickerBookStats2, request);
        }

        public SquadLoadActiveResponse SquadLoadActive(SquadLoadActiveRequest request)
        {
            return Connection.SendRequest<SquadLoadActiveRequest, SquadLoadActiveResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.squadLoadActive, request);
        }

        public Task<SquadLoadActiveResponse> SquadLoadActiveAsync(SquadLoadActiveRequest request)
        {
            return Connection.SendRequestAsync<SquadLoadActiveRequest, SquadLoadActiveResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.squadLoadActive, request);
        }

        public UserReliabilityInfoResponse GetUserReliabilityRequest(ProvidedUID request)
        {
            return Connection.SendRequest<ProvidedUID, UserReliabilityInfoResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.getUserReliabilityInfo, request);
        }

        public Task<UserReliabilityInfoResponse> GetUserReliabilityRequestAsync(ProvidedUID request)
        {
            return Connection.SendRequestAsync<ProvidedUID, UserReliabilityInfoResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.getUserReliabilityInfo, request);
        }

        public override Type GetCommandRequestType(CardHouseComponentCommand componentCommand)
        {
            return CardHouseComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(CardHouseComponentCommand componentCommand)
        {
            return CardHouseComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(CardHouseComponentCommand componentCommand)
        {
            return CardHouseComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(CardHouseComponentNotification notification)
        {
            return CardHouseComponentBase.GetNotificationType(notification);
        }
    }
}