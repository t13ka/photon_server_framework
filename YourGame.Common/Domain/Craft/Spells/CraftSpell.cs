namespace YourGame.Common.Domain.Craft.Spells
{
    using System;

    using YourGame.Common.Domain.Craft.Grid;
    using YourGame.Common.Domain.Enums;

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
