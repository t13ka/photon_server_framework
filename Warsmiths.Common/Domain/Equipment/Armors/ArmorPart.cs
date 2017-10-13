using System;
using System.Collections.Generic;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Equipment.Armors
{
    public class ArmorPart
    {
        #region Props

        /// <summary>
        /// Сколько максимум можно вставить модулей в эту часть брони
        /// </summary>
        public int MaxModulesCount ;

        public ArmorPartTypes ArmorPartType ;

        public int Durability ;

        public int Casing ;

        /// <summary>
        /// Модули которые вставлены!
        /// </summary>
        public List<BaseModule> Modules ;

        public List<Craft.Common.Position> Position ;
        #endregion

        #region Ctor

        public ArmorPart()
        {
            Modules = new List<BaseModule>();
            Position = new List<Craft.Common.Position>();
        }

        public ArmorPart(ArmorPartTypes type, List<Craft.Common.Position> pos = null)
        {
            Modules = new List<BaseModule>();
            ArmorPartType = type;
            Position = pos;
        }
        #endregion

        #region Methods
        public float GetPartOfValueByType(float value)
        {
            float result;
            switch (ArmorPartType)
            {
                case ArmorPartTypes.Back:
                case ArmorPartTypes.Chest:
                    result = value * 0.25f;
                    break;

                case ArmorPartTypes.RightHand:
                case ArmorPartTypes.LeftHand:
                    result = value * 0.10f;
                    break;

                case ArmorPartTypes.LeftLeg:
                case ArmorPartTypes.RightLeg:
                    result = value * 0.15f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }
        #endregion
    }
}
