using ARAWorks.Inventory.Contracts;
using ARAWorks.Inventory.Enums;
using System;
using System.Collections.Generic;

namespace ARAWorks.Inventory.Currency
{
    public class CurrencyCoreService : ICurrencyCoreService
    {
        public event Action<ECurrencyType, long> CurrencyUpdated { add => _currencyCoreHandler.CurrencyDataHandler.CurrencyUpdated += value; remove => _currencyCoreHandler.CurrencyDataHandler.CurrencyUpdated -= value; }
        public IReadOnlyDictionary<ECurrencyType, ContractCurrency> Currencies => _currencyCoreHandler.CurrencyDataHandler.Currencies;

        private CurrencyCoreHandler _currencyCoreHandler;

        public CurrencyCoreService(string characterID, IDBItemCurrency itemService)
        {
            _currencyCoreHandler = new CurrencyCoreHandler(characterID, itemService);
        }

        public void SetCurrencyAmount(ECurrencyType type, long amount)
        {
            _currencyCoreHandler.CurrencyDataHandler.SetCurrencyAmount(type, amount);
        }

        public void AddCurrencyAmount(ECurrencyType type, long amount)
        {
            _currencyCoreHandler.CurrencyDataHandler.AddCurrencyAmount(type, amount);
        }

        public void SubtractCurrencyAmount(ECurrencyType type, long amount)
        {
            _currencyCoreHandler.CurrencyDataHandler.SubtractCurrencyAmount(type, amount);
        }
    }
}
