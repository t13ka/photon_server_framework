namespace YourGame.Common.Domain.Craft.Spells
{
    using System;
    using System.Collections.Generic;

    using YourGame.Common.Domain.Craft.Grid;
    using YourGame.Common.Domain.Enums;

    public class Shreder : CraftSpell
    {
        public Action<ElementController> OnExecuteElement;
        public void MoveOn(BaseController bcon)
        {
            var bconelem = bcon as ElementController;
            if (bconelem != null) Execute(bconelem);
        }
        public Shreder(Common.Position[] occupideCell, GridController[,] grid) : base(occupideCell, grid)
        {
            OccupideCell = occupideCell;
            SpellType = CraftSpellTypes.ThreeLineCube;
            SpellPriority = 5;
            foreach (var cell in occupideCell)
            {
                var cel = grid[cell.X, cell.Y];
                if (cel.SpellOn == null)
                {
                    cel.SpellOn = new List<CraftSpell>();
                }
                cel.Grid.Status.Remove(GridStatusTypes.Free);
                cel.Grid.Status.Add(GridStatusTypes.ApplyMagic);
                cel.SpellOn.Add(this);
                cel.OnUse += MoveOn;
            }
        }

        public override void Execute(ElementController element)
        {
            element.DestroyElement();
            OnExecuteElement?.Invoke(element);
        }

        public override void Refresh()
        {

        }

        public override void Destroy()
        {
            OnDestroy?.Invoke();
        }

        public override void Execute()
        {}

        protected override void Init()
        {
            
        }
    }
}
