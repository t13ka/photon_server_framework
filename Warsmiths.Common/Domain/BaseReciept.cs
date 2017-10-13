using System.Collections.Generic;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Craft;
using Warsmiths.Common.Domain.Craft.Spells.SavedClasses;
using Warsmiths.Common.Domain.Equipment.Armors;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.Common.Domain.Craft.Quest;

namespace Warsmiths.Common.Domain
{
    public class BaseReciept : BaseItem
    {
        public string Prefix;
        public int Charges ;
        public CraftSpellSaved[] RecieptSpells;
        public List<TriggerTypes> Triggers = new List<TriggerTypes>();
        public RaretyTypes Rarity ;
        public ReceptTypes ReceptType ;
        public List<StatTypes> ReceptKeys ;
        public List<BaseQuestStage> StageList ;
        public RaceTypes Race ;
        public List<StatTypes> ShowingStats;
        public byte SizeX ;
        public byte SizeY ;
        public ArmorTypes ArmorType ;
        public RecieptStatus Status ;
        public List<ArmorPart> PartsPosition ;

        public List<string> ElementsList ;
        public List<string> Perks ;
        //// stats
        public int Steps ;
        public float Durability ;
        public int Red ;
        public int Blue ;
        public int Yellow ;
        public int LatticeColor ;
        public int LatticeForm ;
        public int Casing ;
        public int Slots ;
        public int Weight ;
        public List<ElementQuantity> ElementsQuantity;
        public int Anomality ;
    }
}
