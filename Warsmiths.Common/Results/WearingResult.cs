using System.Collections.Generic;
using Warsmiths.Common.Domain.Equipment;

namespace Warsmiths.Common.Results
{
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