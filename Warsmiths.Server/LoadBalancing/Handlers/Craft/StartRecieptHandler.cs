using System.Linq;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Client;
using Warsmiths.Common;
using Warsmiths.Common.Domain;

using Warsmiths.Common.Domain.Craft.Quest;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Common;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Craft;
using Warsmiths.Server.Operations.Response.Craft;
using CraftCustomController = Warsmiths.Server.GameServer.Craft.Field.CraftCustomController;
using CraftQuestController = Warsmiths.Server.GameServer.Craft.Field.CraftQuestController;

namespace Warsmiths.Server.Handlers.Craft
{
    public class StartRecieptHandler : BaseHandler
    {

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        private readonly RecieptRepository _recieptRepository = new RecieptRepository();
        public override OperationCode ControlCode => OperationCode.StartReciept;

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters, PeerBase peerBase)
        {
            _log.Debug("Try Start reciept");
            var peer = (MasterClientPeer)peerBase;
            var request = new StartRecieptRequest(peer.Protocol, operationRequest);

            var currentPlayer = peer.GetCurrentPlayer();
            var currentCharacter = currentPlayer.GetCurrentCharacter();


            BaseReciept reciept = null;

            var startReciept = request.Reciept.FromBson<BaseReciept>();

         
            if (startReciept!=null)
            {
                if (startReciept is BaseQuest)
                {
                    var allqusests = _recieptRepository.GetAll();
                    reciept = allqusests.FirstOrDefault(
                        x => x.OwnerId == currentPlayer._id && x.Name == startReciept.Name);

                    if (reciept == null)
                    {
                        _log.Debug(startReciept + " no found ");
                        return new OperationResponse
                        {
                            ReturnCode = (short) ErrorCode.OperationInvalid,
                            DebugMessage = "No found"
                        };
                    }

                    reciept.StageList = MasterApplication.Reciepts.Find(x => x.Name == reciept.Name).StageList;

                    _log.Debug(startReciept.Name + " succusses , id : " + reciept._id);
                }
                else
                {

                    reciept = MasterApplication.Reciepts.Find(x => x.Name == startReciept.Name);
                    reciept.Perks = startReciept.Perks;
                    reciept.ElementsList = startReciept.ElementsList;
                    _log.Debug(reciept.Perks.Count);
                    _log.Debug(reciept.ElementsList.Count);
                }

                if (MasterApplication.CraftFields.ContainsKey(currentPlayer.Login))
                {
                    if (MasterApplication.CraftFields[currentPlayer.Login] != null)
                    {
                        if (startReciept is BaseQuest && MasterApplication.CraftFields[currentPlayer.Login].Reciept.Name == startReciept.Name && startReciept.Status == RecieptStatus.IsFinish)
                        {
                            _log.Debug("Its same Quest, erase last resoult : " + currentCharacter.CraftLevel + ":" + currentCharacter.CraftExperience);
                            PlayerCommon.DicreseCraftExpririence(currentPlayer, currentCharacter, MasterApplication.CraftFields[currentPlayer.Login].Resoult.Expereince);
                            _log.Debug("End exist recept : " + currentCharacter.CraftLevel + ":" +
                                       currentCharacter.CraftExperience);
                            peer.SendUpdatePlayerProfileEvent();
                        }
                        MasterApplication.CraftFields[currentPlayer.Login].End();
                    }
                    MasterApplication.CraftFields.Remove(currentPlayer.Login);
                }

                if (reciept is BaseQuest)
                {
                    MasterApplication.CraftFields.Add(currentPlayer.Login,
                        new CraftQuestController(reciept, currentPlayer));
                }
                else
                {
                    MasterApplication.CraftFields.Add(currentPlayer.Login,
                        new CraftCustomController(reciept, currentPlayer));
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
                new StartRecieptResponse { RecieptData = reciept.ToBson()  })
            {
                ReturnCode = (short)ErrorCode.Ok,
                DebugMessage = "ok"
            };

            return response;
        }
    }
}

