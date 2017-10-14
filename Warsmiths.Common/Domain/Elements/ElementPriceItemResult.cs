namespace YourGame.Common.Domain.Elements
{
    public class ElementPriceItemResult
    {
        /// <summary>
        /// Id элемента
        /// </summary>
        public string ElementId ;

        /// <summary>
        /// Цена элемент по которой сервер продает
        /// </summary>
        public float SellPrice ;

        /// <summary>
        /// Цена элемента по которой сервер покупает
        /// </summary>
        public float BuyPrice ;
    }
}
