using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Enums.ItemGeneration;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Victory;
using Warsmiths.Server.VictoryPrizes;

namespace Warsmiths.Server.Handlers.Victory
{
    public class RequestVictoryPrizesHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.RequestVictoryPrizes;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters, PeerBase peerBase)
        {
            OperationResponse response;

            var peer = (MasterClientPeer)peerBase;

            var request = new GetVictoryPrizesRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            response = new OperationResponse(operationRequest.OperationCode)
            {
                ReturnCode = (short)ErrorCode.Ok,
            };

            //TODO: Remove Random
            var masteryValue = DomainConfiguration.Random.NextDouble();
            var luckValue = DomainConfiguration.Random.Next(1, 5);
            var mutualValue = DomainConfiguration.Random.Next(0, 10);

            ItemGenerationMasteryTypes mastery;
            var luck = (ItemGenerationLuckTypes)luckValue;
            ItemGenerationMutualAidTypes mutualAid;

            if (masteryValue < 0.2)
            {
                mastery = ItemGenerationMasteryTypes.Regular;
            }
            else if (masteryValue < 0.3)
            {
                mastery = ItemGenerationMasteryTypes.Rare;
            }
            else  
            {
                mastery = masteryValue < 0.5 ? ItemGenerationMasteryTypes.Epic : ItemGenerationMasteryTypes.Legend;
            }
            
            // aid
            if (mutualValue < 2)
            {
                mutualAid = ItemGenerationMutualAidTypes.Regular;
            }
            else if (mutualValue < 4)
            {
                mutualAid = ItemGenerationMutualAidTypes.Rare;
            }
            else 
            {
                mutualAid = mutualValue < 6 ? ItemGenerationMutualAidTypes.Epic : ItemGenerationMutualAidTypes.Legend;
            }

            var prizes = VictoryPrizesGenerator.GetWinPrisesForPlayer(mastery, luck, mutualAid);
            peer.LastVictoryPrizesResult = prizes;
            peer.SendVictoryPrizes(prizes);

            return response;
        }
    }
}