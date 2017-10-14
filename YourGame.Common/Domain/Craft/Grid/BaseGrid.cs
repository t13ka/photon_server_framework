namespace YourGame.Common.Domain.Craft.Grid
{
    using System.Collections.Generic;

    using YourGame.Common.Domain.Enums;

    public class BaseGrid : BaseCell
    {
        public List<GridStatusTypes> Status = new List<GridStatusTypes>();

        public List<GridCharacterTypes> Character = new List<GridCharacterTypes>();
    }
}