using ARAWorks.Inventory.Contracts;
using ARAWorks.Inventory.Enums;
using System.Collections;
using System.Collections.Generic;

namespace ARAWorks.Inventory.Currency
{
    public class CurrencyData
    {
        public Dictionary<ECurrencyType, ContractCurrency> Currencies { get; private set; }

        private IDBItemCurrency _itemService;

        public CurrencyData(string characterID, IDBItemCurrency itemService)
        {
            Currencies = new Dictionary<ECurrencyType, ContractCurrency>();
            foreach (ContractCurrency currency in itemService.GetAllCurrencies(characterID, true))
            {
                Currencies.Add(currency.CurrencyType, currency);
            }
        }

        public void SetAmount(ECurrencyType type, long amount)
        {
            Currencies[type].Amount = amount;
        }
    }
}
