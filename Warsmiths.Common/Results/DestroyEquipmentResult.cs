namespace YourGame.Common.Results
{
    using System.Collections.Generic;

    using YourGame.Common.Domain;
    using YourGame.Common.Domain.Elements;

    public class DestroyEquipmentResult
    {
        public BaseElement Element ;

        public List<DropElementItemResult> DropElementItemResult ;

        public override string ToString()
        {
            return string.Format("Element name:{0}, quantity:{1}", Element?.GetType().Name, Element.Quantity);
        }
    }
}
