namespace YourGame.Common.Domain.VictoryPrizes
{
    using System.Collections.Generic;

    using YourGame.Common.Domain.Enums.ItemGeneration;

    public enum VictoryPrizeTypeE
    {
        Equipment = 0,
        NoneEqupment = 1,
        Keys = 2,
        Crystals = 3
    }
    public class VictoryPrize
    {
        public int PrizeId;
        public VictoryPrizeTypeE Type;
        public IEntity Item;
        public int CrystalAmout;
        public int KeysAmout;

        public VictoryPrize()
        {
            Type = VictoryPrizeTypeE.Equipment;
            Item = null;
            CrystalAmout = 0;
            KeysAmout = 0;
        }
    }

    public class VictoryPrizesResult
    {
        public ItemGenerationMasteryTypes Mastery;
        public ItemGenerationLuckTypes Luck;
        public ItemGenerationMutualAidTypes MutaulAid;
        public int FarmedCombatPotential;

        public int GoldCoinsBonus;
        public int BlackCoinsBonus;

        public List<VictoryPrize> Items;

        public VictoryPrizesResult()
        {
            Items = new List<VictoryPrize>();
        }
    }
}