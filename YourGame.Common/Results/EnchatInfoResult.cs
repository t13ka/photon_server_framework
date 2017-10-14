namespace YourGame.Common.Results
{
    public struct EnchatInfoResult
    {
        public float ChanceToBroke;

        public float ElemetsCountForMaxEcnhat;

        public float CurrentAttack;

        public float NewAttack;

        public int CurrentEnchatPercent;

        public int NewEnchatPercent;

        public override string ToString()
        {
            return string.Format(
                "ChanceToBroke:{0}; ElemetsCountForMaxEcnhat:{1}; CurrentAttack:{2}; NewAttack:{3}; CurrentEnchatPercent:{4}; NewEnchatPercent:{5}",
                ChanceToBroke,
                ElemetsCountForMaxEcnhat,
                CurrentAttack,
                NewAttack,
                CurrentEnchatPercent,
                NewEnchatPercent);
        }
    }
}