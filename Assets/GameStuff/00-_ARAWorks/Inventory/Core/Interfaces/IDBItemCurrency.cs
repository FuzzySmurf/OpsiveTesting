using ARAWorks.Inventory.Contracts;
using ARAWorks.Inventory.Enums;
using System.Collections.Generic;

namespace ARAWorks.Inventory
{
    public interface IDBItemCurrency
    {
        /// <summary>
        /// Gets the characters current currency based on the curreny type.
        /// </summary>
        /// <param name="characterID">The character to access.</param>
        /// <param name="currencyType">Currency type to get.</param>
        /// <param name="createIfNonexistent">Create in the database if does not exist.</param>
        /// <returns></returns>
        ContractCurrency GetCurrency(string characterID, ECurrencyType currencyType, bool createIfNonexistent = false);

        /// <summary>
        /// Gets all the currency the character has.
        /// </summary>
        /// <param name="characterID">The character to access.</param>
        /// <param name="createIfNonexistent">Create in the database if does not exist.</param>
        /// <returns></returns>
        List<ContractCurrency> GetAllCurrencies(string characterID, bool createIfNonexistent = false);

        /// <summary>
        /// Creates or updates the currency for a character depending on if it exists already or not.
        /// </summary>
        /// <param name="characterID">The character to access.</param>
        /// <param name="currencyType">Currency type to update.</param>
        /// <param name="amount">Amount to set.</param>
        void UpdateCurrency(string characterID, ECurrencyType currencyType, long amount);

        /// <summary>
        /// Creates or updates the currency for a character depending on if it exists already or not.
        /// </summary>
        /// <param name="characterID">The character to access.</param>
        /// <param name="currency">Currency contract to use to update.</param>
        void UpdateCurrency(string characterID, ContractCurrency currency);


    }
}