using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Elements;

namespace Warsmiths.Server.Services.Economic
{
    public class GlobalElementStatement
    {
        public BaseElement Element ;

        /// <summary>
        /// Количество заказанных у сервера элементов
        /// </summary>
        public int OrderedQuantity ;

        /// <summary>
        /// Количество проданных серверу элементов
        /// </summary>
        public int SoldQuantity ;

        /// <summary>
        /// Цена по которой сервер продает игроку
        /// </summary>
        public int OrderPrice ;

        /// <summary>
        /// Цена по которой сервер покупает у игрока
        /// </summary>
        public int BuyPrice => OrderPrice-(int)(OrderPrice *0.2f);
    }
}
