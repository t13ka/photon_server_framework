using System;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Equipment;

namespace Warsmiths.Common.Domain
{
    public class BaseElement : BaseItem
    {
        #region Props
        public float Weight ;
        public float Anomality ;
        public int Quantity ;
        public float Durability ;
        public ElementCategoryTypes CategoryType ;
        public PropertyTypes Property;
        public TriggerTypes Trigger;
        
        public int BasePrice ;
        public int MaxPrice
        {
            get
            {
                var delta = Price*2f;
                return Price + (int)delta;
            }
        }

        public float MinPrice
        {
            get
            {
                var delta = Price * 0.2f;
                return delta;
            }
        }

        public ElementStateTypes ElementStateType ;

        public byte Power ;

        public string AnamalityType;  

        public int DamageZone ;

        public string AnomalityType ;
        //
        public int Blue ;

        public int Red ;

        public int Yellow ;

        public ElementColorTypes ColorType ;
        public ElementTypes Type ;
        #endregion 

        #region Ctors

        public BaseElement()
        {
            _id = Guid.NewGuid().ToString();//string.Format("element_{0}", GetType().Name);
        }

        public void SetName()
        {
            Name = $"{Type}{ColorType}";
            Sprite = Name;
        }
        #endregion
    }
}