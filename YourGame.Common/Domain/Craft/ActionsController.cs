namespace YourGame.Common.Domain.Craft
{
    using System;

    using YourGame.Common.Domain.Craft.Grid;

    public static class ActionsController
    {

        public static Action<ElementController> OnThreeLine;
        public static Action<ElementController> OnCreateElement;
        public static  Action<ElementController> OnMoveElement;
    }
}
