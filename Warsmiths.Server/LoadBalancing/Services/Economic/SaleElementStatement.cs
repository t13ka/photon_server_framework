using System;
using System.Linq;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Elements;
using Warsmiths.DatabaseService.Repositories;

namespace Warsmiths.Server.Services.Economic
{
    public class SaleElementStatement : ElementStatement
    {
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public SaleElementStatement(string playerId, string elementId, int quantity) : base(playerId, elementId, quantity)
        {
            Instantly = true;
            StatementId = Guid.NewGuid();
        }

        public override void Update()
        {
            // idle
        }

        public override TransactionProcessingResult Handle(EconomicRuntimeService service, GlobalElementStatement ges)
        {
            TransactionProcessingResult result;
            IEntity element;

            var player = _playerRepository.GetById(PlayerId);

            if (player.TryGetInventoryValue(ElementId, out element) == false)
            {
                return new TransactionProcessingResult
                {
                    Success = false,
                    DebugMessage = $"element with id {ElementId} not in inventory at the moment"
                };
            }

            var inventoryElement = (BaseElement)element;
            if (inventoryElement.Quantity < Quantity)
            {
                result = new TransactionProcessingResult
                {
                    Success = false,
                    DebugMessage = $"element with id {ElementId}. lacks the necessary quantities for sale",
                };
            }
            else
            {
                player.Gold += ges.BuyPrice * Quantity;

                ges.SoldQuantity = ges.SoldQuantity + Quantity;

                inventoryElement.Quantity -= Quantity;

                if (inventoryElement.Quantity <= 0)
                {
                    player.TryRemoveFromInventory(inventoryElement._id);
                }

                if (service.Subscribers.Any())
                {
                    _playerRepository.Update(player);
                }

                result = new TransactionProcessingResult
                {
                    Success = true,
                    DebugMessage = "successful sale",
                };
            }

            return result;
        }
    }
}