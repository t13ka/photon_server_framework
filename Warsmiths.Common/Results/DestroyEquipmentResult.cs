using System.Collections.Generic;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Elements;

namespace Warsmiths.Common.Results
{
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
