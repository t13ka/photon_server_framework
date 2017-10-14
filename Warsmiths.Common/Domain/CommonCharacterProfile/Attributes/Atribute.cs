namespace YourGame.Common.Domain.CommonCharacterProfile.Attributes
{
    using YourGame.Common.Domain.Enums;

    public class Atribute<T>
    {
        public string Name;
        public string Description;
        public SkillGroupTypes Type;
        public T Value;
        public float BpPercent;
    }
}
