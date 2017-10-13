using System;

namespace Warsmiths.Common.Domain.Elements
{
    public class ElementOrderItemResult
    {
        public string ElementId ;

        /// <summary>
        /// Количество уже сгенерированных элементов
        /// </summary>
        public int Generated ;

        public double SecondsToFinishOrder ;
        public DateTime CreatedDateTime ;
        public int OrderedQuantity ;
        public DateTime PlannedCompletionDateTime ;
        public TimeSpan TimeSpanToFinish ;

        public override string ToString()
        {
            return
                string.Format(
                    "Generated:{0}, SecondsToFinishOrder:{1}, CreatedDateTime:{2}, OrderedQuantity:{3}, PlannedCompletionDateTime:{4}",
                    Generated, SecondsToFinishOrder, CreatedDateTime, OrderedQuantity, PlannedCompletionDateTime
                    );
        }
    }
}