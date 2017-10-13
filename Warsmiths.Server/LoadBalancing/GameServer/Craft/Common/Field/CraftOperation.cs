using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Craft;
using Warsmiths.Common.Domain.Craft.Grid;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Server.GameServer.Craft.Field;

namespace Warsmiths.Server.GameServer.Craft.Common
{
    public static class CraftOperation
    {
        public static void DestroyElement(this CraftController controller, ElementController elem)
        {
            elem.DestroyElement();
            controller.ElementsList[elem.Elem.X, elem.Elem.Y] = null;
            controller.CellList[elem.Elem.X, elem.Elem.Y].Cell = null;
        }

        public static ElementController CreateElement(this CraftController controller, GridController cel, string elemId, byte power)
        {
            var primalElement = new DomainConfiguration(true).Elements.Find(x => x.Name == elemId);
            var elem = new ElementController
            {
                Elem = new Element
                {
                    Type = primalElement.Type,
                    ColorType = primalElement.ColorType,
                    ID = primalElement.Name,
                    X = cel.Grid.X,
                    Y = cel.Grid.Y
                }
            };
            controller.ElementsList[elem.Elem.X, elem.Elem.Y] = elem;
            elem.Elem.Power = power;
            cel.Cell = elem;

            return elem;
        }
        public static void GraveModule(this CraftController controller, GridController cel)
        {
            cel.Cell = new ChipController {Module = new BaseChip()};
        }
        public static void CreateSpell(this CraftController controller, GridController cel, CraftSpellTypes stype)
        {
           
        }
    }
    
}
