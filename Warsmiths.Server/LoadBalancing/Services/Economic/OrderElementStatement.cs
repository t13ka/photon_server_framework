using System;
using System.Linq;
using ExitGames.Logging;

using Warsmiths.Server.MasterServer;

namespace Warsmiths.Server.Services.Economic
{
    using YourGame.DatabaseService.Repositories;

    public class OrderElementStatement : ElementStatement
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly PlayerRepository _playerRepository = new PlayerRepository();

        public OrderElementStatement(string playerId, string elementId, int quantity, bool instantly = false)
            : base(playerId, elementId, quantity)
        {
            Instantly = instantly;

            // calculate time to finish order
            for (var i = 0; i < quantity; i++)
            {
                var val = MasterApplication.DomainConfiguration.ElementsSpawnPoints[i];
                var span = new TimeSpan(0, 0, 0, val);
                Spans.Enqueue(span);
            }
            SecondsToFinishOrder = Spans.Sum(t => t.Seconds);
            //SecondsToFinishOrder = 5;

            TimeSpanToFinish = new TimeSpan(0, 0, 0, (int) SecondsToFinishOrder);
            CreatedDateTime = DateTime.UtcNow;
            PlannedCompletionDateTime = CreatedDateTime.AddSeconds(SecondsToFinishOrder);
            PlayerId = playerId;
            ElementId = elementId;
            Quantity = quantity;
            if (string.IsNullOrEmpty(playerId))
            {
                throw new NullReferenceException("player id can't be nullable");
            }
            if (string.IsNullOrEmpty(elementId))
            {
                throw new NullReferenceException("elementId can't be nullable");
            }

            Completed = false;
            StatementId = Guid.NewGuid();
        }

        public override TransactionProcessingResult Handle(EconomicRuntimeService service, GlobalElementStatement ges)
        {
            TransactionProcessingResult result;

            if (Quantity > MasterApplication.DomainConfiguration.MaxElementsAmountForPlayer)
            {
                return new TransactionProcessingResult
                {
                    Success = false,
                    DebugMessage =
                        $"quantity is greater than max amount for player (default:{MasterApplication.DomainConfiguration.MaxElementsAmountForPlayer})"
                };
            }

            var maxForCurrentPlayer = service.ElementStatements
                .Where(t => t.PlayerId == PlayerId)
                .Sum(t => t.Generated);

            if (maxForCurrentPlayer >= MasterApplication.DomainConfiguration.MaxElementsAmountForPlayer)
            {
                return new TransactionProcessingResult
                {
                    Success = false,
                    DebugMessage = "ordered quantity is greater than max amount for player"
                };
            }

            // отключено по просьбе Павла и Димы (01.02.2017)
            // check if element already ordered
            //if (service.ElementStatements.FirstOrDefault(t => t.ElementId == ElementId
            //                                                  && t.PlayerId == PlayerId) != null)
            //{
            //    return new TransactionProcessingResult
            //    {
            //        Success = false,
            //        DebugMessage =
            //            $"ordering the element with the ID: {ElementId} is forbidden! because he had already ordered and the order has not yet been made!",
            //    };
            //}

            var player = _playerRepository.GetById(PlayerId);

            if (player.Gold >= ges.OrderPrice * Quantity)
            {
                player.Gold -= ges.OrderPrice * Quantity;

                // count total ordered elements
                ges.OrderedQuantity = ges.OrderedQuantity + Quantity;

                if (Instantly == false)
                {
                    if (service.Subscribers.Any())
                    {
                        _playerRepository.Update(player);
                    }

                    result = new TransactionProcessingResult
                    {
                        Success = true,
                        DebugMessage = "successful order",
                    };
                }
                else
                {
                    Update();
                    result = new TransactionProcessingResult
                    {
                        Success = true,
                        DebugMessage = "successful order",
                    };
                }
            }
            else
            {
                result = new TransactionProcessingResult
                {
                    Success = false,
                    DebugMessage = "order fail. not enough Gold",
                };
            }

            return result;
        }

        public override void Update()
        {
            if (Instantly)
            {
                Generated = Quantity;
                Completed = true;
            }
            else
            {
                var debugmessage = string.Empty;
                if (Completed == false)
                {
                    CurrentTimeSpan = CurrentTimeSpan.Add(EconomicRuntimeService.RefreshTimeSpan);
                    SecondsToFinishOrder--;
                    TimeSpanToFinish = TimeSpanToFinish.Subtract(EconomicRuntimeService.RefreshTimeSpan);

                    debugmessage =
                        $"SatementID:{StatementId}; Element:{ElementId}; player Id:{PlayerId}; " +
                        $"will be completed after:{TimeSpanToFinish};";

                    if (SecondsToFinishOrder <= 0)
                    {
                        Generated = Quantity;
                        Completed = true;

                        if (Log.IsDebugEnabled)
                        {
                            Log.Debug($"OrderElementStatement with ID='{StatementId}' has been completed!");
                        }
                    }
                }

                if (Log.IsDebugEnabled)
                {
                    if (string.IsNullOrEmpty(debugmessage) == false)
                    {
                        Log.Debug(debugmessage);
                    }
                }
            }
        }
    }
}