using System;
using System.Collections.Generic;
using System.Linq;
using Warsmiths.Common.Domain.Craft.Grid;
using Warsmiths.Common.Domain.Craft.Quest;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Craft
{
    public class RecieptsContainer
    {
        public BaseReciept[] Reciepts;
    }



    public class Coordinate
    {
        public byte X;
        public byte Y;
    } 
    public static class Common
    {

        public static List<BaseQuest> CalculateQuestQuene(BaseQuest q, List<BaseReciept> questlist)
        {
            var reclist = new List<BaseQuest>();

            var exit = 100;
            while (true)
            {
                if (exit <= 0)
                {
                    break;
                }
                if (q == null || string.IsNullOrEmpty(q.PrevQuest)) break;

                q = (BaseQuest)questlist.Find(x => x._id == q.PrevQuest);
            }

            while (true)
            {
                if (exit <= 0)
                {
                    break;
                }
                if (q != null && !string.IsNullOrEmpty(q.NextQuest))
                {
                    reclist.Add(q);
                    q = (BaseQuest)questlist.Find(x => x.Name == q.NextQuest);
                }
                else break;
            }
            reclist.Add(q);

            return reclist;
        }

        public static int CalculateQuestExpirience(int stars, int qlvl, int stage)
        {
            if (stage == 0) stage = 1;
            var starExp = stars * 400;
            var qNumbExp = starExp * (1 + Math.Sqrt(qlvl * qlvl * qlvl * qlvl) / 10) *  ( 1 + (stage * 0.1f));

            return (int)qNumbExp;
        }
        public static void ChangeElementColor(BaseController cell, ElementColorTypes col)
        {
            ((ElementController)cell).Elem.ColorType = col;
        }
        public static void ChangeElementColorSimple(this ElementController elem)
        {
            if(elem.Elem.ColorType == ElementColorTypes.Blue || elem.Elem.ColorType == ElementColorTypes.Red || elem.Elem.ColorType == ElementColorTypes.Yellow)
            {
                elem.ChangeColor((ElementColorTypes)new Random((int)DateTime.Now.Ticks).Next(0, 3));
            }
            else
            {
                elem.ChangeColor((ElementColorTypes)new Random((int)DateTime.Now.Ticks).Next(3, 6));
            }
        }
        public static void ChangeElementPower(ElementController element, byte pow, DomainConfiguration config)
        {
            var elemlist = config.Elements; 
            if (element.Elem.Power != pow)
            {
                var primalElem = elemlist.Find(x => x.ColorType == element.Elem.ColorType && x.Type == element.Elem.Type);

                element.ChangeStats(StatTypes.Durability,(int) primalElem.Durability + 5*pow);
                element.ChangeStats(StatTypes.Blue, primalElem.Blue + 5*pow);
                element.ChangeStats(StatTypes.Red, primalElem.Red + 5*pow);
                element.ChangeStats(StatTypes.Yellow, primalElem.Yellow + 5*pow);
                element.Elem.Power = pow;
            }
        }
        public static void DestroyElement(this ElementController el)
        {
            el.Grid.ClearGrid();
            el.OnDestroyElement();
        }

        public static void ClearGrid(this GridController grid)
        {

            ChangeGridStatus(grid, GridStatusTypes.Free);

            if (grid.Cell != null)
            {
                if (grid.Cell is ElementController)
                {
                    grid.Cell = null;
                    grid.OnClear?.Invoke();
                }
                else
                {
                    if (grid.Cell is ChipController)
                    {
                        grid.Cell = null;
                    }
                }
            }
        }


        public static void ChangeGridStatus(GridController grid, GridStatusTypes state)
        {
            
            if (state == GridStatusTypes.Free)
            {
                if(grid.Grid.Status.Contains(GridStatusTypes.UnUse))
                {
                    return;
                }
                if (grid.Grid.Status.Contains(GridStatusTypes.Block))
                {
                    grid.Grid.Status.Remove(GridStatusTypes.Block);
                }
                if (grid.Grid.Status.Contains(GridStatusTypes.Occupied))
                {
                    grid.Grid.Status.Remove(GridStatusTypes.Occupied);
                }
            }
            else if (state == GridStatusTypes.Block || state == GridStatusTypes.Occupied)
            {
                if (grid.Grid.Status.Contains(GridStatusTypes.Free))
                {
                    grid.Grid.Status.Remove(GridStatusTypes.Free);
                }
            }

            if (!grid.Grid.Status.Contains(state))
            {
                grid.Grid.Status.Add(state);
            }
        }

        public static void ChangeElementState(ElementController element, ElementStateTypes state)
        {
            element.Elem.State = state;
        }
        public static void ChangeElementStats(ElementController element, StatTypes stat, int amout)
        {
            switch (stat)
            {
                case StatTypes.Durability:
                    element.Elem.Durability = amout;
                    break;
                case StatTypes.Blue:
                    element.Elem.Blue = amout;
                    break;
                case StatTypes.Red:
                    element.Elem.Red = amout;
                    break;
                case StatTypes.Yellow:
                    element.Elem.Yellow = amout;
                    break;
            }
        }

        public class Position
        {
            public byte X;
            public byte Y;
        }

        public class BorderPosition
        {
            public enum BorderTypesE
            {
                None = 0,
                LeftTop = 1,
                MiddleHorizont = 2,
                MiddleVertical = 3,
                RightTop = 4,
                LeftBottom = 5,
                RightBottom = 6,
                Closed = 7
            }

            public byte X;
            public byte Y;
            public BorderTypesE BorderType;
        }


        public static void MoveToGrid(this BaseController cell, GridController grid)
        {
            if (cell is ElementController)
            {
                if (((ElementController) cell).Grid != null)
                {
                    ((ElementController) cell).Grid.Cell = null;


                    ChangeGridStatus(((ElementController)cell).Grid, GridStatusTypes.Free);
                }

                ((ElementController)cell).Grid = grid;

                grid.Cell = cell;
                ChangeGridStatus(grid, GridStatusTypes.Occupied);
                grid.OnMoveOn?.Invoke(cell);
            }
        }

    }
}
