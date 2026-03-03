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
        resetUser = 108,
        getDeckInfo = 301,
        moveCard = 304,
        playGame = 305,
        assignCards = 307,
        createPack = 401,
        viewCards = 402,
        discardCard = 403,
        applyCard = 405,
        changePlayers = 406,
        getStaffBonus = 408,
        applySalaryCap = 409,
        ISViewTrade = 703,
        squadSave = 708,
        getSquadList = 709,
        squadLoadActive = 711,
        stickerBookStats2 = 800,
        stickerBookSearch = 802,
        activateCard = 803,
        ISWatchList = 804,
        ISRemoveWatch = 806,
        matchRegisterStart = 1000,
        matchRegisterFinish = 1001,
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
            CardHouseComponentCommand.resetUser => typeof(ProvidedUID),
            CardHouseComponentCommand.getDeckInfo => typeof(DeckInfoRequest),
            CardHouseComponentCommand.moveCard => typeof(MoveCardRequest),
            CardHouseComponentCommand.playGame => typeof(PlayGameRequest),
            CardHouseComponentCommand.assignCards => typeof(AssignCardsRequest),
            CardHouseComponentCommand.createPack => typeof(CreatePackRequest),
            CardHouseComponentCommand.viewCards => typeof(ViewCardsRequest),
            CardHouseComponentCommand.discardCard => typeof(DiscardCardRequest),
            CardHouseComponentCommand.changePlayers => typeof(ChangePlayersRequest),
            CardHouseComponentCommand.applyCard => typeof(ApplyCardRequest),
            CardHouseComponentCommand.getStaffBonus => typeof(ProvidedUID),
            CardHouseComponentCommand.applySalaryCap => typeof(ApplySalaryCapRequest),
            CardHouseComponentCommand.squadSave => typeof(SquadSaveRequest),
            CardHouseComponentCommand.getSquadList => typeof(ProvidedUID),
            CardHouseComponentCommand.squadLoadActive => typeof(SquadLoadActiveRequest),
            CardHouseComponentCommand.stickerBookStats2 => typeof(StickerBookStats2Request),
            CardHouseComponentCommand.stickerBookSearch => typeof(StickerBookSearchRequest),
            CardHouseComponentCommand.activateCard => typeof(ActivateCardRequest),
            CardHouseComponentCommand.ISWatchList => typeof(ISWatchListRequest),
            CardHouseComponentCommand.ISViewTrade => typeof(ISViewTradeRequest),
            CardHouseComponentCommand.ISRemoveWatch => typeof(ISRemoveWatchRequest),
            CardHouseComponentCommand.matchRegisterStart => typeof(MatchRegisterStartRequest),
            CardHouseComponentCommand.matchRegisterFinish => typeof(MatchRegisterFinishRequest),
            CardHouseComponentCommand.getUserReliabilityInfo => typeof(ProvidedUID),
            _ => typeof(NullStruct)
        };
    }

    public static Type GetCommandResponseType(CardHouseComponentCommand componentCommand)
    {
        return componentCommand switch
        {
            CardHouseComponentCommand.login => typeof(LoginResponse),
            CardHouseComponentCommand.logout => typeof(NumericResponse),
            CardHouseComponentCommand.gamerSetInfo => typeof(NumericResponse),
            CardHouseComponentCommand.gamerGetInfo => typeof(GamerGetInfoResponse),
            CardHouseComponentCommand.getConfig => typeof(GetConfigResponse),
            CardHouseComponentCommand.getDeckInfo => typeof(DeckInfoResponse),
            CardHouseComponentCommand.moveCard => typeof(MoveCardResponse),
            CardHouseComponentCommand.playGame => typeof(PlayGameResponse),
            CardHouseComponentCommand.assignCards => typeof(AssignCardsResponse),
            CardHouseComponentCommand.createPack => typeof(CreatePackResponse),
            CardHouseComponentCommand.viewCards => typeof(ViewCardsResponse),
            CardHouseComponentCommand.discardCard => typeof(DiscardCardResponse),
            CardHouseComponentCommand.changePlayers => typeof(ChangePlayersResponse),
            CardHouseComponentCommand.getStaffBonus => typeof(StaffBonusResponse),
            CardHouseComponentCommand.applySalaryCap => typeof(ApplySalaryCapResponse),
            CardHouseComponentCommand.applyCard => typeof(ApplyCardResponse),
            CardHouseComponentCommand.squadSave => typeof(SquadSaveResponse),
            CardHouseComponentCommand.getSquadList => typeof(SquadListResponse),
            CardHouseComponentCommand.squadLoadActive => typeof(SquadLoadActiveResponse),
            CardHouseComponentCommand.stickerBookStats2 => typeof(StickerBookStats2Response),
            CardHouseComponentCommand.stickerBookSearch => typeof(StickerBookSearchResponse),
            CardHouseComponentCommand.activateCard => typeof(ActivateCardResponse),
            CardHouseComponentCommand.ISWatchList => typeof(ISWatchListResponse),
            CardHouseComponentCommand.ISViewTrade => typeof(ISViewTradeResponse),
            CardHouseComponentCommand.ISRemoveWatch => typeof(ISRemoveWatchResponse),
            CardHouseComponentCommand.matchRegisterStart => typeof(MatchRegisterStartResponse),
            CardHouseComponentCommand.matchRegisterFinish => typeof(NumericResponse),
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
        public virtual Task<NumericResponse> LogoutRequestAsync(LogoutRequest request, BlazeRpcContext context)
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
        
        [BlazeCommand((ushort)CardHouseComponentCommand.resetUser)]
        public virtual Task<NumericResponse> ResetUserRequestAsync(ProvidedUID request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.getConfig)]
        public virtual Task<GetConfigResponse> GetConfigRequestAsync(ProvidedUID request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.getDeckInfo)]
        public virtual Task<DeckInfoResponse> GetDeckInfoAsync(DeckInfoRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }
        
        [BlazeCommand((ushort)CardHouseComponentCommand.applyCard)]
        public virtual Task<ApplyCardResponse> ApplyCardAsync(ApplyCardRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }
        
        [BlazeCommand((ushort)CardHouseComponentCommand.applySalaryCap)]
        public virtual Task<ApplySalaryCapResponse> ApplySalaryCapAsync(ApplySalaryCapRequest request, BlazeRpcContext context)
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
        
        [BlazeCommand((ushort)CardHouseComponentCommand.assignCards)]
        public virtual Task<AssignCardsResponse> AssignCardsAsync(AssignCardsRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }
        
                
        [BlazeCommand((ushort)CardHouseComponentCommand.moveCard)]
        public virtual Task<MoveCardResponse> MoveCardAsync(MoveCardRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }


        [BlazeCommand((ushort)CardHouseComponentCommand.discardCard)]
        public virtual Task<DiscardCardResponse> DiscardCardAsync(DiscardCardRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }
        
        [BlazeCommand((ushort)CardHouseComponentCommand.changePlayers)]
        public virtual Task<ChangePlayersResponse> ChangePlayersAsync(ChangePlayersRequest request, BlazeRpcContext context)
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
        [BlazeCommand((ushort)CardHouseComponentCommand.activateCard)]
        public virtual Task<ActivateCardResponse> ActivateCardAsync(ActivateCardRequest request, BlazeRpcContext context)
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
        
        [BlazeCommand((ushort)CardHouseComponentCommand.matchRegisterStart)]
        public virtual Task<MatchRegisterStartResponse> MatchRegisterStartAsync(MatchRegisterStartRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }
        
        [BlazeCommand((ushort)CardHouseComponentCommand.matchRegisterFinish)]
        public virtual Task<NumericResponse> MatchRegisterFinishAsync(MatchRegisterFinishRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.playGame)]
        public virtual Task<PlayGameResponse> PlayGameAsync(PlayGameRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }

        [BlazeCommand((ushort)CardHouseComponentCommand.ISWatchList)]
        public virtual Task<ISWatchListResponse> ISWatchListAsync(ISWatchListRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }
        
                
        [BlazeCommand((ushort)CardHouseComponentCommand.ISViewTrade)]
        public virtual Task<ISViewTradeResponse> ISViewTradeAsync(ISViewTradeRequest request, BlazeRpcContext context)
        {
            throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
        }
        
        [BlazeCommand((ushort)CardHouseComponentCommand.ISRemoveWatch)]
        public virtual Task<ISRemoveWatchResponse> ISRemoveWatchAsync(ISRemoveWatchRequest request, BlazeRpcContext context)
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
        
        public NumericResponse LogoutRequest(LogoutRequest request)
        {
            return Connection.SendRequest<LogoutRequest, NumericResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.logout, request);
        }

        public Task<NumericResponse> LogoutRequestAsync(LogoutRequest request)
        {
            return Connection.SendRequestAsync<LogoutRequest, NumericResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.logout, request);
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
        
        public ApplySalaryCapResponse ApplySalaryCapRequest(ApplySalaryCapRequest request)
        {
            return Connection.SendRequest<ApplySalaryCapRequest, ApplySalaryCapResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.applySalaryCap, request);
        }

        public Task<ApplySalaryCapResponse> ApplySalaryCapRequestAsync(ApplySalaryCapRequest request)
        {
            return Connection.SendRequestAsync<ApplySalaryCapRequest, ApplySalaryCapResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.applySalaryCap, request);
        }

        public NumericResponse ResetUserRequest(ProvidedUID request)
        {
            return Connection.SendRequest<ProvidedUID, NumericResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.resetUser, request);
        }

        public Task<NumericResponse> ResetUserRequestAsync(ProvidedUID request)
        {
            return Connection.SendRequestAsync<ProvidedUID, NumericResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.resetUser, request);
        }
        
        public MoveCardResponse MoveCardRequest(MoveCardRequest request)
        {
            return Connection.SendRequest<MoveCardRequest, MoveCardResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.moveCard, request);
        }

        public Task<MoveCardResponse> MoveCardRequestAsync(MoveCardRequest request)
        {
            return Connection.SendRequestAsync<MoveCardRequest, MoveCardResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.moveCard, request);
        }
        
        public ApplyCardResponse ApplyCardRequest(ApplyCardRequest request)
        {
            return Connection.SendRequest<ApplyCardRequest, ApplyCardResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.applyCard, request);
        }

        public Task<ApplyCardResponse> ApplyCardRequestAsync(ApplyCardRequest request)
        {
            return Connection.SendRequestAsync<ApplyCardRequest, ApplyCardResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.applyCard, request);
        }
        
        public ActivateCardResponse ActivateCardRequest(ActivateCardRequest request)
        {
            return Connection.SendRequest<ActivateCardRequest, ActivateCardResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.activateCard, request);
        }

        public Task<ActivateCardResponse> ActivateCardRequestAsync(ActivateCardRequest request)
        {
            return Connection.SendRequestAsync<ActivateCardRequest, ActivateCardResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.activateCard, request);
        }
        
        public ChangePlayersResponse ChangePlayersRequest(ChangePlayersRequest request)
        {
            return Connection.SendRequest<ChangePlayersRequest, ChangePlayersResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.changePlayers, request);
        }

        public Task<ChangePlayersResponse> ChangePlayersRequestAsync(ChangePlayersRequest request)
        {
            return Connection.SendRequestAsync<ChangePlayersRequest, ChangePlayersResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.changePlayers, request);
        }
        
        public GetConfigResponse GetConfigRequest(ProvidedUID request)
        {
            return Connection.SendRequest<ProvidedUID, GetConfigResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.getConfig, request);
        }

        public Task<GetConfigResponse> GetConfigRequestAsync(ProvidedUID request)
        {
            return Connection.SendRequestAsync<ProvidedUID, GetConfigResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.getConfig, request);
        }
        
        public AssignCardsResponse AssignCardsRequest(AssignCardsRequest request)
        {
            return Connection.SendRequest<AssignCardsRequest, AssignCardsResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.assignCards, request);
        }

        public Task<AssignCardsResponse> AssignCardsRequestAsync(AssignCardsRequest request)
        {
            return Connection.SendRequestAsync<AssignCardsRequest, AssignCardsResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.assignCards, request);
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
        
        public MatchRegisterStartResponse MatchRegisterStart(MatchRegisterStartRequest request)
        {
            return Connection.SendRequest<MatchRegisterStartRequest, MatchRegisterStartResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.matchRegisterStart, request);
        }

        public Task<MatchRegisterStartResponse> MatchRegisterStartAsync(MatchRegisterStartRequest request)
        {
            return Connection.SendRequestAsync<MatchRegisterStartRequest, MatchRegisterStartResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.matchRegisterStart, request);
        }
        
        public NumericResponse MatchRegisterFinish(MatchRegisterFinishRequest request)
        {
            return Connection.SendRequest<MatchRegisterFinishRequest, NumericResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.matchRegisterFinish, request);
        }

        public Task<NumericResponse> MatchRegisterFinishAsync(MatchRegisterFinishRequest request)
        {
            return Connection.SendRequestAsync<MatchRegisterFinishRequest, NumericResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.matchRegisterFinish, request);
        }
        
        public PlayGameResponse PlayGame(PlayGameRequest request)
        {
            return Connection.SendRequest<PlayGameRequest, PlayGameResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.playGame, request);
        }

        public Task<PlayGameResponse> PlayGameAsync(PlayGameRequest request)
        {
            return Connection.SendRequestAsync<PlayGameRequest, PlayGameResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.playGame, request);
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
        
        public ISWatchListResponse ISWatchListRequest(ISWatchListRequest request)
        {
            return Connection.SendRequest<ISWatchListRequest, ISWatchListResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.ISWatchList, request);
        }

        public Task<ISWatchListResponse> ISWatchListRequestAsync(ISWatchListRequest request)
        {
            return Connection.SendRequestAsync<ISWatchListRequest, ISWatchListResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.ISWatchList, request);
        }
        
        public ISViewTradeResponse ISViewTradeRequest(ISViewTradeRequest request)
        {
            return Connection.SendRequest<ISViewTradeRequest, ISViewTradeResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.ISViewTrade, request);
        }

        public Task<ISViewTradeResponse> ISViewTradeRequestAsync(ISViewTradeRequest request)
        {
            return Connection.SendRequestAsync<ISViewTradeRequest, ISViewTradeResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.ISViewTrade, request);
        }
            
        public ISRemoveWatchResponse ISRemoveWatchRequest(ISRemoveWatchRequest request)
        {
            return Connection.SendRequest<ISRemoveWatchRequest, ISRemoveWatchResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.ISRemoveWatch, request);
        }

        public Task<ISRemoveWatchResponse> ISRemoveWatchAsync(ISRemoveWatchRequest request)
        {
            return Connection.SendRequestAsync<ISRemoveWatchRequest, ISRemoveWatchResponse, NullStruct>(this, (ushort)CardHouseComponentCommand.ISRemoveWatch, request);
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