using System;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Equipment
{
    public class BaseEquipment : IEntity
    {
        #region Props
        public RaceTypes RaceType ;
        public RaretyTypes Rarety ;
        public EnergyTypes EnergyTypes ;
        public ElementColorTypes ColorType ;
        public float Prok ;
        public float Anomality ;
        public float Weight ;    
        public int Sharpening ;
        public float Price ;
        public string Sprite ;
        public int Chance ;
        #endregion

        #region Methods

        public BaseEquipment()
        {
            Sprite = "";
        }

        public virtual BaseEquipment Clone()
        {
            return null;
        }

        public override string ToString()
        {
            return $"Name: {Name}; Price: {Price}";
        }
        #endregion
    }
}