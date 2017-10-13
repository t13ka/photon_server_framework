using System.Linq;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Client;
using Warsmiths.Common;
using Warsmiths.Common.Domain;

using Warsmiths.Common.Domain.Craft.Quest;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Common;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Craft;
using Warsmiths.Server.Operations.Response.Craft;
using CraftCustomController = Warsmiths.Server.GameServer.Craft.Field.CraftCustomController;
using CraftQuestController = Warsmiths.Server.GameServer.Craft.Field.CraftQuestController;

namespace Warsmiths.Server.Handlers.Craft
{
    public class GetLastQuestHandler : BaseHandler
    {

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        private readonly RecieptRepository _recieptRepository = new RecieptRepository();
        public override OperationCode ControlCode => OperationCode.LastRecieptQuest;

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters, PeerBase peerBase)
        {
            _log.Debug("Try Start reciept");
            var peer = (MasterClientPeer)peerBase;
            var request = new StartRecieptRequest(peer.Protocol, operationRequest);

            var currentPlayer = peer.GetCurrentPlayer();
            var currentCharacter = currentPlayer.GetCurrentCharacter();
            BaseReciept nextQuest;

            var startQuest = request.Reciept.FromBson<BaseReciept>();

            if (MasterApplication.CraftFields.ContainsKey(currentPlayer.Login))
            {
                _log.Debug("Its same Quest, erase last resoult : " + currentCharacter.CraftLevel +":" + currentCharacter.CraftExperience);
                PlayerCommon.DicreseCraftExpririence(currentPlayer,currentCharacter, MasterApplication.CraftFields[currentPlayer.Login].Resoult.Expereince);
                _log.Debug("End exist recept : " + currentCharacter.CraftLevel + ":" + currentCharacter.CraftExperience);
                MasterApplication.CraftFields[currentPlayer.Login].End();
                MasterApplication.CraftFields.Remove(currentPlayer.Login);
            
            }

            if (startQuest!=null)
            {
                var allqusests = _recieptRepository.GetAll();
                nextQuest = allqusests.FirstOrDefault(x =>x.OwnerId == currentPlayer._id && x.Name == startQuest.Name);

                if (nextQuest == null)
                {
                    _log.Debug(startQuest + " no found ");
                    return new OperationResponse { ReturnCode = (short)ErrorCode.OperationInvalid, DebugMessage = "No found"};
                }

                nextQuest.StageList = MasterApplication.Reciepts.Find(x => x.Name == nextQuest.Name).StageList;

                _log.Debug(startQuest.Name + " succusses , id : "  + nextQuest._id);


                if (nextQuest is BaseQuest)
                {
                    MasterApplication.CraftFields.Add(currentPlayer.Login,
                        new CraftQuestController(nextQuest, currentPlayer));
                }
                else
                {
                    MasterApplication.CraftFields.Add(currentPlayer.Login, 
                        new CraftCustomController(nextQuest, currentPlayer));
                }

         //       _playerRepository.Update(currentPlayer);
         //       peer.SendUpdatePlayerProfileEvent();
            }
            else
            {
                _log.Debug(" no found ");
                return new OperationResponse { ReturnCode = (short)ErrorCode.OperationInvalid, DebugMessage = "No found" };
            }

            var response = new OperationResponse(operationRequest.OperationCode,
                new StartRecieptResponse { RecieptData = nextQuest.ToBson()  })
            {
                ReturnCode = (short)ErrorCode.Ok,
                DebugMessage = "ok"
            };

            return response;
        }
    }
}
