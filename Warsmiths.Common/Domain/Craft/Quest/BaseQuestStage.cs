using System.Collections.Generic;
using Warsmiths.Common.Domain.Craft.Grid;
using Warsmiths.Common.Domain.Craft.Spells.SavedClasses;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Craft.Quest
{
    public class BaseQuestStage 
    {
        public QuestStageTypes StageStatus;
        public List<BaseGrid> GridList;
        public Element[,] Elements;
        public List<BaseChip> Chips;
        public List<Mark> Marks;
        public List<Common.BorderPosition> BorderList;
        public List<CraftSpellSaved> Spells;

        public BaseQuestStage()
        {
            Elements = new Element[16, 9];
            GridList = new List<BaseGrid>();
            Marks = new List<Mark>();
            BorderList = new List<Common.BorderPosition>();
            Spells = new List<CraftSpellSaved>();
            Chips = new List<BaseChip>();
        }
    }
}