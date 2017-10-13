using System.Collections.Generic;
using ExitGames.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Photon.SocketServer;
using Warsmiths.Client;
using Warsmiths.Common;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Factories;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.GameServer.Tasks.Common;
using Warsmiths.Server.Handlers.Craft;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Characters;
using Warsmiths.Server.Operations.Response.Character;

namespace Warsmiths.Server.Handlers.Characters
{
    public class CreateCharacterHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.CreateCharacter;

        public override OperationResponse Handle(OperationRequest operationRequest,
            SendParameters sendParameters
            , PeerBase peerBase)
        {
            OperationResponse response;

            var request = new CreateCharacterRequest(peerBase.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var peer = (MasterClientPeer) peerBase;

            var newCharacter = CharacterFactory.CreateDefaultCharacter(request.Name, (ClassTypes) request.ClassTypes,
                (HeroTypes) request.Hero, (RaceTypes) request.Race);

            var currentPlayer = peer.GetCurrentPlayer();
            // five first item

            var character = currentPlayer.GetCharacterByName(request.Name);

            if (character == null)
            {
                newCharacter.OwnerId = currentPlayer._id;
                newCharacter.Selected = true;

                currentPlayer.Characters.Add(newCharacter);
                _log.Debug(currentPlayer._id +":player id ");

                newCharacter.TasksList = new List<IEntity>(MasterApplication.TaskList.DeepClone());
                ((Warsmiths.Common.Domain.Tasks.Task)newCharacter.TasksList[0]).Status = TaskStatusTypesE.Finished;
                ((Warsmiths.Common.Domain.Tasks.Task)newCharacter.TasksList[1]).Status = TaskStatusTypesE.InProgress;
                TaskOperations.CheckTask(currentPlayer, newCharacter, TaskTypesE.CreateCharacter);
                SetCraftExperienceHandler.LevlUp(currentPlayer, newCharacter, true);


                _playerRepository.Update(currentPlayer);

                response = new OperationResponse(operationRequest.OperationCode,
                    new CharacterCreatedResponse {CharacterName = request.Name})
                {
                    ReturnCode = (short) ErrorCode.Ok,
                    DebugMessage = $"сharacted with name '{request.Name}' created"
                };

                peer.SendUpdateCharactersList();
            }
            else
            {
                response = new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short) ErrorCode.OperationFailed,
                    DebugMessage = "Character already created"
                };
            }

            return response;
        }
    }
}