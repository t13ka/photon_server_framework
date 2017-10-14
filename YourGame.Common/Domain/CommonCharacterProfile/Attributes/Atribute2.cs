namespace YourGame.Common.Domain.CommonCharacterProfile.Attributes
{
    using YourGame.Common.Domain.Enums;

    public class Atribute2<T>
    {
        public string Name;
        public string Description;
        public SkillGroupTypes Type;

        /// <summary>
        /// Right hand
        /// </summary>
        public T Value1;

        /// <summary>
        /// Left hand
        /// </summary>
        public T Value2;

        /// <summary>
        /// отображать ли вторую вещь
        /// </summary>
        public bool UseSecondValue;

        public float BpPercent;
    }
}
