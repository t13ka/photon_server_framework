using System.Linq;

using ExitGames.Logging;

using Photon.SocketServer;

using Warsmiths.Common;
using Warsmiths.Common.Domain.VictoryPrizes;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Victory;

// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable SwitchStatementMissingSomeCases
namespace Warsmiths.Server.Handlers.Victory
{
    public class SelectVictoryPrizesHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.SelectVictoryPrizes;

        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peerBase)
        {
            OperationResponse response;

            var peer = (MasterClientPeer)peerBase;

            var request = new SelectVictoryPrizesRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            response = new OperationResponse(operationRequest.OperationCode) { ReturnCode = (short)ErrorCode.Ok, };

            var player = peer.GetCurrentPlayer();
            player.Gold += request.Money;

            _log.Info("SelectVictoryPrizesHandler");
            if (peer.LastVictoryPrizesResult != null)
            {
                for (var i = 0; i < peer.LastVictoryPrizesResult.Items.Count; i++)
                {
                    if (!request.Ids.Contains(peer.LastVictoryPrizesResult.Items[i].PrizeId)) continue;
                    _log.Info(
                        peer.LastVictoryPrizesResult.Items[i].Type + " "
                        + peer.LastVictoryPrizesResult.Items[i].PrizeId);
                    switch (peer.LastVictoryPrizesResult.Items[i].Type)
                    {
                        case VictoryPrizeTypeE.Equipment:
                            var b = player.TryAddToInventory(peer.LastVictoryPrizesResult.Items[i].Item);
                            _log.Info($"Added item: {b}; " + peer.LastVictoryPrizesResult.Items[i].Item.Name);
                            break;
                        case VictoryPrizeTypeE.Crystals:
                            player.Crystals += peer.LastVictoryPrizesResult.Items[i].CrystalAmout;
                            break;
                        case VictoryPrizeTypeE.Keys:
                            player.Keys += peer.LastVictoryPrizesResult.Items[i].KeysAmout;
                            break;
                    }
                }

                peer.LastVictoryPrizesResult.Items.Clear();
            }

            peer.LastVictoryPrizesResult = null;
            _playerRepository.Update(player);
            peer.SendUpdatePlayerProfileEvent();
            peer.SendUpdatePlayerInventoryEvent();

            return response;
        }
    }
}