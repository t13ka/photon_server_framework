using System;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Characters;

namespace Warsmiths.Server.Handlers.Characters
{
    public class SetExperienceHandler : BaseHandler
    {
        public static void LevlUp(Warsmiths.Common.Domain.Player player, Character character, bool zerolevel = false)
        {

            foreach (var a in DomainConfiguration.HeroLevelRewards[character.CraftLevel].LevelFeatures)
            {
                if (!player.LevelFeatures.Contains(a))
                    player.LevelFeatures.Add(a);
            }

            foreach (var b in DomainConfiguration.HeroLevelRewards[character.CraftLevel].Reciepts)
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

            LogManager.GetLogger("").Debug("Lvl Up " + character.Level);
        }

        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public override OperationCode ControlCode => OperationCode.SetExperience;

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters, PeerBase peerBase)
        {
            OperationResponse response;
            var peer = (MasterClientPeer)peerBase;

            var request = new SetExperienceRequest(peer.Protocol, operationRequest);
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
                    DebugMessage = $"character not selected"
                };
            }

            character.SetExperience(request.Experience);
            LevlUp(currentPlayer,character);
            character.Update();

            _playerRepository.Update(currentPlayer);

            

            response = new OperationResponse(operationRequest.OperationCode)
            {
                ReturnCode = (short)ErrorCode.Ok,
                DebugMessage = "setted. character.Experience now = " + character.CraftExperience
            };

            return response;
        }
    }
}