using System;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Craft;

namespace Warsmiths.Server.Handlers.Craft
{
    public class SetCraftExperienceHandler : BaseHandler
    {
        public static void LevlUp(Warsmiths.Common.Domain.Player player, Character character, bool zerolevel = false)
        {
           if(!zerolevel) character.CraftLevel++;
            character.CraftExperience = 0; 

            foreach (var a in DomainConfiguration.CraftLevelRewards[character.CraftLevel].LevelFeatures)
            {
                if(!player.LevelFeatures.Contains(a)) player.LevelFeatures.Add(a);
            }

            foreach (var b in DomainConfiguration.CraftLevelRewards[character.CraftLevel].Reciepts)
            {
                if (character.Reciepts.Find(x => x == b) == null)
                {
                    var rec = MasterApplication.Reciepts.Find(x => x.Name == b);
                    character.Reciepts.Add(b);
                    rec._id = Guid.NewGuid().ToString();
                    rec.OwnerId = player._id;
                    new RecieptRepository().Create(rec);
                }
            }

            LogManager.GetLogger("").Debug("Lvl Up " + character.CraftLevel);
        }
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.SetCraftExperience;

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters, PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;

            var request = new SetCraftExperienceRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var currentPlayer = peer.GetCurrentPlayer();
            var character = currentPlayer.GetCurrentCharacter();

            if (character == null)
            {
                return new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short)ErrorCode.OperationFailed,
                    DebugMessage = "character not selected"
                };
            }

            character.CraftExperience += request.CraftExperience;

            _log.Debug("Current lvl " + character.CraftLevel);
            _log.Debug("Current Exp " + character.CraftExperience);
            _log.Debug("Need Exp " + DomainConfiguration.CharacterCraftLevelsExperience[character.CraftLevel]);


            while (DomainConfiguration.CharacterCraftLevelsExperience[character.CraftLevel] < 
                character.CraftExperience && character.CraftLevel < DomainConfiguration.CharacterCraftLevelsExperience.Length)
            {

                LevlUp(currentPlayer, character);
                _log.Debug("Lvl up " + character.CraftLevel);
            }

            character.Update();
            _playerRepository.Update(currentPlayer); 

             peer.SendUpdatePlayerProfileEvent();
        

            response = new OperationResponse(operationRequest.OperationCode)
            {
                ReturnCode = (short)ErrorCode.Ok,
                DebugMessage = "setted. character.CraftExperience now = " + character.CraftExperience
            };

            return response;
        }
    }
}