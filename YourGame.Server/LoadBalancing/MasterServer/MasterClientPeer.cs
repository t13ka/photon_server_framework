using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ExitGames.Logging;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

using YourGame.Server.Handlers.Auth;
using YourGame.Server.MasterServer.Lobby;

namespace YourGame.Server.MasterServer
{
    using YourGame.Common;
    using YourGame.Common.Domain;
    using YourGame.Common.Domain.VictoryPrizes;
    using YourGame.DatabaseService.Repositories;
    using YourGame.Server.Framework.Handlers;
    using YourGame.Server.Framework.Services;

    public sealed class MasterClientPeer : PeerBase, ILobbyPeer
    {
        private readonly PlayerRepository _playerRepository;
        #region Constructors and Destructors

        public MasterClientPeer(InitRequest initRequest, GameApplication application)
            : base(initRequest.Protocol, initRequest.PhotonPeer)
        {
            _application = application;
            var services = ServiceManager.GetAllServices();
            foreach (var runtimeService in services)
            {
                runtimeService.Value.AddSubscriber(this);
            }
            _playerRepository = new PlayerRepository();
        }

        #endregion

        public static HandlerPicker HandlerPicker { get; } = new HandlerPicker();

        #region Constants and Fields

        /// <summary>
        /// </summary>
        public static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private IGameListSubscibtion _gameChannelSubscription;

        /// <summary>
        /// </summary>
        private GameApplication _application;

        private Character _currentCharacter;

        public VictoryPrizesResult LastVictoryPrizesResult;
        #endregion

        #region Properties

        public string UserId { get; set; }

        //public Player CurrentPlayer
        //{
        //    get
        //    {
        //        if (CurrentPlayerId == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return _playerRepository.GetById(CurrentPlayerId);
        //        }
        //    }
        //}

        public Player GetCurrentPlayer()
        {
            return _playerRepository.GetById(CurrentPlayerId);
        }

        public string CurrentPlayerId ;

        public GameApplication Application
        {
            get { return _application; }

            protected set
            {
                var oldApp = Interlocked.Exchange(ref _application, value);
                if (oldApp != null)
                {
                    oldApp.OnClientDisconnected(this);
                }

                if (value != null)
                {
                    value.OnClientConnected(this);
                }
            }
        }

        public AppLobby AppLobby ;

        public IGameListSubscibtion GameChannelSubscription
        {
            get { return _gameChannelSubscription; }
            set
            {
                var oldsubscription = Interlocked.Exchange(ref _gameChannelSubscription, value);
                oldsubscription?.Dispose();
            }
        }

        #endregion

        #region Methods

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("Disconnect: pid={0}: reason={1}, detail={2}", ConnectionId, reasonCode, reasonDetail);
            }

            // remove peer from the lobby if he has joined one
            if (AppLobby != null)
            {
                AppLobby.RemovePeer(this);
                AppLobby = null;
            }

            // remove the peer from the application
            Application = null;

            // update application 
            var services = ServiceManager.GetAllServices();
            foreach (var runtimeService in services)
            {
                runtimeService.Value.RemoveSubscriber(this);
            }

            string debugMessage;
            if (LogoutHandler.TryLogOut(this, out debugMessage))
            {
                Log.DebugFormat(debugMessage);
            }
        }

        protected override void OnOperationRequest(OperationRequest operationRequest,
            SendParameters sendParameters)
        {
            var handler = HandlerPicker.GetHandler(operationRequest);
            if (handler == null)
            {
                Log.DebugFormat(@"OnOperationRequest - can't pickup handler: pid={0}, op={1}", ConnectionId, operationRequest.OperationCode);
                return;
            }

            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat(@"OnOperationRequest: pid={0}, op={1}, control_code={2}", 
                    ConnectionId, operationRequest.OperationCode, handler.ControlCode);
            }

            OperationResponse response;
            try
            {
                if (handler.ControlCode == OperationCode.Login 
                    || handler.ControlCode == OperationCode.Authenticate
                    || handler.ControlCode == OperationCode.JoinLobby
                    || handler.ControlCode == OperationCode.Registration)
                {
                    response = handler.Handle(operationRequest, sendParameters, this);
                }
                else
                {
                    response = OperationHelper.ValidateLogin(operationRequest, this);
                    if (response == null)
                    {
                        response = handler.Handle(operationRequest, sendParameters, this);
                    }
                }
            }
            catch (Exception exception)
            {
                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short) ErrorCode.InternalServerError,
                    DebugMessage = $"handler:{handler.GetType().Name}, error:" + exception.Message
                };

                Log.Error("handling error:", exception);
            }

            if (response != null)
            {
                if (response.Parameters == null)
                {
                    response.Parameters = new Dictionary<byte, object>();
                }
                response.Parameters.Add((byte)ParameterCode.ControlCode, handler.ControlCode);
                SendOperationResponse(response, sendParameters);
            }
        }

        #endregion
    }
}