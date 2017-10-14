using System;
using System.Collections.Generic;

namespace YourGame.Server.Services.Economic
{
    public abstract class ElementStatement
    {
        protected TimeSpan CurrentTimeSpan = new TimeSpan(0, 0, 0, 0, 0);
        protected readonly Queue<TimeSpan> Spans = new Queue<TimeSpan>();

        #region Ctor

        protected ElementStatement(string playerId, string elementId, int quantity)
        {
            ElementId = elementId;
            Quantity = quantity;
            PlayerId = playerId;
        }

        #endregion

        #region Properties

        public bool Completed { get; protected set; }

        public double SecondsToFinishOrder { get; protected set; }
        public string PlayerId { get; protected set; }
        public string ElementId { get; protected set; }
        public int Quantity { get; protected set; }
        public int Generated { get; protected set; }
        public DateTime CreatedDateTime { get; protected set; }
        public DateTime PlannedCompletionDateTime { get; protected set; }
        public TimeSpan TimeSpanToFinish ;
        public Guid StatementId ;
        public bool Instantly ;

        #endregion

        public abstract void Update();

        public abstract TransactionProcessingResult Handle(EconomicRuntimeService service, GlobalElementStatement ges);
    }
}