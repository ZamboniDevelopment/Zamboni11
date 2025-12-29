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
    public enum HutComponentCommand : ushort
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
            HutComponentCommand.gamerSetInfo => typeof(GamerSetInfoRequest),
            HutComponentCommand.gamerGetInfo => typeof(GamerGetInfoRequest),
            HutComponentCommand.getConfig => typeof(ProvidedUID),
            HutComponentCommand.getDeckInfo => typeof(DeckInfoRequest),
            HutComponentCommand.createPack => typeof(CreatePackRequest),
            HutComponentCommand.viewCards => typeof(ViewCardsRequest),
            HutComponentCommand.discardCard => typeof(DiscardCardRequest),
            HutComponentCommand.getStaffBonus => typeof(ProvidedUID),
            HutComponentCommand.squadSave => typeof(SquadSaveRequest),
            HutComponentCommand.getSquadList => typeof(ProvidedUID),
            HutComponentCommand.squadLoadActive => typeof(SquadLoadActiveRequest),
            HutComponentCommand.stickerBookStats2 => typeof(StickerBookStats2Request),
            HutComponentCommand.stickerBookSearch => typeof(StickerBookSearchRequest),
            HutComponentCommand.getUserReliabilityInfo => typeof(ProvidedUID),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetCommandResponseType(HutComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            HutComponentCommand.login => typeof(HutLoginResponse),
            HutComponentCommand.logout => typeof(ExampleResponse),
            HutComponentCommand.gamerSetInfo => typeof(NumericResponse),
            HutComponentCommand.gamerGetInfo => typeof(GamerGetInfoResponse),
            HutComponentCommand.getConfig => typeof(HutConfigResponse),
            HutComponentCommand.getDeckInfo => typeof(DeckInfoResponse),
            HutComponentCommand.createPack => typeof(CreatePackResponse),
            HutComponentCommand.viewCards => typeof(ViewCardsResponse),
            HutComponentCommand.discardCard => typeof(DiscardCardResponse),
            HutComponentCommand.getStaffBonus => typeof(StaffBonusResponse),
            HutComponentCommand.squadSave => typeof(SquadSaveResponse),
            HutComponentCommand.getSquadList => typeof(SquadListResponse),
            HutComponentCommand.squadLoadActive => typeof(SquadLoadActiveResponse),
            HutComponentCommand.stickerBookStats2 => typeof(StickerBookStats2Response),
            HutComponentCommand.stickerBookSearch => typeof(StickerBookSearchResponse),
            HutComponentCommand.getUserReliabilityInfo => typeof(UserReliabilityInfoResponse),
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
        public Server() : base(CardHouseComponentBase.Id, CardHouseComponentBase.Name)
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

        [BlazeCommand((ushort)HutComponentCommand.gamerSetInfo)]
        public virtual Task<NumericResponse> SetGamerInfoRequestAsync(GamerSetInfoRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)HutComponentCommand.gamerGetInfo)]
        public virtual Task<GamerGetInfoResponse> GetGamerInfoRequestAsync(GamerGetInfoRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)HutComponentCommand.getConfig)]
        public virtual Task<HutConfigResponse> GetHutConfigRequestAsync(ProvidedUID request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)HutComponentCommand.getDeckInfo)]
        public virtual Task<DeckInfoResponse> GetDeckInfoAsync(DeckInfoRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)HutComponentCommand.createPack)]
        public virtual Task<CreatePackResponse> CreatePackAsync(CreatePackRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)HutComponentCommand.viewCards)]
        public virtual Task<ViewCardsResponse> ViewCardsAsync(ViewCardsRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)HutComponentCommand.discardCard)]
        public virtual Task<DiscardCardResponse> DiscardCardAsync(DiscardCardRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)HutComponentCommand.getStaffBonus)]
        public virtual Task<StaffBonusResponse> GetStaffBonusAsync(ProvidedUID request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)HutComponentCommand.squadSave)]
        public virtual Task<SquadSaveResponse> SquadSaveAsync(SquadSaveRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)HutComponentCommand.getSquadList)]
        public virtual Task<SquadListResponse> GetSquadListAsync(ProvidedUID request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)HutComponentCommand.squadLoadActive)]
        public virtual Task<SquadLoadActiveResponse> SquadLoadActiveAsync(SquadLoadActiveRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)HutComponentCommand.stickerBookSearch)]
        public virtual Task<StickerBookSearchResponse> StickerBookSearchAsync(StickerBookSearchRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)HutComponentCommand.stickerBookStats2)]
        public virtual Task<StickerBookStats2Response> StickerBookStats2Async(StickerBookStats2Request request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)HutComponentCommand.getUserReliabilityInfo)]
        public virtual Task<UserReliabilityInfoResponse> GetUserReliabilityInfoAsync(ProvidedUID request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        public override Type GetCommandRequestType(HutComponentCommand componentCommand)
        {
            return CardHouseComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(HutComponentCommand componentCommand)
        {
            return CardHouseComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(HutComponentCommand componentCommand)
        {
            return CardHouseComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(HutComponentNotification notification)
        {
            return CardHouseComponentBase.GetNotificationType(notification);
        }
    }

    public class Client : BlazeClientComponent<HutComponentCommand, HutComponentNotification, Blaze3RpcError>
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public Client(BlazeClientConnection connection) : base(CardHouseComponentBase.Id, CardHouseComponentBase.Name)
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

        public NumericResponse SetGamerInfoRequest(GamerSetInfoRequest request)
        {
            return Connection.SendRequest<GamerSetInfoRequest, NumericResponse, NullStruct>(this, (ushort)HutComponentCommand.gamerSetInfo, request);
        }

        public Task<NumericResponse> SetGamerInfoRequestAsync(GamerSetInfoRequest request)
        {
            return Connection.SendRequestAsync<GamerSetInfoRequest, NumericResponse, NullStruct>(this, (ushort)HutComponentCommand.gamerSetInfo, request);
        }

        public GamerGetInfoResponse GetGamerInfoRequest(GamerGetInfoRequest request)
        {
            return Connection.SendRequest<GamerGetInfoRequest, GamerGetInfoResponse, NullStruct>(this, (ushort)HutComponentCommand.gamerGetInfo, request);
        }

        public Task<GamerGetInfoResponse> GetGamerInfoRequestAsync(GamerGetInfoRequest request)
        {
            return Connection.SendRequestAsync<GamerGetInfoRequest, GamerGetInfoResponse, NullStruct>(this, (ushort)HutComponentCommand.gamerGetInfo, request);
        }

        public HutConfigResponse GetHutConfigRequest(ProvidedUID request)
        {
            return Connection.SendRequest<ProvidedUID, HutConfigResponse, NullStruct>(this, (ushort)HutComponentCommand.getConfig, request);
        }

        public Task<HutConfigResponse> GetHutConfigRequestAsync(ProvidedUID request)
        {
            return Connection.SendRequestAsync<ProvidedUID, HutConfigResponse, NullStruct>(this, (ushort)HutComponentCommand.getConfig, request);
        }

        public DeckInfoResponse GetDeckInfo(DeckInfoRequest request)
        {
            return Connection.SendRequest<DeckInfoRequest, DeckInfoResponse, NullStruct>(this, (ushort)HutComponentCommand.getDeckInfo, request);
        }

        public Task<DeckInfoResponse> GetDeckInfoAsync(DeckInfoRequest request)
        {
            return Connection.SendRequestAsync<DeckInfoRequest, DeckInfoResponse, NullStruct>(this, (ushort)HutComponentCommand.getDeckInfo, request);
        }

        public CreatePackResponse CreatePack(CreatePackRequest request)
        {
            return Connection.SendRequest<CreatePackRequest, CreatePackResponse, NullStruct>(this, (ushort)HutComponentCommand.createPack, request);
        }

        public Task<CreatePackResponse> CreatePackAsync(CreatePackRequest request)
        {
            return Connection.SendRequestAsync<CreatePackRequest, CreatePackResponse, NullStruct>(this, (ushort)HutComponentCommand.createPack, request);
        }

        public ViewCardsResponse ViewCards(ViewCardsRequest request)
        {
            return Connection.SendRequest<ViewCardsRequest, ViewCardsResponse, NullStruct>(this, (ushort)HutComponentCommand.viewCards, request);
        }

        public Task<ViewCardsResponse> ViewCardsAsync(ViewCardsRequest request)
        {
            return Connection.SendRequestAsync<ViewCardsRequest, ViewCardsResponse, NullStruct>(this, (ushort)HutComponentCommand.viewCards, request);
        }

        public DiscardCardResponse DiscardCard(DiscardCardRequest request)
        {
            return Connection.SendRequest<DiscardCardRequest, DiscardCardResponse, NullStruct>(this, (ushort)HutComponentCommand.discardCard, request);
        }

        public Task<DiscardCardResponse> DiscardCardAsync(DiscardCardRequest request)
        {
            return Connection.SendRequestAsync<DiscardCardRequest, DiscardCardResponse, NullStruct>(this, (ushort)HutComponentCommand.discardCard, request);
        }

        public StaffBonusResponse GetStaffBonusRequest(ProvidedUID request)
        {
            return Connection.SendRequest<ProvidedUID, StaffBonusResponse, NullStruct>(this, (ushort)HutComponentCommand.getStaffBonus, request);
        }

        public Task<StaffBonusResponse> GetStaffBonusRequestAsync(ProvidedUID request)
        {
            return Connection.SendRequestAsync<ProvidedUID, StaffBonusResponse, NullStruct>(this, (ushort)HutComponentCommand.getStaffBonus, request);
        }

        public SquadSaveResponse SquadSaveRequest(SquadSaveRequest request)
        {
            return Connection.SendRequest<SquadSaveRequest, SquadSaveResponse, NullStruct>(this, (ushort)HutComponentCommand.squadSave, request);
        }

        public Task<SquadSaveResponse> SquadSaveRequestAsync(SquadSaveRequest request)
        {
            return Connection.SendRequestAsync<SquadSaveRequest, SquadSaveResponse, NullStruct>(this, (ushort)HutComponentCommand.squadSave, request);
        }

        public SquadListResponse GetSquadListRequest(ProvidedUID request)
        {
            return Connection.SendRequest<ProvidedUID, SquadListResponse, NullStruct>(this, (ushort)HutComponentCommand.getSquadList, request);
        }

        public Task<SquadListResponse> GetSquadListRequestAsync(ProvidedUID request)
        {
            return Connection.SendRequestAsync<ProvidedUID, SquadListResponse, NullStruct>(this, (ushort)HutComponentCommand.getSquadList, request);
        }

        public StickerBookSearchResponse StickerBookSearch(StickerBookSearchRequest request)
        {
            return Connection.SendRequest<StickerBookSearchRequest, StickerBookSearchResponse, NullStruct>(this, (ushort)HutComponentCommand.stickerBookSearch, request);
        }

        public Task<StickerBookSearchResponse> StickerBookSearchAsync(StickerBookSearchRequest request)
        {
            return Connection.SendRequestAsync<StickerBookSearchRequest, StickerBookSearchResponse, NullStruct>(this, (ushort)HutComponentCommand.stickerBookSearch, request);
        }


        public StickerBookStats2Response GetStickerBookStats2(StickerBookStats2Request request)
        {
            return Connection.SendRequest<StickerBookStats2Request, StickerBookStats2Response, NullStruct>(this, (ushort)HutComponentCommand.stickerBookStats2, request);
        }

        public Task<StickerBookStats2Response> GetStickerBookStats2Async(StickerBookStats2Request request)
        {
            return Connection.SendRequestAsync<StickerBookStats2Request, StickerBookStats2Response, NullStruct>(this, (ushort)HutComponentCommand.stickerBookStats2, request);
        }

        public SquadLoadActiveResponse SquadLoadActive(SquadLoadActiveRequest request)
        {
            return Connection.SendRequest<SquadLoadActiveRequest, SquadLoadActiveResponse, NullStruct>(this, (ushort)HutComponentCommand.squadLoadActive, request);
        }

        public Task<SquadLoadActiveResponse> SquadLoadActiveAsync(SquadLoadActiveRequest request)
        {
            return Connection.SendRequestAsync<SquadLoadActiveRequest, SquadLoadActiveResponse, NullStruct>(this, (ushort)HutComponentCommand.squadLoadActive, request);
        }

        public UserReliabilityInfoResponse GetUserReliabilityRequest(ProvidedUID request)
        {
            return Connection.SendRequest<ProvidedUID, UserReliabilityInfoResponse, NullStruct>(this, (ushort)HutComponentCommand.getUserReliabilityInfo, request);
        }

        public Task<UserReliabilityInfoResponse> GetUserReliabilityRequestAsync(ProvidedUID request)
        {
            return Connection.SendRequestAsync<ProvidedUID, UserReliabilityInfoResponse, NullStruct>(this, (ushort)HutComponentCommand.getUserReliabilityInfo, request);
        }

        public override Type GetCommandRequestType(HutComponentCommand componentCommand)
        {
            return CardHouseComponentBase.GetCommandRequestType(componentCommand);
        }

        public override Type GetCommandResponseType(HutComponentCommand componentCommand)
        {
            return CardHouseComponentBase.GetCommandResponseType(componentCommand);
        }

        public override Type GetCommandErrorResponseType(HutComponentCommand componentCommand)
        {
            return CardHouseComponentBase.GetCommandErrorResponseType(componentCommand);
        }

        public override Type GetNotificationType(HutComponentNotification notification)
        {
            return CardHouseComponentBase.GetNotificationType(notification);
        }
    }
}