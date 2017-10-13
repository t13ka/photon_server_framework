using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.CommonCharacterProfile.Attributes
{
    public class Atribute<T>
    {
        public string Name;
        public string Description;
        public SkillGroupTypes Type;
        public T Value;
        public float BpPercent;
    }
}
