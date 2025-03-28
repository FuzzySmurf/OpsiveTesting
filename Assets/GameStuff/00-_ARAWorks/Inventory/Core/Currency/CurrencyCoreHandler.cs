
namespace ARAWorks.Inventory.Currency
{
    public class CurrencyCoreHandler : ICurrencyCoreHandler
    {
        public CurrencyDataHandler CurrencyDataHandler { get; private set; }

        private IDBItemCurrency _itemService;


        public CurrencyCoreHandler(string characterID, IDBItemCurrency itemService)
        {
            _itemService = itemService;
            CurrencyDataHandler = new CurrencyDataHandler(characterID, itemService);
        }
    }
}
