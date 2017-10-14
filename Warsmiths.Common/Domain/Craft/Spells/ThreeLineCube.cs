namespace YourGame.Common.Domain.Craft.Spells
{
    using System.Linq;

    using YourGame.Common.Domain.Craft.Grid;
    using YourGame.Common.Domain.Enums;

    public class ThreeLineCube : CraftSpell
    {
        public bool Died;
        public string CubeReward;
        public Common.Position[] TriggerPosition;
        public ThreeLineCube(Common.Position[] occupideCell, GridController[,] grid = null) : base(occupideCell, grid)
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
            foreach(var a in TriggerPosition)
            {
                ActionsController.OnThreeLine += Execute;
            }
        }
        public override void Refresh()
        {

        }

        public override void Destroy()
        {
          //  Grid[OccupideCell[0].X, OccupideCell[0].Y].Grid.Status.Remove(GridStatusTypes.Block);
            ActionsController.OnThreeLine -= Execute;
            OnDestroy?.Invoke();
        }

        public override void Execute()
        {
            if (Died) return; // TODO : FAICKACAICK
            var mbase = new DomainConfiguration(true).Elements.OrderByDescending(x => x.Anomality).ToList();
           
            //var existElem = Reciept.Elements.Find(x => x.Name == mbase[0].Name);
            ///if (existElem != null) existElem.Quantity += 5;
           // else { Reciept.Elements.Add(mbase[0]);  mbase[0].Quantity = 5; };

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
