using System;
using System.Collections;
using ExitGames.Logging;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using PhotonHostRuntimeInterfaces;
using Warsmiths.Common;
using Warsmiths.Server.Common;
using Warsmiths.Server.Framework.Services;
using Warsmiths.Server.LoadShedding;
using Warsmiths.Server.ServerToServer.Events;
using Warsmiths.Server.ServerToServer.Operations;
using Warsmiths.Server.Services;
using OperationCode = Warsmiths.Server.ServerToServer.Operations.OperationCode;

namespace Warsmiths.Server.MasterServer.GameServer
{
    public class IncomingGameServerPeer : ServerPeerBase
    {
        #region Constructors and Destructors

        public IncomingGameServerPeer(InitRequest initRequest, MasterApplication application)
            : base(initRequest.Protocol, initRequest.PhotonPeer)
        {
            _application = application;
            Log.InfoFormat("game server connection from {0}:{1} established (id={2})", RemoteIP, RemotePort,
                ConnectionId);
        }

        #endregion

        #region Constants and Fields

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly MasterApplication _application;

        #endregion

        #region Properties

        public string Key { get; protected set; }

        public Guid? ServerId { get; protected set; }

        public string TcpAddress { get; protected set; }

        public string UdpAddress { get; protected set; }

        public string WebSocketAddress { get; protected set; }

        public FeedbackLevel LoadLevel { get; private set; }

        public ServerState State { get; private set; }

        public int PeerCount { get; private set; }

        #endregion

        #region Public Methods

        public void RemoveGameServerPeerOnMaster()
        {
            if (ServerId.HasValue)
            {
                _application.GameServers.OnDisconnect(this);
                _application.LoadBalancer.TryRemoveServer(this);
                _application.RemoveGameServerFromLobby(this);
            }
        }

        public override string ToString()
        {
            if (ServerId.HasValue)
            {
                return string.Format("GameServer({2}) at {0}/{1}", TcpAddress, UdpAddress, ServerId);
            }

            return base.ToString();
        }

        #endregion

        #region Methods

        protected virtual Hashtable GetAuthlist()
        {
            return null;
        }

        protected virtual byte[] SharedKey
        {
            get { return null; }
        }

        protected virtual OperationResponse HandleRegisterGameServerRequest(OperationRequest request)
        {
            var registerRequest = new RegisterGameServer(Protocol, request);

            if (registerRequest.IsValid == false)
            {
                var msg = registerRequest.GetErrorMessage();
                Log.ErrorFormat("RegisterGameServer contract error: {0}", msg);

                return new OperationResponse(request.OperationCode)
                {
                    DebugMessage = msg,
                    ReturnCode = (short)ErrorCode.OperationInvalid
                };
            }

            var masterAddress = _application.GetInternalMasterNodeIpAddress();
            var contract = new RegisterGameServerResponse {InternalAddress = masterAddress.GetAddressBytes()};

            // is master
            if (_application.IsMaster)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat(
                        "Received register request: Address={0}, UdpPort={1}, TcpPort={2}, WebSocketPort={3}, State={4}",
                        registerRequest.GameServerAddress,
                        registerRequest.UdpPort,
                        registerRequest.TcpPort,
                        registerRequest.WebSocketPort,
                        (ServerState) registerRequest.ServerState);
                }

                if (registerRequest.UdpPort.HasValue)
                {
                    UdpAddress = registerRequest.GameServerAddress + ":" + registerRequest.UdpPort;
                }

                if (registerRequest.TcpPort.HasValue)
                {
                    TcpAddress = registerRequest.GameServerAddress + ":" + registerRequest.TcpPort;
                }

                if (registerRequest.WebSocketPort.HasValue && registerRequest.WebSocketPort != 0)
                {
                    WebSocketAddress = registerRequest.GameServerAddress + ":" + registerRequest.WebSocketPort;
                }

                ServerId = new Guid(registerRequest.ServerId);
                State = (ServerState) registerRequest.ServerState;

                Key = $"{registerRequest.GameServerAddress}-{registerRequest.UdpPort}-{registerRequest.TcpPort}";

                _application.GameServers.OnConnect(this);

                if (State == ServerState.Normal)
                {
                    _application.LoadBalancer.TryAddServer(this, 0);
                }

                contract.AuthList = GetAuthlist();
                contract.SharedKey = SharedKey;

                return new OperationResponse(request.OperationCode, contract);
            }

            return new OperationResponse(request.OperationCode, contract)
            {
                ReturnCode = (short) ErrorCode.RedirectRepeat,
                DebugMessage = "RedirectRepeat"
            };
        }

        protected virtual void HandleRemoveGameState(IEventData eventData)
        {
            var removeEvent = new RemoveGameEvent(Protocol, eventData);
            if (removeEvent.IsValid == false)
            {
                var msg = removeEvent.GetErrorMessage();
                Log.ErrorFormat("RemoveGame contract error: {0}", msg);
                return;
            }

            _application.DefaultApplication.OnGameRemovedOnGameServer(removeEvent.GameId);
        }

        protected virtual void HandleUpdateGameServerEvent(IEventData eventData)
        {
            var updateGameServer = new UpdateServerEvent(Protocol, eventData);
            if (updateGameServer.IsValid == false)
            {
                var msg = updateGameServer.GetErrorMessage();
                Log.ErrorFormat("UpdateServer contract error: {0}", msg);
                return;
            }

            var previuosLoadLevel = LoadLevel;

            LoadLevel = (FeedbackLevel) updateGameServer.LoadIndex;
            PeerCount = updateGameServer.PeerCount;

            if ((ServerState) updateGameServer.State != State)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("GameServer state changed for {0}: old={1}, new={2} ", TcpAddress, State,
                        (ServerState) updateGameServer.State);
                }

                State = (ServerState) updateGameServer.State;
                if (State == ServerState.Normal)
                {
                    if (_application.LoadBalancer.TryAddServer(this, LoadLevel) == false)
                    {
                        Log.WarnFormat("Failed to add game server to load balancer: serverId={0}", ServerId);
                    }
                }
                else if (State == ServerState.Offline)
                {
                    ////this.RemoveGameServerPeerOnMaster();
                }
                else
                {
                    _application.LoadBalancer.TryRemoveServer(this);
                }
            }
            else if (previuosLoadLevel != LoadLevel)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("UpdateGameServer - from LoadLevel {0} to {1}, PeerCount {2}", previuosLoadLevel,
                        LoadLevel, PeerCount);
                }

                if (!_application.LoadBalancer.TryUpdateServer(this, LoadLevel))
                {
                    Log.WarnFormat("Failed to update game server state for {0}", TcpAddress);
                }
            }
        }

        protected virtual void HandleUpdateGameState(IEventData eventData)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("HandleUpdateGameState");
            }

            var updateEvent = new UpdateGameEvent(Protocol, eventData);
            if (updateEvent.IsValid == false)
            {
                var msg = updateEvent.GetErrorMessage();
                Log.ErrorFormat("UpdateGame contract error: {0}", msg);
                return;
            }

            _application.DefaultApplication.OnGameUpdateOnGameServer(updateEvent, this);
        }

        private void HandleUpdateAppStatsEvent(IEventData eventData)
        {
            var service = ServiceManager.Get<ApplicationStatsRuntimeService>();
            if (service != null)
            {
                var updateAppStatsEvent = new UpdateAppStatsEvent(Protocol, eventData);
                service.UpdateGameServerStats(this, updateAppStatsEvent.PlayerCount,
                    updateAppStatsEvent.GameCount);
            }
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            if (Log.IsInfoEnabled)
            {
                var serverId = ServerId.HasValue ? ServerId.ToString() : "{null}";
                Log.InfoFormat(
                    "OnDisconnect: game server connection closed (connectionId={0}, serverId={1}, reason={2})",
                    ConnectionId, serverId, reasonCode);
            }

            RemoveGameServerPeerOnMaster();
        }

        protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
        {
            try
            {
                if (!ServerId.HasValue)
                {
                    Log.Warn("received game server event but server is not registered");
                    return;
                }

                switch ((ServerEventCode) eventData.Code)
                {
                    default:
                        if (Log.IsDebugEnabled)
                        {
                            Log.DebugFormat("Received unknown event code {0}", eventData.Code);
                        }

                        break;

                    case ServerEventCode.UpdateServer:
                        HandleUpdateGameServerEvent(eventData);
                        break;

                    case ServerEventCode.UpdateGameState:
                        HandleUpdateGameState(eventData);
                        break;

                    case ServerEventCode.RemoveGameState:
                        HandleRemoveGameState(eventData);
                        break;

                    case ServerEventCode.UpdateAppStats:
                        HandleUpdateAppStatsEvent(eventData);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        protected override void OnOperationRequest(OperationRequest request, SendParameters sendParameters)
        {
            try
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("OnOperationRequest: pid={0}, op={1}", ConnectionId, request.OperationCode);
                }

                OperationResponse response;

                switch ((OperationCode) request.OperationCode)
                {
                    default:
                        response = new OperationResponse(request.OperationCode)
                        {
                            ReturnCode = -1,
                            DebugMessage = "Unknown operation code"
                        };
                        break;

                    case OperationCode.RegisterGameServer:
                    {
                        response = ServerId.HasValue
                            ? new OperationResponse(request.OperationCode)
                            {
                                ReturnCode = -1,
                                DebugMessage = "already registered"
                            }
                            : HandleRegisterGameServerRequest(request);
                        break;
                    }
                }

                SendOperationResponse(response, sendParameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}