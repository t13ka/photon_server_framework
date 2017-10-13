using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Client;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Craft;
using Warsmiths.Common.Domain.Craft.Quest;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Craft;
using Warsmiths.Server.Operations.Response.Craft;

namespace Warsmiths.Server.Handlers.Craft
{
    public class GetRecieptsHandler : BaseHandler
    {

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        private readonly RecieptRepository _recieptRepository = new RecieptRepository();

        public override OperationCode ControlCode => OperationCode.GetReciepts;

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters, PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;
            var currentPlayer = peer.GetCurrentPlayer();

            var request = new GetRecieptsRequest(peer.Protocol, operationRequest);
            
            //if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
             //   return response;
            }
            var allreciepts = _recieptRepository.GetAllByOwner(currentPlayer._id).ToList();

            var shortedReciept = new List<BaseReciept>();

            var maxRecieptsInPack = 4;
            var recieptsInPack = 0;
            _log.Debug("Try get all " + request.Type + " page " + request.Page);
            if ((ReceptTypes)request.Type == ReceptTypes.Quest)
            {
                allreciepts.RemoveAll(x => x.ReceptType == ReceptTypes.Dummy);
                var primalList = new List<BaseReciept>(allreciepts);

                var newReclist = new List<BaseReciept>();
                primalList = primalList.OrderByDescending(x => x.RequiredLevel).ToList();
                var lvl = -1;
                foreach (var n in primalList)
                {
                    if (lvl == n.RequiredLevel) continue;
                    if (n is BaseQuest)
                    {
                        //a
                        lvl = n.RequiredLevel;
                        var newlist = primalList.Where(x => x.RequiredLevel == lvl);
                        newlist = newlist.OrderByDescending(x => x.Prefix);
                        newReclist.AddRange(newlist);
                    }
                }



                _log.Debug("quest Page " + request.Page + ":" + newReclist.Count);

                for (var i = 0; i < newReclist.Count; i++)
                {
                    var qrec = newReclist[i] as BaseQuest;
                    if (qrec == null || !string.IsNullOrEmpty(qrec.PrevQuest))
                    {
                        newReclist.Remove(newReclist[i]);
                        i--;
                    }
                }
                _log.Debug(" amout  " + newReclist.Count);
                for (var i = request.Page * maxRecieptsInPack; i < newReclist.Count && recieptsInPack < maxRecieptsInPack; i++)
                {

                    var questQuene = new List<BaseReciept>(Warsmiths.Common.Domain.Craft.Common.CalculateQuestQuene(
                        (BaseQuest) newReclist[i],
                        MasterApplication.Reciepts)).DeepClone();


                    _log.Debug(newReclist[i].Name + ":" + i);

                    for (var z = 0; z < questQuene.Count; z++)
                    {
                        var exist = allreciepts.Find(x => x.Name == questQuene[z].Name);
                        if (exist != null)
                        {
                            questQuene[z] = exist;
                        }
                        questQuene[z].StageList = null;
                        questQuene[z].RecieptSpells = null;
                        questQuene[z].PartsPosition = null;
                        shortedReciept.Add(questQuene[z]);
                        _log.Debug(questQuene[z].Name + "added:");
                    }
                }

                recieptsInPack++;
            }
            else if((ReceptTypes)request.Type == ReceptTypes.Dummy)
            {
                for (var i = request.Page * maxRecieptsInPack; i < allreciepts.Count && recieptsInPack < maxRecieptsInPack; i++)
                {
                    if (allreciepts[i].ReceptType == ReceptTypes.Dummy)
                    {
                        var exist = allreciepts[i];
                        exist.StageList = null;
                        exist.RecieptSpells = null;
                        exist.PartsPosition = null;
                        shortedReciept.Add(exist);
                        _log.Debug(exist.Name + "added:");
                    }
                }

                recieptsInPack++;
            }


            _log.Debug(shortedReciept.Count + " Reciepts ready to Send");
            if (shortedReciept.Count <= 0)
            {
                _log.Debug("no quests");
                return new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.OperationFailed,
                    DebugMessage = "No quests"
                };
            }
            var container = new RecieptsContainer
            {
                Reciepts = shortedReciept.ToArray()
            };
            var recData = container.ToBson();
            _log.Debug("Reciepts size is " + recData.Length);
            response = new OperationResponse(operationRequest.OperationCode,
                new GetRecieptsResponse { RecieptData = recData })
            {
                ReturnCode = (short)ErrorCode.Ok,
                DebugMessage = "ok"
            };

            return response;
        }
    }
}

