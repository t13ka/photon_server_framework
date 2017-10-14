namespace YourGame.Common.Domain.Craft.Grid
{
    using System;
    using System.Collections.Generic;

    using YourGame.Common.Domain.Craft.Spells;

    public class GridController : BaseController
    {
        public BaseController Cell;

        public BaseGrid Grid;

        public List<CraftSpell> SpellOn;

        [NonSerialized]
        public Action<BaseController> OnMoveOn;

        [NonSerialized]
        public Action OnClear;

        [NonSerialized]
        public Action<BaseController> OnUse;
    }
}