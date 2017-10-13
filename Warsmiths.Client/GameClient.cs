using System;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using ExitGames.Logging;
using Newtonsoft.Json;
using Warsmiths.Client.Loadbalancing;
using Warsmiths.Client.Loadbalancing.Codes;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Craft;
using Warsmiths.Common.Domain.Craft.Quest;
using Warsmiths.Common.Domain.Craft.SharedClasses;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.Common.Domain.Tasks;
using Warsmiths.Common.Domain.VictoryPrizes;
using Warsmiths.Common.ListContainer;
using Warsmiths.Common.Results;
using Player = Warsmiths.Client.Loadbalancing.Player;

// ReSharper disable InconsistentNaming

namespace Warsmiths.Client
{
    using GameEvent = Common.EventCode;
    using GameParameters = Common.ParameterCode;
    using GameOpCode = Common.OperationCode;
    using GameErrorCode = Common.ErrorCode;

    public delegate void ServerCharacterProfileDelegate(Common.Domain.Player profile);

    public delegate void ServerCharacterCharacteristicsDelegate(Common.Domain.Characteristics.CharacteristicsBase characteristics);

    public delegate void ServerNotificationDelegate(byte[] data);

    public delegate void ServerUpdateCharacterListDelegate(CharactersListContainer charsList);

    public delegate void ServerUpdateInventoryDelegate(PlayerInventory inventory);

    public delegate void ServerUpdateEquipmentDelegate(CharacterEquipment eq);

    public delegate void ServerUpdateAuctionDelegate(LotsListContainer lotsLost);

    public delegate void ServerUpdateElementsOrderDelegate(ElementOrderListContainer orders);

    public delegate void ErrorAction(GameErrorCode errorCode);

    public delegate void ServerUpdateElementPricesDelegate(ElementsPricesContainer data);

    public delegate void ServerUpdateDomainConfigurationDelegate(DomainConfiguration data);

    public class GameClient : LoadBalancingClient
    {
        public const byte MaxPlayers = 2;
        public Action OnConnectToServer = () => { };
        public Action OnDisconnect = () => { };

        public Action OnRegistretedSuccess = () => { };
        public Action OnLogged = () => { };
        public Action OnLogOut = () => { };

        public Action OnCharacterRemoved = () => { };
        public Action OnCharacterCreated = () => { };
        public Action<string> OnCharacterSelected = (characterName) => { };

        public Action OnProfileSent = () => { };
        public Action OnCharacterProfile = () => { };
        public Action OnAccountCreated = () => { };
        public Action<string> OnEquipmentWearSuccess = (equipmentId) => { };
        public Action<string> OnEquipmentUnWearSuccess = (equipmentId) => { };
        public Action OnLotsListSent = () => { };
        public Action OnBuyLot = () => { };
        public Action OnLotPublished = () => { };
        public Action OnLotUnpublished = () => { };
        public Action OnInventoryDataSent = () => { };
        public Action OnAddedToInventory = () => { };
        public Action OnRemovedFromInventory = () => { };
        public Action OnProfileSaved = () => { };
        public Action<DestroyEquipmentResult> OnDestroyEquipmentResult = result => {};
        public Action<string, string> OnArmorCreated = (equpmentId, recieptId) => { };
        public Action OnEnchantSuccess = () => { };
        public Action OnSellElementResult = () => { };
        public Action OnOrderElementResult = () => { };
        public Action OnElementsOrderResult = () => { };
        public Action OnReceiveElementResult = () => { };
        public Action<int, int, int, int> OnUpdateCurrency = (gold, crystal, keys, helbox) => { };
        public Action OnClassAddedResult = () => { };
        public Action OnClassRemovedResult = () => { };
        public Action OnAddAbilityResult = () => { };
        public Action OnRemoveAbilityResult = () => { };
        public Action OnSetCraftExperienceResult = () => { };
        public Action OnSetExperienceResult = () => { };
        public Action OnReciveSavedReciepts = () => { };
        public Action OnReciveRecieptsInfo = () => { };
        public Action OnReciveRecieptStage = () => { };
        public Action<VictoryPrizesResult> OnVictoryPrizesEvent = result => { };

        public Action<LotsListContainer> OnUpdateAuctionEvent = lots => { };
        public Action<DebugLevel, string> OnDebugReturn = (level, message) => { };
        public Action<BaseReciept> OnGetRecieptResult = (reciept) => { };
        public Action<BaseReciept> OnGetNextQuest = (reciept) => { };
        public Action<BaseReciept[]> OnGetRecieptsResult = (reciept) => { };
        public Action<CraftResoult> OnEndCraft = (resoult) => { };
        public Action<Task> OnTaskCompleted = (resoult) => { };
        public Action<Task> OnTaslStarted = (resoult) => { };
        public Action<BaseReciept> OnStartCraft = (reciept) => { };
        public Action OnQuestReceptCompeled = () => { };
        public Action<BaseQuestStage> OnGetQuestStage = (queststage) => { };
        public Action<BaseReciept> OnRecieptCreated = (reciept) => { };


        public ErrorAction OnLoggedFailed = code => { };
        public ErrorAction OnRegistretedFailed = code => { };
        public ErrorAction OnCharacterRemovedFailed = code => { };
        public ErrorAction OnCharacterCreatedFailed = code => { };
        public ErrorAction OnProfileSentFailed = code => { };
        public ErrorAction OnAccountCreatedFailed = code => { };
        public ErrorAction OnEquipmentWearFailed = code => { };
        public ErrorAction OnLotsListSentFailed = code => { };
        public ErrorAction OnBuyLotFailed = code => { };
        public ErrorAction OnLotPublishedFailed = code => { };
        public ErrorAction OnLotUnpublishedFailed = code => { };
        public ErrorAction OnInventoryDataSentFailed = code => { };
        public ErrorAction OnAddedToInventoryFailed = code => { };
        public ErrorAction OnRemovedFromInventoryFailed = code => { };
        public ErrorAction OnProfileSavedFailed = code => { };
        public ErrorAction OnEquipmentUnWearFailed = code => { };
        public ErrorAction OnDestroyEquipmentResultFailed = code => { };
        public ErrorAction OnArmorCreatedFailed = code => { };
        public ErrorAction OnEnchantFailed = code => { };
        public ErrorAction OnSellElementResultFailed = code => { };
        public ErrorAction OnOrderElementResultFailed = code => { };
        public ErrorAction OnElementsOrderResultFailed = code => { };
        public ErrorAction OnReceiveElementResultFailed = code => { };
        public ErrorAction OnCharacterSelectedFailed = code => { };
        public ErrorAction OnLogOutFailed = code => { };
        public ErrorAction OnGetRecieptFailed = code => { };
        public ErrorAction OnRecieptCreatedFailed = code => { };
        public ErrorAction OnClassAddFailResult = code => { };
        public ErrorAction OnClassRemoveFailResult = code => { };
        public ErrorAction OnAddAbilityFailResult = code => { };
        public ErrorAction OnRemoveAbilityFailResult = code => { };
        public ErrorAction OnSetCraftExperienceFailResult = code => { };
        public ErrorAction OnSetExperienceFailResult = code => { };

        public ServerCharacterProfileDelegate OnPlayerProfileEvent = profile => { };
        public ServerCharacterCharacteristicsDelegate OnCharacerCharacteristicsChangeEvent = characteristics => { };

        public ServerNotificationDelegate OnNotificationEvent = data => { };
        public ServerUpdateCharacterListDelegate OnUpdateCharacterListEvent = list => { };
        public ServerUpdateInventoryDelegate OnUpdateInventoryEvent = inventory => { };
        public ServerUpdateEquipmentDelegate OnUpdateEquipmentEvent = eq => { };
        //public ServerUpdateAuctionDelegate OnUpdateAuctionEvent = lots => { };
        public ServerUpdateElementsOrderDelegate OnUpdateElementsOrderEvent = orders => { };
        public ServerUpdateElementPricesDelegate OnUpdateElementPricesEvent = data => { };
        public ServerUpdateDomainConfigurationDelegate OnUpdateDomainConfigurationEvent = data => { };
        public ServerCharacterProfileDelegate ServerCharacterCharacteristicsDelegate = characteristics => { };

        #region Service

        public override void DebugReturn(DebugLevel level, string message)
        {
            OnDebugReturn(level, message);
            base.DebugReturn(level, message);
        }

        public override void OnStatusChanged(StatusCode statusCode)
        {
            base.OnStatusChanged(statusCode);


            switch (statusCode)
            {
                case StatusCode.Connect:
                    OnConnectToServer();
                    break;
                case StatusCode.TimeoutDisconnect:
                    break;
                case StatusCode.DisconnectByServer:
                    break;
                case StatusCode.Exception:
                case StatusCode.ExceptionOnConnect:
                    break;
                case StatusCode.Disconnect:
                    OnDisconnect();
                    break;
                case StatusCode.SecurityExceptionOnConnect:
                    break;
                case StatusCode.QueueOutgoingReliableWarning:
                    break;
                case StatusCode.QueueOutgoingUnreliableWarning:
                    break;
                case StatusCode.SendError:
                    break;
                case StatusCode.QueueOutgoingAcksWarning:
                    break;
                case StatusCode.QueueIncomingReliableWarning:
                    break;
                case StatusCode.QueueIncomingUnreliableWarning:
                    break;
                case StatusCode.QueueSentWarning:
                    break;
                case StatusCode.ExceptionOnReceive:
                    break;
                case StatusCode.DisconnectByServerUserLimit:
                    break;
                case StatusCode.DisconnectByServerLogic:
                    break;
                case StatusCode.EncryptionEstablished:
                    break;
                case StatusCode.EncryptionFailedToEstablish:
                    break;
                /*default:
                    throw new ArgumentOutOfRangeException("statusCode", statusCode, null);*/
            }
        }

        public void SetupMasterServerAddress(string address)
        {
            MasterServerAddress = address;
        }

        protected internal override Player CreatePlayer(string actorName, int actorNumber, bool isLocal,
            Hashtable actorProperties)
        {
            var player = new Player(actorName, actorNumber, isLocal, actorProperties);
            return player;
        }

        #endregion

        #region CallBacks

        public override void OnEvent(EventData photonEvent)
        {
            base.OnEvent(photonEvent);
            
            var gameEvent = (GameEvent) photonEvent.Code;

            switch (gameEvent)
            {
                case GameEvent.PlayerProfile:
                    var bsonProfileData = (byte[]) photonEvent.Parameters[(byte) GameParameters.ProfileData];
                    var profile = bsonProfileData.FromBson<Common.Domain.Player>();
                    OnPlayerProfileEvent(profile);
                    break;

                case GameEvent.UpdateCurrency:
                        var gold = (int)photonEvent.Parameters[(byte)GameParameters.CurrencyGold];
                        var crystal = (int)photonEvent.Parameters[(byte)GameParameters.CurrencyCrystals];
                        var keys = (int)photonEvent.Parameters[(byte)GameParameters.CurrencyKeys];
                        var healbox = (int)photonEvent.Parameters[(byte)GameParameters.CurrenctyHealBox];
                        OnUpdateCurrency(gold, crystal, keys, healbox);
                    break;
                case GameEvent.Notification:
                    var nData = (byte[]) photonEvent.Parameters[(byte) GameParameters.NotificationData];
                    OnNotificationEvent(nData);
                    break;

               case GameEvent.Craft:
                        var craftData = (byte[])photonEvent.Parameters[(byte)GameParameters.Craft];
                        OnNotificationEvent(craftData);
                        break;

                case GameEvent.UpdateCharacterList:
                    var bsonCharList = (byte[]) photonEvent.Parameters[(byte) GameParameters.CharactersListData];
                    OnUpdateCharacterListEvent(bsonCharList.FromBson<CharactersListContainer>());
                    break;

                case GameEvent.UpdateInventory:
                    var bsonInv = (byte[]) photonEvent.Parameters[(byte) GameParameters.InventoryData];
                    OnUpdateInventoryEvent(bsonInv.FromBson<PlayerInventory>());
                    break;

                case GameEvent.UpdateEquipment:
                    var bsonEq = (byte[]) photonEvent.Parameters[(byte) GameParameters.EquipmentData];

                    var result = bsonEq.FromBson<EquipmentListContainer>(); 
                    OnUpdateEquipmentEvent(result.Equipments);
                    break;

                case GameEvent.UpdateAuction:
                    var bsonAuc = (byte[]) photonEvent.Parameters[(byte) GameParameters.AuctionData];
                    OnUpdateAuctionEvent(bsonAuc.FromBson<LotsListContainer>());
                    break;

                case GameEvent.UpdateElementsOrder:
                    var bsonElOrder = (byte[]) photonEvent.Parameters[(byte) GameParameters.ElementsOrderData];
                    OnUpdateElementsOrderEvent(bsonElOrder.FromBson<ElementOrderListContainer>());
                    break;

                case GameEvent.UpdateElementPrices:
                    var bsonPrices = (byte[]) photonEvent.Parameters[(byte) GameParameters.ElementsPrices];
                    OnUpdateElementPricesEvent(bsonPrices.FromBson<ElementsPricesContainer>());
                    break;

                case GameEvent.UpdateDomainConfiguration:
                    var configData = (byte[]) photonEvent.Parameters[(byte) GameParameters.DomainConfiguration];
                    var domainConfiguration = configData.FromBson<DomainConfiguration>();
                    OnUpdateDomainConfigurationEvent(domainConfiguration);
                    break;

                case GameEvent.VictoryPrizes:
                    var prizesData = (byte[])photonEvent.Parameters[(byte)GameParameters.VictoryPrizes];
                    var prizes = prizesData.FromBson<VictoryPrizesResult>();
                    OnVictoryPrizesEvent(prizes);
                    break;

                case GameEvent.GameList:
                    break;

                case GameEvent.GameListUpdate:
                    break;

                case GameEvent.QueueState:
                    break;

                case GameEvent.AppStats:
                    break;

                case GameEvent.GameServerOffline:
                    break;

                case GameEvent.UpdateCommonCharacterProfile:
                    var commonCharacterProfile = (byte[])photonEvent.Parameters[(byte)GameParameters.CommonCharacterProfile];
                    var characteristic = commonCharacterProfile.FromBson<Common.Domain.Characteristics.CharacteristicsBase>();
                    OnCharacerCharacteristicsChangeEvent(characteristic);
                    break;
            }
        }

        public override void OnOperationResponse(OperationResponse operationResponse)
        {
            base.OnOperationResponse(operationResponse);

            DebugReturn(DebugLevel.INFO, operationResponse.ToStringFull());

            switch (operationResponse.OperationCode)
            {
                case OperationCode.Authenticate:
                    if (operationResponse.ReturnCode == ErrorCode.InvalidAuthentication)
                    {
                        DebugReturn(DebugLevel.ERROR, "InvalidAuthentication");
                    }

                    if (operationResponse.ReturnCode == ErrorCode.InvalidOperation ||
                        operationResponse.ReturnCode == ErrorCode.InternalServerError)
                    {
                        DebugReturn(DebugLevel.ERROR,
                            $"Authentication failed. You successfully connected but the server ({MasterServerAddress}) but it doesn't know the 'authenticate'.  Check if it runs the Loadblancing server-logic.\nResponse: {operationResponse.ToStringFull()}");
                    }
                    break;

                case OperationCode.CreateGame:
                    var gsAddress = (string) operationResponse[ParameterCode.Address];
                    if (!string.IsNullOrEmpty(gsAddress) && gsAddress.StartsWith("127.0.0.1"))
                    {
                        DebugReturn(DebugLevel.ERROR, string.Empty);
                    }
                    break;

                case OperationCode.JoinRandomGame:
                    var gsAddressJoin = (string) operationResponse[ParameterCode.Address];
                    if (!string.IsNullOrEmpty(gsAddressJoin) && gsAddressJoin.StartsWith("127.0.0.1"))
                    {
                        DebugReturn(DebugLevel.ERROR, string.Empty);
                    }

                    if (operationResponse.ReturnCode != 0)
                    {
                        var roomname = "room_" + RoomsCount;
                        OpJoinOrCreateRoom(roomname, 0, new RoomOptions {MaxPlayers = MaxPlayers}, null);
                    }
                    break;

                case OperationCode.JoinGame:
                    if (operationResponse.ReturnCode != 0)
                    {
                        var roomname = "room_" + RoomsCount + 1;

                        OpJoinOrCreateRoom(roomname, 0, new RoomOptions {MaxPlayers = MaxPlayers}, null);
                    }
                    break;

                case OperationCode.Leave:
                    break;

                case OperationCode.RaiseEvent:
                    break;


                case OperationCode.CustomOp:
                    HandleCustomOperationsOnOperationResponse(operationResponse);
                    break;
            }
        }

        private void HandleCustomOperationsOnOperationResponse(OperationResponse operationResponse)
        {
            var code = (GameErrorCode) operationResponse.ReturnCode;
            var controlCode = GameOpCode.CustomOp;
            if (operationResponse.Parameters.ContainsKey((byte) GameParameters.ControlCode))
            {
                controlCode = (GameOpCode) operationResponse.Parameters[(byte) GameParameters.ControlCode];
            }

            switch (controlCode)
            {
                case GameOpCode.Registration:
                    if (code == GameErrorCode.Ok)
                    {
                        OnRegistretedSuccess();
                    }
                    else
                    {
                        OnRegistretedFailed(code);
                    }
                    break;

                case GameOpCode.Login:
                    if (code == GameErrorCode.Ok)
                    {
                        OnLogged();
                    }
                    else
                    {
                        OnLoggedFailed(code);
                    }
                    break;

                case GameOpCode.GetProfile:
                    if (code == GameErrorCode.Ok)
                    {
                        OnProfileSent();
                    }
                    else
                    {
                        OnProfileSentFailed(code);
                    }
                    break;

                case GameOpCode.CreateCharacter:
                    if (code == GameErrorCode.Ok)
                    {
                        OnCharacterCreated();
                    }
                    else
                    {
                        OnCharacterCreatedFailed(code);
                    }
                    break;
                case GameOpCode.RemoveCharacter:
                    if (code == GameErrorCode.Ok)
                    {
                        OnCharacterRemoved();
                    }
                    else
                    {
                        OnCharacterRemovedFailed(code);
                    }
                    break;

                case GameOpCode.TryWearEquipment:
                    if (code == GameErrorCode.Ok)
                    {
                        var equipmentId = (string) operationResponse.Parameters[(byte) GameParameters.EquipmentId];
                        OnEquipmentWearSuccess(equipmentId);
                    }
                    else
                    {
                        OnEquipmentWearFailed(code);
                    }
                    break;

                case GameOpCode.TryUnwearEquipment:
                    if (code == GameErrorCode.Ok)
                    {
                        var equipmentId = (string) operationResponse.Parameters[(byte) GameParameters.EquipmentId];
                        OnEquipmentUnWearSuccess(equipmentId);
                    }
                    else
                    {
                        OnEquipmentUnWearFailed(code);
                    }
                    break;

                case GameOpCode.GetLots:
                    if (code == GameErrorCode.Ok)
                    {
                        OnLotsListSent();
                    }
                    else
                    {
                        OnLotsListSentFailed(code);
                    }
                    break;

                case GameOpCode.TryBuyLot:
                    if (code == GameErrorCode.Ok)
                    {
                        OnBuyLot();
                    }
                    else
                    {
                        OnBuyLotFailed(code);
                    }
                    break;

                case GameOpCode.PublishLot:
                    if (code == GameErrorCode.Ok)
                    {
                        OnLotPublished();
                    }
                    else
                    {
                        OnLotPublishedFailed(code);
                    }
                    break;

                case GameOpCode.UnpublishLot:
                    if (code == GameErrorCode.Ok)
                    {
                        OnLotUnpublished();
                    }
                    else
                    {
                        OnLotUnpublishedFailed(code);
                    }

                    break;

                case GameOpCode.SelectCharacter:
                    if (code == GameErrorCode.Ok)
                    {
                        OnCharacterSelected((string) operationResponse.Parameters[(byte) GameParameters.Name]);
                    }
                    else
                    {
                        OnCharacterSelectedFailed(code);
                    }
                    break;

                case GameOpCode.GetInventory:
                    if (code == GameErrorCode.Ok)
                    {
                        OnInventoryDataSent();
                    }
                    else
                    {
                        OnInventoryDataSentFailed(code);
                    }
                    break;

                case GameOpCode.AddToInventory:
                    if (code == GameErrorCode.Ok)
                    {
                        OnAddedToInventory();
                    }
                    else
                    {
                        OnAddedToInventoryFailed(code);
                    }
                    break;

                case GameOpCode.DeleteFromInventory:
                    if (code == GameErrorCode.Ok)
                    {
                        OnRemovedFromInventory();
                    }
                    else
                    {
                        OnRemovedFromInventoryFailed(code);
                    }
                    break;

                case GameOpCode.DestroyEquipment:
                    if (code == GameErrorCode.Ok)
                    {
                        var dataBytes = (byte[]) operationResponse.Parameters[(byte) GameParameters.DestroyedEquipmentDataResult];
                        OnDestroyEquipmentResult(dataBytes.FromBson<DestroyEquipmentResult>());
                    }
                    else
                    {
                        OnDestroyEquipmentResultFailed(code);
                    }
                    break;

                case GameOpCode.CreateArmor:
                    if (code == GameErrorCode.Ok)
                    {
                        var recieptId = (string)operationResponse.Parameters[(byte)GameParameters.RecieptId];
                        var equipmentId = (string)operationResponse.Parameters[(byte)GameParameters.EquipmentId];
                        OnArmorCreated(equipmentId, recieptId);
                    }
                    else
                    {
                        OnArmorCreatedFailed(code);
                    }
                    break;

                case GameOpCode.EnchantEquipment:
                    if (code == GameErrorCode.Ok)
                    {
                        OnEnchantSuccess();
                    }
                    else
                    {
                        OnEnchantFailed(code);
                    }
                    break;

                case GameOpCode.SellElement:
                    if (code == GameErrorCode.Ok)
                    {
                        OnSellElementResult();
                    }
                    else
                    {
                        OnSellElementResultFailed(code);
                    }
                    break;

                case GameOpCode.OrderElement:
                    if (code == GameErrorCode.Ok)
                    {
                        OnOrderElementResult();
                    }
                    else
                    {
                        OnOrderElementResultFailed(code);
                    }
                    break;

                case GameOpCode.GetElementsOrders:
                    if (code == GameErrorCode.Ok)
                    {
                        OnElementsOrderResult();
                    }
                    else
                    {
                        OnElementsOrderResultFailed(code);
                    }
                    break;


                case GameOpCode.ReceiveElement:
                    if (code == GameErrorCode.Ok)
                    {
                        OnReceiveElementResult();
                    }
                    else
                    {
                        OnReceiveElementResultFailed(code);
                    }
                    break;

                case GameOpCode.SaveReservedFieldsForCharacter:
                    break;

                case GameOpCode.Logout:
                    if (code == GameErrorCode.Ok)
                    {
                        OnLogOut();
                    }
                    else
                    {
                        OnLogOutFailed(code);
                    }
                    break;

                case GameOpCode.Join:
                    break;

                case GameOpCode.Leave:
                    break;

                case GameOpCode.RaiseEvent:
                    break;

                case GameOpCode.SetProperties:
                    break;

                case GameOpCode.GetProperties:
                    break;

                case GameOpCode.Ping:
                    break;

                case GameOpCode.ChangeGroups:
                    break;

                case GameOpCode.Authenticate:
                    break;

                case GameOpCode.JoinLobby:
                    break;

                case GameOpCode.LeaveLobby:
                    break;

                case GameOpCode.CreateGame:
                    break;

                case GameOpCode.JoinGame:
                    break;

                case GameOpCode.JoinRandomGame:
                    break;

                case GameOpCode.DebugGame:
                    break;

                case GameOpCode.FiendFriends:
                    break;

                case GameOpCode.GetElementPrices:
                    break;

                case GameOpCode.CustomOp:
                    break;
                case GameOpCode.TakeDestroyResult:
                    break;
                case GameOpCode.CraftQuestComplete:
                    if (code == GameErrorCode.Ok)
                    {
                        OnQuestReceptCompeled();
                    }
                    else
                    {
                        OnGetRecieptFailed(code);
                    }
                    break;
                case GameOpCode.GetReciepts:

                    if (code == GameErrorCode.Ok)
                    {
                        var dataBytes = (byte[])operationResponse.Parameters[(byte)GameParameters.RecieptsData];
                        var reciepts = dataBytes.FromBson<RecieptsContainer>();
                        OnGetRecieptsResult(reciepts.Reciepts);
                    }
                    else
                    {
                        OnGetRecieptFailed(code);
                    }
                    break;
                case GameOpCode.StartReciept:

                    if (code == GameErrorCode.Ok)
                    {
                        try
                        {
                            var dataBytes = (byte[]) operationResponse.Parameters[(byte) GameParameters.StartReciept];
                            var rec = dataBytes.FromBson<BaseReciept>();
                            OnStartCraft(rec);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message + "\n"+ operationResponse.Parameters.ToJson());
                        }
                    }
                    else
                    {
                        OnGetRecieptFailed(code);
                    }
                    break;
                case GameOpCode.CompleteTask:

                    if (code == GameErrorCode.Ok)
                    {
                        var dataBytes = (byte[])operationResponse.Parameters[(byte)GameParameters.TaskComplete];
                        var resoult = dataBytes.FromBson<Task>();
                        OnTaskCompleted(resoult);
                    }
                    else
                    {
                        OnGetRecieptFailed(code);
                    }
                    break;
                case GameOpCode.EndReciept:

                    if (code == GameErrorCode.Ok)
                    {
                        var dataBytes = (byte[])operationResponse.Parameters[(byte)GameParameters.EndReciept];
                        var resoult = dataBytes.FromBson<CraftResoult>();
                        OnEndCraft(resoult);
                    }
                    else
                    {
                        OnGetRecieptFailed(code);
                    }
                    break;
                case GameOpCode.CraftQuestNext:
                    if (code == GameErrorCode.Ok)
                    {
                        var dataBytes = (byte[])operationResponse.Parameters[(byte)GameParameters.NextCraftQuest];
                        var reciept = dataBytes.FromBson<BaseReciept>();
                        OnGetNextQuest(reciept);
                    }
                    else
                    {
                        OnGetRecieptFailed(code);
                    }
                    break;
                case GameOpCode.GetReciept:
                    if (code == GameErrorCode.Ok)
                    {
                        var dataBytes = (byte[])operationResponse.Parameters[(byte)GameParameters.RecieptData];
                        var reciept = dataBytes.FromBson<BaseReciept>();
                        OnGetRecieptResult(reciept);
                    }
                    else
                    {
                        OnGetRecieptFailed(code);
                    }
                    break;
                case GameOpCode.SaveReciept:
                    if (code == GameErrorCode.Ok)
                    {
                        var dataBytes = (byte[])operationResponse.Parameters[(byte)GameParameters.RecieptData];
                        var reciept = dataBytes.FromBson<BaseReciept>();
                        OnRecieptCreated(reciept);
                    }
                    else
                    {
                        OnRecieptCreatedFailed(code);
                    }
                    break;

                case GameOpCode.AddClass:
                    if (code == GameErrorCode.Ok)
                    {
                        OnClassAddedResult();
                    }
                    else
                    {
                        OnClassAddFailResult(code);
                    }
                    break;

                case GameOpCode.RemoveClass:
                    if (code == GameErrorCode.Ok)
                    {
                        OnClassRemovedResult();
                    }
                    else
                    {
                        OnClassRemoveFailResult(code);
                    }
                    break;

                case GameOpCode.AddAbility:
                    if (code == GameErrorCode.Ok)
                    {
                        OnAddAbilityResult();
                    }
                    else
                    {
                        OnAddAbilityFailResult(code);
                    }
                    break;

                case GameOpCode.RemoveAbility:
                    if (code == GameErrorCode.Ok)
                    {
                        OnRemoveAbilityResult();
                    }
                    else
                    {
                        OnRemoveAbilityFailResult(code);
                    }
                    break;

                case GameOpCode.SetCraftExperience:
                    if (code == GameErrorCode.Ok)
                    {
                        OnSetCraftExperienceResult();
                    }
                    else
                    {
                        OnSetCraftExperienceFailResult(code);
                    }
                    break;

                case GameOpCode.SetExperience:
                    if (code == GameErrorCode.Ok)
                    {
                        OnSetExperienceResult();
                    }
                    else
                    {
                        OnSetExperienceFailResult(code);
                    }
                    break;
            }
        }

        #endregion

        #region Login/Registration/Character Managment

        public void SendLogin(string login, string password)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {(byte) GameParameters.ControlCode, GameOpCode.Login},
                {(byte) GameParameters.LoginReg, login},
                {(byte) GameParameters.Password, password},
            }, true);
        }

        public void SendLogout()
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {(byte) GameParameters.ControlCode, GameOpCode.Logout}
            }, true);
        }

        public void SendRegistration(string userFirstName, string userLastName, string login, string password,
            string email)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.Registration
                },
                {(byte) GameParameters.LoginReg, login},
                {(byte) GameParameters.Password, password},
                {(byte) GameParameters.UserFirstName, userFirstName},
                {(byte) GameParameters.UserLastName, userLastName},
                {(byte) GameParameters.Email, email},
            }, true);
        }

        public void SendCreateCharacter(string characterName, RaceTypes race, HeroTypes hero, ClassTypes characterClass,
            StartBonusTypes bonus)
        {
            var d = new Dictionary<byte, object>
            {
                {(byte) GameParameters.ControlCode, GameOpCode.CreateCharacter},
                {(byte) GameParameters.Name, characterName},
                {(byte) GameParameters.Race, (int) race},
                {(byte) GameParameters.Hero, (int) hero},
                {(byte) GameParameters.StartBonus, (int) bonus},
                {(byte) GameParameters.CharacterClass, (int) characterClass}
            };
            peer.OpCustom((byte) GameOpCode.CustomOp, d, true);
        }

        public void SendRemoveCharacter(string characterName)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.RemoveCharacter
                },
                {(byte) GameParameters.Name, characterName},
            }, true);
        }

        public void SendSelectCharacter(string characterName)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.SelectCharacter
                },
                {(byte) GameParameters.Name, characterName},
            }, true);
        }

        #endregion

        #region Inventory

        public void SendGetInventory()
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.GetInventory
                }
            }, true);
        }

        public void SendAddToInventory(IEntity item, int amount)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.AddToInventory
                },
                {
                    (byte) GameParameters.Entity, item._id
                },
                {
                    (byte) GameParameters.Quantity, amount
                }
            }, true);
        }

        public void SendRemoveFromInventory(IEntity item)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.AddToInventory
                },
                {
                    (byte) GameParameters.Entity, item._id
                }
            }, true);
        }

        #endregion

        #region Auction

        public void SendPublishLot(BaseEquipment item, float price)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.PublishLot
                },
                {(byte) GameParameters.EquipmentId, item._id},
                {(byte) GameParameters.Money, price},
            }, true);
        }

        public void SendUnpublishLot(Lot item)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.UnpublishLot
                },
                {(byte) GameParameters.LotId, item._id},
            }, true);
        }

        public void SendBuyLot(Lot lot)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.TryBuyLot
                },
                {(byte) GameParameters.EquipmentId, lot._id},
            }, true);
        }

        public void SendGetAuction()
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.GetLots
                },
            }, true);
        }

        #endregion

        #region Profile 

        public void SendGetProfile()
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.GetProfile
                },
            }, true);
        }

        #endregion

        #region Craft

        public void SendCreateArmor(BaseReciept reciept)
        {
            if (reciept.PartsPosition == null)
            {
                throw new Exception("ArmorParts is null!");
            }

            var data = reciept.ToBson();
            
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.CreateArmor
                },
                {
                    (byte) GameParameters.RecieptData, data
                }
            }, true);
        }


        public void SendSaveReciept(BaseReciept reciept)
        {
            if (reciept == null)
            {
                throw new Exception("reciept is null! failed");
            }

            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.SaveReciept
                },
                {
                    (byte) GameParameters.RecieptJson, reciept.ToJson()
                }
            }, true);
        }
        public void SendGetTaskComplete(string taskName, TaskTypesE taskType)
        {
            
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.CompleteTask
                },
                {
                    (byte) GameParameters.TaskName, taskName
                },
                {
                    (byte) GameParameters.TaskType, taskType
                }
            }, true);
        }
        public void SendGetTaskStarted(string taskName, TaskTypesE taskType)
        {

            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.CompleteTask
                },
                {
                    (byte) GameParameters.TaskComplete, taskName
                },
                {
                    (byte) GameParameters.TaskType, taskType
                }
            }, true);
        }
        public void SendChangeCurrency(int value, CurrencyTypeE type)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.ChangeCurrency
                },
                {
                    (byte) GameParameters.CurrencyValue, value
                },
                {
                    (byte) GameParameters.CurrencyType, (int)type
                }
            }, true);
        }
        public void SendGetReceptStage(string recieptId, int stage)
        {
            if (string.IsNullOrEmpty(recieptId))
            {
                throw new Exception("reciept is null! failed");
            }

            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.GetRecieptStage
                },
                {
                    (byte) GameParameters.RecieptId, recieptId
                },
                {
                    (byte) GameParameters.RecieptStage, stage
                }
            }, true);
        }

        public void SendQuestReceptCompleted (string receptName)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.CraftQuestComplete
                },
                {
                    (byte) GameParameters.CraftQuestComplete, receptName
                }
            }, true);
        }
        public void SendNextQuestReciepts(string currentQuest)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.CraftQuestNext
                },
                {
                    (byte) GameParameters.NextCraftQuest, currentQuest
                }
            }, true);
        }
        public void SendStartReciept(BaseReciept reciept)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.StartReciept
                },
                {
                    (byte) GameParameters.StartReciept, reciept.ToBson()
                }
            }, true);
        }
        public void SendEndReciept(BaseReciept reciept, int recivedDamage, int stage)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.EndReciept
                },
                {
                    (byte) GameParameters.RecieptData, reciept.ToBson()
                },
                {
                    (byte) GameParameters.RecivedDamage, recivedDamage
                },
                {
                    (byte) GameParameters.Stage, stage
                },
            }, true);
        }
        public void SendGetReciepts(int page, ReceptTypes type)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.GetReciepts
                },
                {
                    (byte) GameParameters.RecieptsPage, page
                },
                {
                    (byte) GameParameters.RecieptType, (int)type
                }
            }, true);
        }
        public void SendGetAllQuest()
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.GetAllReciepts
                },
            }, true);
        }
        public void SendGetReciept(string recieptId)
        {
            if (string.IsNullOrEmpty(recieptId))
            {
                throw new Exception("reciept is null! failed");
            }

            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.GetReciept
                },
                {
                    (byte) GameParameters.RecieptId, recieptId
                }
            }, true);
        }
        #endregion

        #region Economic

        public void SendGetElementsOrder()
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.GetElementsOrders
                },
            }, true);
        }

        public void SendGetElementsPriceList()
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.GetElementPrices
                },
            }, true);
        }

        public void SendSellElement(string selectedElement, int value)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.SellElement
                },
                {
                    (byte) GameParameters.ElementId, selectedElement
                },
                {
                    (byte) GameParameters.Quantity, value
                },
            }, true);
        }

        public void SendBuyElement(string selectedElement, int value)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.OrderElement
                },
                {
                    (byte) GameParameters.ElementId, selectedElement
                },
                {
                    (byte) GameParameters.Quantity, value
                },
            }, true);
        }

        public void SendSaveReservedFields(List<string> r1, List<string> r2, List<string> r3, List<string> r4,
            List<string> r5)
        {
            var lc1 = new SimpleListContainer{List = r1};
            var lc2 = new SimpleListContainer { List = r2 };
            var lc3 = new SimpleListContainer { List = r3 };
            var lc4 = new SimpleListContainer { List = r4 };
            var lc5 = new SimpleListContainer { List = r5 };

            var res1 = lc1.ToBson();
            var res2 = lc2.ToBson();
            var res3 = lc3.ToBson();
            var res4 = lc4.ToBson();
            var res5 = lc5.ToBson();
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.SaveReservedFieldsForCharacter
                },
                {(byte) GameParameters.RecieptsPage, res1},
            }, true);
        }

        #endregion

        #region Equipment

        public void SendDestroyEquipment(string eqId)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.DestroyEquipment
                },
                {(byte) GameParameters.EquipmentId, eqId}
            }, true);
        }

        public void SendEnchanceEquipment(string eqId, int value, string elementId)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.EnchantEquipment
                },
                {(byte) GameParameters.EquipmentId, eqId},
                {(byte) GameParameters.EnchantValue, value},
                {(byte) GameParameters.ElementId, elementId}
            }, true);
        }

        public void SendUnWearEquipment(string eqId, EquipmentPlaceTypes place)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.TryUnwearEquipment
                },
                {(byte) GameParameters.EquipmentId, eqId},
                {(byte) GameParameters.EquipmentPlace, (int) place}
            }, true);
        }

        public void WearEquipment(string eqId, EquipmentPlaceTypes place)
        {
            peer.OpCustom((byte) GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.TryWearEquipment
                },
                {(byte) GameParameters.EquipmentId, eqId},
                {(byte) GameParameters.EquipmentPlace, (int) place}
            }, true);
        }

        public void SendInsertModule(string moduleId, ArmorPartTypes armorPartType)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.InsertModule
                },
                {(byte) GameParameters.ModuleId, moduleId},
                {(byte) GameParameters.ArmortPartType, (int) armorPartType}
            }, true);
        }

        public void SendRemoveModule(string moduleId)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.RemoveModule
                },
                {(byte) GameParameters.ModuleId, moduleId},
            }, true);
        }

        public void SendAddClassToCharacter(ClassTypes classType)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.AddClass
                },
                {(byte) GameParameters.ClassType, classType},
            }, true);
        }

        public void SendRemoveClassFromCharacter(ClassTypes classType)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.RemoveClass
                },
                {(byte) GameParameters.ClassType, classType},
            }, true);
        }
        #endregion

        public void SendAddAbilityToCharacter(string abiName, int position)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.AddAbility
                },
                {(byte) GameParameters.Name, abiName},
                {(byte) GameParameters.Position, position},

            }, true);
        }

        public void SendRemoveAbilityFromCharacter(string abiName)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.RemoveAbility
                },
                {(byte) GameParameters.Name, abiName},

            }, true);
        }

        public void SendSetCraftExperience(int experienceValue)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.SetCraftExperience
                },
                {(byte) GameParameters.CraftExperience, experienceValue},

            }, true);
        }

        public void SendSetExperience(int experienceValue)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.SetExperience
                },
                {(byte) GameParameters.Experience, experienceValue},

            }, true);
        }

        public void RequestVictoryPrizes()
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.RequestVictoryPrizes
                },
            }, true);
        }

        public void SendSetExperience(CharacteristicE characteristicType, int skillPercent)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.SetSkillPercentValue
                },
                {(byte) GameParameters.CharacteristicType, characteristicType},
                {(byte) GameParameters.SkillPercent, skillPercent},

            }, true);
        }

        public void SendSelectVictoryPrizes(int[] ids, int prizeGold)
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.SelectVictoryPrizes
                },
                {(byte) GameParameters.VictoryIds, ids},
                {(byte) GameParameters.Money, prizeGold }
            }, true);
        }

        public void SendIncreaseAllyMaxForce()
        {
            peer.OpCustom((byte)GameOpCode.CustomOp, new Dictionary<byte, object>
            {
                {
                    (byte) GameParameters.ControlCode, GameOpCode.IncreaseMaxAllyForce
                },
            }, true);
        }
    }
}