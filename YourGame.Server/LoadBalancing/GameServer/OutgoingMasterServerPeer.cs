using System;
using System.Collections.Generic;
using System.Net;
using ExitGames.Logging;
using Photon.SocketServer;
using Photon.SocketServer.ServerToServer;
using PhotonHostRuntimeInterfaces;

using YourGame.Server.Common;
using YourGame.Server.LoadShedding;
using YourGame.Server.ServerToServer.Events;
using YourGame.Server.ServerToServer.Operations;

namespace YourGame.Server.GameServer
{
    using YourGame.Common;
    using YourGame.Server.Framework;
    using YourGame.Server.Framework.Messages;
    using YourGame.Server.GameServer;

    using OperationCode = YourGame.Server.ServerToServer.Operations.OperationCode;
    using ParameterCode = YourGame.Server.Operations.ParameterCode;

    public class OutgoingMasterServerPeer : ServerPeerBase
    {
        #region Constructors and Destructors

        public OutgoingMasterServerPeer(IRpcProtocol protocol, IPhotonPeer nativePeer, GameApplication application)
            : base(protocol, nativePeer)
        {
            _application = application;
            Log.InfoFormat("connection to master at {0}:{1} established (id={2}), serverId={3}", RemoteIP, RemotePort,
                ConnectionId, GameApplication.ServerId);
            RequestFiber.Enqueue(Register);
        }

        #endregion

        #region Properties

        protected bool IsRegistered ;

        #endregion

        #region Constants and Fields

        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly GameApplication _application;

        private bool _redirected;

        private IDisposable _updateLoop;

        #endregion

        #region Public Methods

        public void RemoveGameState(string gameId)
        {
            if (!IsRegistered)
            {
                return;
            }

            var parameter = new Dictionary<byte, object> {{(byte) ParameterCode.GameId, gameId}};
            var eventData = new EventData {Code = (byte) ServerEventCode.RemoveGameState, Parameters = parameter};
            SendEvent(eventData, new SendParameters());
        }

        public virtual void UpdateAllGameStates()
        {
            if (!IsRegistered)
            {
                return;
            }

            foreach (var gameId in GameCache.Instance.GetRoomNames())
            {
                Room room;
                if (GameCache.Instance.TryGetRoomWithoutReference(gameId, out room))
                {
                    room.EnqueueMessage(new RoomMessage((byte) GameMessageCodes.ReinitializeGameStateOnMaster));
                }
            }
        }

        public void UpdateServerState(FeedbackLevel workload, int peerCount, ServerState state)
        {
            if (!IsRegistered)
            {
                return;
            }

            var contract = new UpdateServerEvent
            {
                LoadIndex = (byte) workload,
                PeerCount = peerCount,
                State = (int) state
            };
            var eventData = new EventData((byte) ServerEventCode.UpdateServer, contract);
            SendEvent(eventData, new SendParameters());
        }

        public void UpdateServerState()
        {
            if (Connected == false)
            {
                return;
            }

            UpdateServerState(
                GameApplication.Instance.WorkloadController.FeedbackLevel,
                GameApplication.Instance.PeerCount,
                GameApplication.Instance.WorkloadController.ServerState);
        }

        #endregion

        #region Methods

        protected virtual void HandleRegisterGameServerResponse(OperationResponse operationResponse)
        {
            var contract = new RegisterGameServerResponse(Protocol, operationResponse);
            if (!contract.IsValid)
            {
                if (operationResponse.ReturnCode != (short) ErrorCode.Ok)
                {
                    Log.ErrorFormat("RegisterGameServer returned with err {0}: {1}", operationResponse.ReturnCode,
                        operationResponse.DebugMessage);
                }

                Log.Error("RegisterGameServerResponse contract invalid: " + contract.GetErrorMessage());
                Disconnect();
                return;
            }

            switch (operationResponse.ReturnCode)
            {
                case (short) ErrorCode.Ok:
                {
                    Log.InfoFormat("Successfully registered at master server: serverId={0}", GameApplication.ServerId);
                    IsRegistered = true;
                    UpdateAllGameStates();
                    StartUpdateLoop();
                    break;
                }

                case (short) ErrorCode.RedirectRepeat:
                {
                    // TODO: decide whether to connect to internal or external address (config)
                    // use a new peer since we otherwise might get confused with callbacks like disconnect
                    var address = new IPAddress(contract.InternalAddress);
                    Log.InfoFormat("Connected master server is not the leader; Reconnecting to master at IP {0}...",
                        address);
                    Reconnect(address); // don't use proxy for direct connections

                    // enable for external address connections
                    //// var address = new IPAddress(contract.ExternalAddress);
                    //// log.InfoFormat("Connected master server is not the leader; Reconnecting to node {0} at IP {1}...", contract.MasterNode, address);
                    //// this.Reconnect(address, contract.MasterNode);
                    break;
                }

                default:
                {
                    Log.WarnFormat("Failed to register at master: err={0}, msg={1}, serverid={2}",
                        operationResponse.ReturnCode, operationResponse.DebugMessage, GameApplication.ServerId);
                    Disconnect();
                    break;
                }
            }
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            IsRegistered = false;
            StopUpdateLoop();

            // if RegisterGameServerResponse tells us to connect somewhere else we don't need to reconnect here
            if (_redirected)
            {
                if (Log.IsDebugEnabled)
                {
                    Log.DebugFormat("{0} disconnected from master server: reason={1}, detail={2}, serverId={3}",
                        ConnectionId, reasonCode, reasonDetail, GameApplication.ServerId);
                }
            }
            else
            {
                Log.InfoFormat("connection to master closed (id={0}, reason={1}, detail={2}), serverId={3}",
                    ConnectionId, reasonCode, reasonDetail, GameApplication.ServerId);
                _application.ReconnectToMaster();
            }
        }

        protected override void OnEvent(IEventData eventData, SendParameters sendParameters)
        {
        }

        protected override void OnOperationRequest(OperationRequest request, SendParameters sendParameters)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Received unknown operation code {0}", request.OperationCode);
            }

            var response = new OperationResponse
            {
                OperationCode = request.OperationCode,
                ReturnCode = -1,
                DebugMessage = "Unknown operation code"
            };
            SendOperationResponse(response, sendParameters);
        }

        protected override void OnOperationResponse(OperationResponse operationResponse, SendParameters sendParameters)
        {
            switch ((OperationCode) operationResponse.OperationCode)
            {
                default:
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Received unknown operation code {0}", operationResponse.OperationCode);
                    }

                    break;
                }

                case OperationCode.RegisterGameServer:
                {
                    HandleRegisterGameServerResponse(operationResponse);
                    break;
                }
            }
        }

        protected void Reconnect(IPAddress address)
        {
            _redirected = true;

            Log.InfoFormat("Reconnecting to master: serverId={0}", GameApplication.ServerId);

            GameApplication.Instance.ConnectToMaster(new IPEndPoint(address, RemotePort));
            Disconnect();
            Dispose();
        }

        protected virtual void Register()
        {
            var contract = new RegisterGameServer
            {
                GameServerAddress = GameApplication.Instance.PublicIpAddress.ToString(),
                UdpPort =
                    GameServerSettings.Default.RelayPortUdp == 0
                        ? _application.GamingUdpPort
                        : GameServerSettings.Default.RelayPortUdp + _application.GetCurrentNodeId() - 1,
                TcpPort =
                    GameServerSettings.Default.RelayPortTcp == 0
                        ? _application.GamingTcpPort
                        : GameServerSettings.Default.RelayPortTcp + _application.GetCurrentNodeId() - 1,
                WebSocketPort =
                    GameServerSettings.Default.RelayPortWebSocket == 0
                        ? _application.GamingWebSocketPort
                        : GameServerSettings.Default.RelayPortWebSocket + _application.GetCurrentNodeId() - 1,
                ServerId = GameApplication.ServerId.ToString(),
                ServerState = (int) _application.WorkloadController.ServerState
            };

            if (Log.IsInfoEnabled)
            {
                Log.InfoFormat(
                    "Registering game server with address {0}, TCP {1}, UDP {2}, WebSocket {3}, ServerID {4}",
                    contract.GameServerAddress,
                    contract.TcpPort,
                    contract.UdpPort,
                    contract.WebSocketPort,
                    contract.ServerId);
            }

            var request = new OperationRequest((byte) OperationCode.RegisterGameServer, contract);
            SendOperationRequest(request, new SendParameters());
        }

        protected void StartUpdateLoop()
        {
            if (_updateLoop != null)
            {
                Log.Error("Update Loop already started! Duplicate RegisterGameServer response?");
                _updateLoop.Dispose();
            }

            _updateLoop = RequestFiber.ScheduleOnInterval(UpdateServerState, 1000, 1000);
            GameApplication.Instance.WorkloadController.FeedbacklevelChanged +=
                WorkloadController_OnFeedbacklevelChanged;
        }

        protected void StopUpdateLoop()
        {
            if (_updateLoop != null)
            {
                _updateLoop.Dispose();
                _updateLoop = null;

                GameApplication.Instance.WorkloadController.FeedbacklevelChanged -=
                    WorkloadController_OnFeedbacklevelChanged;
            }
        }

        private void WorkloadController_OnFeedbacklevelChanged(object sender, EventArgs e)
        {
            UpdateServerState();
        }

        #endregion
    }
}