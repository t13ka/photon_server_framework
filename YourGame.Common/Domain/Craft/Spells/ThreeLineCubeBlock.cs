
// ReSharper disable DelegateSubtraction


namespace YourGame.Common.Domain.Craft.Spells
{
    using YourGame.Common.Domain.Craft.Grid;
    using YourGame.Common.Domain.Enums;

    public class ThreeLineCubeBlock : CraftSpell
    {
        public bool Died;
        public Common.Position[] TriggerPosition;
        public ThreeLineCubeBlock(Common.Position[] occupideCell, GridController[,] grid) : base(occupideCell, grid)
        {
            SpellPriority = 2;
            OccupideCell = occupideCell;
            Grid = grid;
            SpellType = CraftSpellTypes.ThreeLineCube;
            var maincell = occupideCell[0];
    
            if (!grid[maincell.X, maincell.Y].Grid.Status.Contains(GridStatusTypes.Block)) grid[maincell.X, maincell.Y].Grid.Status.Add(GridStatusTypes.Block);
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
           // Grid[OccupideCell[0].X, OccupideCell[0].Y].Grid.Status.Remove(GridStatusTypes.Block);
            ActionsController.OnThreeLine -= Execute;
            OnDestroy?.Invoke();
        }
        public override void Refresh()
        {
            ActionsController.OnThreeLine -= Execute;
            ActionsController.OnThreeLine += Execute;
            Died = false;
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
