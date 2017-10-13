using System.Collections.Generic;

using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Craft.Grid
{
    public class BaseGrid : BaseCell
    {
        public List<GridStatusTypes> Status = new List<GridStatusTypes>();

        public List<GridCharacterTypes> Character = new List<GridCharacterTypes>();
    }
}