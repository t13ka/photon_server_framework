using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.Common.Domain.Craft.Grid
{
    public class Element : BaseCell
    {
        public string ID;

        public bool Interactable;

        public ElementColorTypes ColorType;

        public ElementTypes Type;

        public ElementStateTypes State;

        public float Durability;

        public int Blue;

        public int Red;

        public int Yellow;

        public byte Power;

        public int Weight;

        public int Anamality;

        public Element(bool interactable = true)
        {
            Interactable = true;
        }
    }
}