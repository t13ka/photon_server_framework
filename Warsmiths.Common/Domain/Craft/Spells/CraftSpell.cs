using System;
using Warsmiths.Common.Domain.Craft.Grid;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Craft.Spells
{
    public abstract class CraftSpell
    {
        public int SpellPriority;
        public CraftSpellTypes SpellType;

        public Action OnExecute;
        public Action OnDestroy;

        public Common.Position[] OccupideCell;
        public GridController[,] Grid;

        protected CraftSpell(Common.Position[] occupideCell, GridController[,] grid)
        {
            Grid = grid;
            OccupideCell = occupideCell;
        }

        public abstract void Refresh();

        public abstract void Destroy();

        public abstract void Execute();
        public abstract void Execute(ElementController element);

        protected abstract void Init();
    }
}
