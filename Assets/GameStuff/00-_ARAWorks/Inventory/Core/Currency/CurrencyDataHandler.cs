using ARAWorks.Inventory.Contracts;
using ARAWorks.Inventory.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Inventory.Currency
{
    public class CurrencyDataHandler : ICurrencyDataHandler
    {
        public event Action<ECurrencyType, long> CurrencyUpdated;

        public IReadOnlyDictionary<ECurrencyType, ContractCurrency> Currencies => _currencyData.Currencies;

        private CurrencyData _currencyData;
        private IDBItemCurrency _itemService;
        private string _characterID;

        public CurrencyDataHandler(string characterID, IDBItemCurrency itemService)
        {
            _characterID = characterID;
            _itemService = itemService;

            _currencyData = new CurrencyData(characterID, itemService);
        }

        public void SetCurrencyAmount(ECurrencyType type, long amount)
        {
            if (amount < 0)
            {
                Debug.LogError($"Currency Service -- Unable to set amount. Amount is less then 0 (\"Negative\"). Negative currency values are not allowed. Incoming value: {amount}");
                return;
            }

            _currencyData.SetAmount(type, amount);
            _itemService.UpdateCurrency(_characterID, type, amount);
            CurrencyUpdated?.Invoke(type, amount);
        }

        public void AddCurrencyAmount(ECurrencyType type, long amount)
        {
            long newValue = Currencies[type].Amount + amount;

            SetCurrencyAmount(type, newValue);
        }

        public void SubtractCurrencyAmount(ECurrencyType type, long amount)
        {
            long newValue = Currencies[type].Amount - amount;

            SetCurrencyAmount(type, newValue);
        }
    }
}
