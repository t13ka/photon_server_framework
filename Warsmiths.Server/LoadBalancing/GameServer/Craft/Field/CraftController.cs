using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using ExitGames.Logging;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Craft;
using Warsmiths.Common.Domain.Craft.Grid;
using Warsmiths.Common.Domain.Craft.SharedClasses;
using Warsmiths.Common.Domain.Craft.Spells;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Server.GameServer.Craft.Common;
using Warsmiths.Server.MasterServer;

namespace Warsmiths.Server.GameServer.Craft.Field
{
    public class CraftController
    {
        public readonly ILogger _log = LogManager.GetCurrentClassLogger();
        public Action OnBeforeRecalculate;
        public Action OnAfterRecalculate;

        public Player CraftOwner;
        public CraftState CurrentState;
        public BaseReciept Reciept;
        public GridController[,] CellList;
        public ElementController[,] ElementsList;
        public List<CraftSpell> SpellList;
        public CraftResoult Resoult;
        public int CurrentStage;



        public Dictionary<StatTypes, int> Stats = new Dictionary<StatTypes, int>
        {
            {StatTypes.ThreeLine, 0},
            {StatTypes.Durability, 0},
            {StatTypes.Blue, 0},
            {StatTypes.Red, 0},
            {StatTypes.Yellow, 0},
            {StatTypes.Weight, 0},
            {StatTypes.Anomality, 0},
            {StatTypes.Endurance, 0},
            {StatTypes.Lattice, 0},
            {StatTypes.Casin, 0},
            {StatTypes.CraftStar, 0},
            {StatTypes.Price, 0}
        };

        public CraftController(BaseReciept rec, Player owner)
        {
            Reciept = rec;
            CraftOwner = owner;
        }

        public void Init()
        {
            CellList = new GridController[16, 9];
            ElementsList = new ElementController[16, 9];

            for (byte i = 0; i < 16; i++)
            {
                for (byte j = 0; j < 9; j++)
                {
                    CellList[i, j] = new GridController
                    {
                        Grid = new BaseGrid
                        {
                            X = i,
                            Y = j
                        }
                    };
                }
            }
            CurrentState = CraftState.InProgress;

        }

        public void InitField()
        {
            foreach (var gridController in CellList)
            {

                var sta = Reciept.StageList[CurrentStage]
                    .GridList.Find(x => x.X == gridController.Grid.X && x.Y == gridController.Grid.Y);

                var oc = gridController.Grid.Status.Contains(GridStatusTypes.Occupied);
                gridController.Grid.Character = sta.Character;
                gridController.Grid.Status = sta.Status;
                if (oc && gridController.Cell != null) gridController.Grid.Status.Add(GridStatusTypes.Occupied);
            }

            for (var i = 0; i < 16; i++)
            {
                for (var j = 0; j < 9; j++)
                {
                    if (Reciept.StageList[CurrentStage].Elements[i, j] == null)
                    {
                        var elem = CellList[i, j].Cell as ElementController;
                        elem?.DestroyElement();
                        CellList[i, j].Cell = null;
                    }
                }
            }

            for (var i = 0; i < 16; i++)
            {
                for (var j = 0; j < 9; j++)
                {
                    if (Reciept.StageList[CurrentStage].Elements[i, j] == null || CellList[i, j].Cell != null) continue;
                    this.CreateElement(CellList[i, j], Reciept.StageList[CurrentStage].Elements[i, j].ID, 0);
                }
            }

            foreach (var chip in Reciept.StageList[CurrentStage].Chips)
            {
                this.GraveModule(CellList[chip.X, chip.Y]);
            }


            // create spells
            ClearSpells();
            if (Reciept.StageList[CurrentStage].Spells != null)
                foreach (var a in Reciept.StageList[CurrentStage].Spells)
                {
                    this.CreateSpell(CellList[a.OccupideCell[0].X, a.OccupideCell[0].Y], a.SpellType);
                }
        }


        public void ClearSpells()
        {
            SpellList.Clear();
        }

        public void ClearField()
        {
            CellList = null;
            ElementsList = null;
        }

        public void Start()
        {

        }

        public virtual CraftResoult End()
        {
            return new CraftResoult();
        }

        public virtual void Recalculate()
        {

            //  if (Reciept.Perks.Find(x => x == "BoostsSlots") != null)
            //


            // if (Recept.Perks.Find(x => x == "Chronograph") != null)
            //    ThreeLine();

            Stats[StatTypes.Durability] = 0;
            Stats[StatTypes.Blue] = 0;
            Stats[StatTypes.Red] = 0;
            Stats[StatTypes.Yellow] = 0;
            Stats[StatTypes.Anomality] = 0;
            Stats[StatTypes.Weight] = 0;
            Stats[StatTypes.Casin] = 0;


            foreach (var tempCell in CellList)
            {
                var cell = tempCell.Cell as ElementController;
                if (cell == null || cell.Grid.Grid.Status.Contains(GridStatusTypes.Block)) continue;

                var el = cell;

                Reciept.Weight += (int) CraftApplication.GetPrimalElement(el.Elem.ID).Weight + el.Elem.Weight;
                Reciept.Anomality += (int) CraftApplication.GetPrimalElement(el.Elem.ID).Anomality + el.Elem.Anamality;
                Reciept.Durability += CraftApplication.GetPrimalElement(el.Elem.ID).Durability + el.Elem.Durability;
                Reciept.Blue += CraftApplication.GetPrimalElement(el.Elem.ID).Blue + el.Elem.Blue;
                Reciept.Red += CraftApplication.GetPrimalElement(el.Elem.ID).Red + el.Elem.Red;
                Reciept.Yellow += CraftApplication.GetPrimalElement(el.Elem.ID).Yellow + el.Elem.Yellow;
            }
            Reciept.Anomality = Reciept.Anomality / CellList.Cast<GridController>()
                                    .Count(a => !a.Grid.Status.Contains(GridStatusTypes.UnUse));

            /*
            var totalcasin = 0;
            foreach (var sFigure in FiguresList)
            {
                sFigure.Calculate();
                if (sFigure.FigureType == CraftCommon.FigureTypes.Slot)
                {
                    totalcasin += sFigure.TotalDurability * 10;
                }
            }*/
            /*
            Reciept.Slots = FiguresList.FindAll(figure => figure.FigureType == CraftCommon.FigureTypes.Slot).Count;

            StatsList[StatTypes.Casin].SetValue(totalcasin / (Recept.Slots > 0 ? Recept.Slots : 1));
            StatsList[StatTypes.Endurance].SetValue(Recept.Slots);

            PreCraftController.I.Casin = totalcasin / 2;

            StatsList[StatTypes.ThreeLine].statLabel.text = Recept.Steps.ToString();
            StatsList[StatTypes.Weight].statLabel.text = Recept.Weight.ToString();
            StatsList[StatTypes.Anomality].statLabel.text = Recept.Anomality.ToString();
            StatsList[StatTypes.Durability].statLabel.text = ((int)Recept.Durability).ToString();
            StatsList[StatTypes.Blue].statLabel.text = Recept.Blue.ToString();
            StatsList[StatTypes.Red].statLabel.text = Recept.Red.ToString();
            StatsList[StatTypes.Yellow].statLabel.text = Recept.Yellow.ToString();
            StatsList[StatTypes.Endurance].statLabel.text = Recept.Slots.ToString();
            StatsList[StatTypes.Lattice].statLabel.text = (Recept.LatticeColor + Recept.LatticeForm).ToString();
            StatsList[StatTypes.Casin].SetValue((int)StatsList[StatTypes.Casin].Value);

            /////
            if (PreCraftController.I.Goals.Count > 0)
            {

                StatsList[StatTypes.Weight].CoverFirst.fillAmount = (100 * Recept.Weight / PreCraftController.I.Goals[StatTypes.Weight]) * 0.01f;
                StatsList[StatTypes.Durability].CoverFirst.fillAmount = (100 * Recept.Durability / PreCraftController.I.Goals[StatTypes.Durability]) * 0.01f;
                StatsList[StatTypes.Blue].CoverFirst.fillAmount = (100 * Recept.Blue / PreCraftController.I.Goals[StatTypes.Blue]) * 0.01f;
                StatsList[StatTypes.Red].CoverFirst.fillAmount = (100 * Recept.Red / PreCraftController.I.Goals[StatTypes.Red]) * 0.01f;
                StatsList[StatTypes.Yellow].CoverFirst.fillAmount = (100 * Recept.Yellow / PreCraftController.I.Goals[StatTypes.Yellow] * 0.01f);
                //   StatsList[StatTypes.Casin].CoverFirst.fillAmount = (100 * Recept.Casing/PreCraftController.I.Goals[StatTypes.Casin]*0.001f);
                //   StatsList[StatTypes.Lattice].CoverFirst.fillAmount = (100 * (Recept.LatticeColor + Recept.LatticeForm)/100*PreCraftController.I.Goals[StatTypes.Endurance])*0.001f;
            }
            ///
            if (StatsList[StatTypes.Weight].CoverFirst.fillAmount == 1 && !Recept.ReceptKeys.Contains(StatTypes.Weight))
            {
                StatsList[StatTypes.Weight].ActivateKey();
                Recept.ReceptKeys.Add(StatTypes.Weight);
            }
            if (StatsList[StatTypes.Durability].CoverFirst.fillAmount == 1 && !Recept.ReceptKeys.Contains(StatTypes.Durability))
            {
                Recept.ReceptKeys.Add(StatTypes.Durability);
                StatsList[StatTypes.Durability].ActivateKey();
            }
            if (StatsList[StatTypes.Blue].CoverFirst.fillAmount == 1 && !Recept.ReceptKeys.Contains(StatTypes.Blue))
            {
                Recept.ReceptKeys.Add(StatTypes.Blue);
                StatsList[StatTypes.Blue].ActivateKey();
            }
            if (StatsList[StatTypes.Red].CoverFirst.fillAmount == 1 && !Recept.ReceptKeys.Contains(StatTypes.Red))
            {
                Recept.ReceptKeys.Add(StatTypes.Red);
                StatsList[StatTypes.Red].ActivateKey();
            }
            if (StatsList[StatTypes.Yellow].CoverFirst.fillAmount == 1 && !Recept.ReceptKeys.Contains(StatTypes.Yellow))
            {
                Recept.ReceptKeys.Add(StatTypes.Red);
                StatsList[StatTypes.Yellow].ActivateKey();
            }
            if (StatsList[StatTypes.Casin].CoverFirst.fillAmount == 1 && !Recept.ReceptKeys.Contains(StatTypes.Casin))
            {
                Recept.ReceptKeys.Add(StatTypes.Red);
                StatsList[StatTypes.Casin].ActivateKey();
            }
            if (StatsList[StatTypes.Lattice].CoverFirst.fillAmount == 1 && !Recept.ReceptKeys.Contains(StatTypes.Lattice))
            {
                Recept.ReceptKeys.Add(StatTypes.Red);
                StatsList[StatTypes.Lattice].ActivateKey();
            }


            // calculate power




            var good = CalculateTotalPower(ElementStateTypes.GoodAwake);
            var evil = CalculateTotalPower(ElementStateTypes.EvilAwake);
            var total = good + evil;


            if (good > evil)
            {
                if (GoodPowerS > EvilPowerS)
                {
                    AudioManager.I.PlayTheme("Craft/GoodPower");
                }
            }
            else if (evil > good)
            {
                if (EvilPowerS > GoodPowerS)
                {
                    AudioManager.I.PlayTheme("Craft/EvilPower");
                }
            }
            else
            {
                if ((int)EvilPowerS != (int)GoodPowerS)
                {
                    AudioManager.I.PlayTheme("Craft/NeutralPower");
                }
            }


            if (good != 0 || evil != 0)
            {

                BluePowerInf.GetComponent<MediaTypeNames.Text>().text = good.ToString();
                RedPowerInf.GetComponent<MediaTypeNames.Text>().text = evil.ToString();

                // calculate power
                if (total != 0)
                {
                    GoodPower.fillAmount = good * 100f / total * 0.01f;
                    EvilPower.fillAmount = evil * 100f / total * 0.01f;

                }
                else
                {
                    GoodPower.fillAmount = 0.5f;
                    EvilPower.fillAmount = 0.5f;
                }
            }
            else
            {
                BluePowerInf.GetComponent<MediaTypeNames.Text>().text = "";
                RedPowerInf.GetComponent<MediaTypeNames.Text>().text = "";
            }

            GoodPowerS = good;
            EvilPowerS = evil;





            if (OnRecalculate != null) OnRecalculate();

            if (CraftQuestController.I != null && CraftQuestController.I.Lopata > 5) { CraftQuestController.I.CheckTyps(); }
            // if(FiguresList.Find(x=>x.IsChoose) == null)
            if (CurrentGameStatus != GameStatus.IsFinished && s_BubblePanel.I.FinishPercent <= 0)
                if (CraftQuestController.I == null)
                {
                    CurrentGameStatus = GameStatus.IsFinished;
                    CraftResoult(true);
                }
        }*/
        }
    }
}
