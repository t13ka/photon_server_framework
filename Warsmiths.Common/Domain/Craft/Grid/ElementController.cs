namespace YourGame.Common.Domain.Craft.Grid
{
    using System;

    using YourGame.Common.Domain.Enums;

    public class ElementController : BaseController
    {
        [NonSerialized]
        public Action<CraftSpellTypes> OnSpellEffected;

        [NonSerialized]
        public Action ReplaceElement;

        [NonSerialized]
        public Action<byte> ChangePowerDel;

        [NonSerialized]
        public Action<StatTypes, int> ChangeStatsDel;

        [NonSerialized]
        public Action<ElementStateTypes> ChangeStateDel;

        [NonSerialized]
        public Action SealElementDel;

        [NonSerialized]
        public Action OnDestroyElement;

        [NonSerialized]
        public Action InitElementDel;

        [NonSerialized]
        public Action<ElementColorTypes> ChangecolorDel;

        public Element Elem;

        [NonSerialized]
        public GridController Grid;

        public void SpellEffected(CraftSpellTypes spellType)
        {
            OnSpellEffected?.Invoke(spellType);
        }

        public void ChangeState(ElementStateTypes state)
        {
            ChangeStateDel?.Invoke(state);
            Common.ChangeElementState(this, state);
        }

        public void ChangeColor(ElementColorTypes col)
        {
            Common.ChangeElementColor(this, col);
            ChangecolorDel?.Invoke(col);
        }

        public void ChangePower(byte pow)
        {
            ChangePowerDel?.Invoke(pow);
            Common.ChangeElementPower(this, pow, new DomainConfiguration()); // TODO CHANGE TO REAL DOMAIN
        }

        public void ChangeStats(StatTypes stat, int amout)
        {
            ChangeStatsDel?.Invoke(stat, amout);
            Common.ChangeElementStats(this, stat, amout);
        }

        public void ChangeElementPosition()
        {
            ReplaceElement?.Invoke();
        }

        public void SealElement()
        {
            SealElementDel?.Invoke();
        }

        public void InitElement()
        {
        }
    }
}