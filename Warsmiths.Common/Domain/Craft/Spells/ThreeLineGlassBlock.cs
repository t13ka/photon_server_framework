using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Warsmiths.Common.Domain.Craft.Grid;

namespace Warsmiths.Common.Domain.Craft.Spells
{
    public class ThreeLineGlassBlock : CraftSpell
    {
        public bool Died;
        public Common.Position[] TriggerPosition;
        public ThreeLineGlassBlock(Common.Position[] occupideCell, GridController[,] grid = null) : base(occupideCell, grid)
        {
            SpellPriority = 1;
            OccupideCell = occupideCell;
            Grid = grid;
            SpellType = Enums.CraftSpellTypes.ThreeLineGlassBlock;
            var maincell = occupideCell[0];
            if (Grid != null) Grid[maincell.X, maincell.Y].Grid.Status.Add(Enums.GridStatusTypes.Block);
            TriggerPosition = new[]
            {
               new Common.Position {X = (byte)(maincell.X + 1), Y = maincell.Y},
               new Common.Position {X = (byte)(maincell.X - 1), Y = maincell.Y},
               new Common.Position {X = maincell.X, Y = (byte)(maincell.Y - 1)},
               new Common.Position {X = maincell.X, Y = (byte)(maincell.Y + 1)}
            };

                ActionsController.OnThreeLine += Execute;
            
        }

        public override void Destroy()
        {
            if(Grid!=null) Grid[OccupideCell[0].X, OccupideCell[0].Y].Grid.Status.Remove(Enums.GridStatusTypes.Block);
            ActionsController.OnThreeLine -= Execute;
            OnDestroy?.Invoke();
        }
        public override void Refresh()
        {
            ActionsController.OnThreeLine -= Execute;
            ActionsController.OnThreeLine += Execute;
        }

        public override void Execute()
        {
            if (Died) return; // TODO : FAICKACAICK
            Died = true;
            Destroy();

        }

        public override void Execute(ElementController element)
        {
            foreach (var trigcell in TriggerPosition)
            {
                if (element.Grid.Grid.X == trigcell.X && element.Grid.Grid.Y == trigcell.Y)
                {
                    Execute();
                    return;
                }
            }
        }

        protected override void Init()
        {

        }
    }
}
