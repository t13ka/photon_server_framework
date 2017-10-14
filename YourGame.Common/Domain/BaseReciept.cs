namespace YourGame.Common.Domain
{
    using System.Collections.Generic;

    using YourGame.Common.Domain.Craft;
    using YourGame.Common.Domain.Craft.Quest;
    using YourGame.Common.Domain.Craft.Spells.SavedClasses;
    using YourGame.Common.Domain.Enums;
    using YourGame.Common.Domain.Equipment;
    using YourGame.Common.Domain.Equipment.Armors;

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
