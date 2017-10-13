using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Common;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Economics;

namespace Warsmiths.Server.Handlers.Economic
{
    public class ChangeCurrencyHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.ChangeCurrency;

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            var peer = (MasterClientPeer)peerBase;

            var request = new ChangeCurrencyRequest(peer.Protocol, operationRequest);

            var currentPlayer = peer.GetCurrentPlayer();
            switch ((CurrencyTypeE)request.CurrencyType)
            {
                case CurrencyTypeE.Gold:
                    currentPlayer.Gold += request.CurrencyValue;
                    if (currentPlayer.Gold < 0)
                    {
                        currentPlayer.Gold = 0;
                    }

                    break;
                case CurrencyTypeE.Crystal:
                    currentPlayer.Crystals += request.CurrencyValue;
                    if (currentPlayer.Crystals < 0)
                    {
                        currentPlayer.Crystals = 0;
                    }

                    break;
                case CurrencyTypeE.Keys:
                    currentPlayer.Keys += request.CurrencyValue;
                    if (currentPlayer.Keys < 0)
                    {
                        currentPlayer.Keys = 0;
                    }

                    break;
                case CurrencyTypeE.HealBox:
                    currentPlayer.HealBox += request.CurrencyValue;
                    if (currentPlayer.HealBox < 0)
                    {
                        currentPlayer.HealBox = 0;
                    }

                    break;
            }

            _playerRepository.Update(currentPlayer);
            peer.SendUpdateCurrency();

            var response =
                new OperationResponse(operationRequest.OperationCode)
                    {
                        ReturnCode = (short)ErrorCode.Ok,
                        DebugMessage = "Lot not found!"
                    };
            return response;
        }
    }
}