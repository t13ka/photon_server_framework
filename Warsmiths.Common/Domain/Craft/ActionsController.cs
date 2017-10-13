using System;
using Warsmiths.Common.Domain.Craft.Grid;

namespace Warsmiths.Common.Domain.Craft
{
    public static class ActionsController
    {

        public static Action<ElementController> OnThreeLine;
        public static Action<ElementController> OnCreateElement;
        public static  Action<ElementController> OnMoveElement;
    }
}
