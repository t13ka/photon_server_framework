namespace YourGame.Common.Domain
{
    using YourGame.Common.Domain.Enums;

    public class BaseSkill : IEntity
    {
        #region Props
        public int Priority ;
        public int BpPercent ;
        public float Val ;
        public SkillGroupTypes SkillGroupType ;
        public SkillSection SkillSection ;
        #endregion

        #region Ctors

        public BaseSkill()
        {
        }

        public BaseSkill(float d)
        {
            Val = d;
        }

        #endregion

        #region Methods
        public void Set(float val)
        {
            Val = val;
        }

        public static implicit operator double(BaseSkill d)
        {
            return d.Val;
        }

        public static implicit operator BaseSkill(float d)
        {
            return new BaseSkill(d);
        }

        public override string ToString()
        {
            return this?.GetType().Name + ",Value:" + Val;
        }
        #endregion
    }
}