using System.Collections.Generic;
using System.Linq;

using ExitGames.Logging;

using Photon.SocketServer;

using YourGame.Server.MasterServer;
using YourGame.Server.Operations.Request.Auth;

using PlayerFactory = YourGame.Server.Factories.PlayerFactory;

namespace YourGame.Server.Handlers.Auth
{
    using YourGame.Common;
    using YourGame.Common.Domain;
    using YourGame.Common.Domain.Equipment;
    using YourGame.DatabaseService.Repositories;
    using YourGame.Server.Framework.Handlers;

    public class AccountRegistrationHandler : BaseHandler
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// </summary>
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public override OperationCode ControlCode => OperationCode.Registration;

        public override OperationResponse Handle(
            OperationRequest operationRequest,
            SendParameters sendParameters,
            PeerBase peer)
        {
            OperationResponse response;

            var request = new AccountRegistrationRequest(peer.Protocol, operationRequest);
            if (!OperationHelper.ValidateOperation(request, _log, out response))
            {
                return response;
            }

            var players = _playerRepository.SearchFor(t => t.Login == request.LoginReg);

            if (players.SingleOrDefault() == null)
            {
                var equipmentSet = new List<IEntity>();

                // equip a player
                var armors = MasterApplication.DomainConfiguration.GetAll<BaseArmor>().ToList();
                foreach (var armor in armors)
                {
                    armor.InitializeDefaultArmorParts();
                }

                var elemlist = new List<BaseElement>(MasterApplication.DomainConfiguration.Elements);
                foreach (var element in elemlist)
                {
                    element.Quantity = 100;
                }

                equipmentSet.AddRange(elemlist);
                equipmentSet.AddRange(armors);
                equipmentSet.AddRange(MasterApplication.DomainConfiguration.GetAll<BaseWeapon>());
                equipmentSet.AddRange(MasterApplication.DomainConfiguration.GetAll<BaseModule>());

                var newPlayer = PlayerFactory.CreateDefaultPlayerAccount(
                    request.LoginReg,
                    request.Md5Password,
                    request.UserFirstName,
                    request.UserLastName,
                    request.Email,
                    equipmentSet);

                _playerRepository.Create(newPlayer);

                response =
                    new OperationResponse(operationRequest.OperationCode)
                        {
                            ReturnCode = (short)ErrorCode.Ok,
                            DebugMessage = "Registration done"
                        };
            }
            else
            {
                response =
                    new OperationResponse(operationRequest.OperationCode)
                        {
                            ReturnCode =
                                (short)ErrorCode.OperationFailed,
                            DebugMessage =
                                "Registration failed. Already created."
                        };
            }

            return response;
        }
    }
}