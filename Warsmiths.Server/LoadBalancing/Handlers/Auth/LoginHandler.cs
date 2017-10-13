using System;
using System.Linq;
using ExitGames.Logging;
using MongoDB.Bson.Serialization;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Craft.Quest;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Auth;

namespace Warsmiths.Server.Handlers.Auth
{
    public class LoginHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.Login;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters
            , PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer) peerBase;

            var request = new LoginRequest(peer.Protocol, operationRequest);

            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            if (peer.CurrentPlayerId != null)
            {
                return new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short) ErrorCode.OperationFailed,
                    DebugMessage = "Already logged"
                };
            }

            string debugMessage;
            Warsmiths.Common.Domain.Player player = null;

            bool result;

            var players = _playerRepository.SearchFor(t => t.Login == request.LoginReg);

            var playersCount = players.Count;

            switch (playersCount)
            {
                case 0:
                    debugMessage = "not registred";
                    result = false;
                    break;

                case 1:
                    var playerCheck = players.First();
                    if (playerCheck.Password == request.Password)
                    {
                        player = playerCheck;
                        debugMessage = "success";
                        result = true;
                    }
                    else
                    {
                        player = null;
                        debugMessage = "wrong login/password";
                        result = false;
                    }
                    break;

                default:
                    player = null;
                    debugMessage = "more than one instance of the player was found";
                    result = false;
                    break;
            }

            if (result)
            {
                peer.UserId = player._id;
                player.Online = true;
                player.LastEnterDateTime = DateTime.UtcNow;

                if (peer.CurrentPlayerId == null)
                {
                    peer.CurrentPlayerId = player._id;
                }
                else
                {
                    if (peer.CurrentPlayerId != player._id)
                    {
                        _log.ErrorFormat("prev player not correctly logout");
                        peer.CurrentPlayerId = player._id;
                    }
                    else
                    {
                        if (_log.IsDebugEnabled)
                        {
                            _log.DebugFormat("loaded minded player profile: {0}, result:{1}", request.LoginReg);
                        }
                    }
                }

                _playerRepository.Update(player);

                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short) ErrorCode.Ok,
                    DebugMessage = debugMessage
                };

                // send domain config
                peer.SendDomainConfigurationEvent();

                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("Login for player: {0}, result:{1}", request.LoginReg, debugMessage);
                }
            }
            else
            {
                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short) ErrorCode.OperationFailed,
                    DebugMessage = debugMessage
                };
            }

            return response;
        }
    }
}