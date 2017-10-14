namespace YourGame.Common.Domain.CommonCharacterProfile.Attributes
{
    using YourGame.Common.Domain.Enums;

    /// <summary>
    /// ���� ���������� �������� ���������, ������������, ���-�� ������� ������� ������� 
    /// � ArmorPart (1 - ������� �������� ����
    /// 2- ������� ���������)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Atribute4<T>
    {
        public string Name;
        public string Description;
        public SkillGroupTypes Type;
        public T Value1;
        public T Value2;
        public T Value3;
        public T Value4;
        public float Percent;
    }
}
