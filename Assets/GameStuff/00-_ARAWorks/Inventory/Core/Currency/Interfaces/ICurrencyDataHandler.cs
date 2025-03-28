
using ARAWorks.Inventory.Contracts;
using ARAWorks.Inventory.Enums;
using System;
using System.Collections.Generic;

namespace ARAWorks.Inventory.Currency
{
    public interface ICurrencyDataHandler
    {
        /// <summary>
        /// Triggered when any currency amount is changed. Returns the curreny type and the new amount.
        /// </summary>
        event Action<ECurrencyType, long> CurrencyUpdated;

        /// <summary>
        /// All stored currency.
        /// </summary>
        IReadOnlyDictionary<ECurrencyType, ContractCurrency> Currencies { get; }

        /// <summary>
        /// Set the amount of the specified currency. Does not allow negative values.
        /// </summary>
        /// <param name="type">The type to set.</param>
        /// <param name="amount">The amount to set.</param>
        void SetCurrencyAmount(ECurrencyType type, long amount);

        /// <summary>
        /// Set the amount of the specified currency. Does not allow negative values.
        /// </summary>
        /// <param name="type">The type to set.</param>
        /// <param name="amount">The amount to set.</param>
        void AddCurrencyAmount(ECurrencyType type, long amount);

        /// <summary>
        /// Subtract the amount of the specified currency. Does not allow negative values.
        /// </summary>
        /// <param name="type">The type to set.</param>
        /// <param name="amount">The amount to set.</param>
        void SubtractCurrencyAmount(ECurrencyType type, long amount);
    }
}