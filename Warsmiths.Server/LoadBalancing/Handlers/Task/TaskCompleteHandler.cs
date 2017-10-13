using System.Diagnostics;
using System.Linq;
using ExitGames.Logging;
using Photon.SocketServer;
using Warsmiths.Client;
using Warsmiths.Common;
using Warsmiths.Common.Domain.Craft.Quest;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.ListContainer;
using Warsmiths.DatabaseService.Repositories;
using Warsmiths.Server.Events;
using Warsmiths.Server.Framework.Handlers;
using Warsmiths.Server.GameServer.Tasks.Common;
using Warsmiths.Server.MasterServer;
using Warsmiths.Server.Operations.Request.Craft;
using Warsmiths.Server.Operations.Request.Tasks;
using Warsmiths.Server.Operations.Response.Craft;
using Warsmiths.Server.Operations.Response.Tasks;

namespace Warsmiths.Server.Handlers.Task
{
    public class TaskCompleteHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        private readonly PlayerRepository _playerRepository = new PlayerRepository();


        public override OperationCode ControlCode => OperationCode.CompleteTask;

        public override OperationResponse Handle(OperationRequest operationRequest, SendParameters sendParameters, PeerBase peerBase)
        {
            var peer = (MasterClientPeer)peerBase;
            var request = new TaskCompleteRequest(peer.Protocol, operationRequest);
            
            var currentPlayer = peer.GetCurrentPlayer();
            var character = currentPlayer.GetCurrentCharacter();
            var taskType = (TaskTypesE) (int.Parse(request.TaskName));

            _log.Debug("Task chek" + taskType + ":" + taskType);
            var checkTask = TaskOperations.CheckTask(currentPlayer, character, taskType);


           /* var tasklist = character.TasksList.Cast<Warsmiths.Common.Domain.Tasks.Task>().ToList();
            var firstTask = tasklist.Find(x => x.Status == TaskStatusTypesE.InProgress);
            _log.Debug("Task chek" + firstTask.Name);
            var currentTasks = tasklist.Where(x => x.Lvl == firstTask.Lvl).ToList();
            _log.Debug("Task chek" + currentTasks[0].Name);
            var a = currentTasks.Find(x => x.Type == request.TaskType) != null;
           _log.Debug("task "+ request.TaskType +":"+  a);*/

            if (checkTask == null)
            {
                _log.Debug("Fail");
                return new OperationResponse(operationRequest.OperationCode)
                {
                    ReturnCode = (short) ErrorCode.OperationFailed,
                    DebugMessage = $"not task completed"
                };
            }
            _log.Debug("Complete");
            character.Update();
            _playerRepository.Update(currentPlayer);
            peer.SendUpdatePlayerProfileEvent();


            var response = new OperationResponse(operationRequest.OperationCode,
                new TaskCompleteResponse { CompletedTask = checkTask.ToBson()})
            {
                ReturnCode = (short)ErrorCode.Ok,
                DebugMessage = "ok"
            };

            return response;
        }
    }
}
