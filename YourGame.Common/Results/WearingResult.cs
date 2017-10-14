namespace YourGame.Common.Results
{
    using System.Collections.Generic;

    using YourGame.Common.Domain.Equipment;

    public class WearingResult
    {
        public bool Success ;

        public List<BaseEquipment> TakeOffEquipments ;

        public WearingResult()
        {
            TakeOffEquipments = new List<BaseEquipment>();
        }
    }
}