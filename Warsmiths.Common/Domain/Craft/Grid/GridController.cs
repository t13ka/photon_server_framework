using System;
using System.Collections.Generic;

using Warsmiths.Common.Domain.Craft.Spells;

namespace Warsmiths.Common.Domain.Craft.Grid
{
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