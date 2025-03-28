using ARAWorks.Inventory.Contracts;
using System.Collections.Generic;

namespace ARAWorks.QuickSlots
{
    public interface IDBItemQuickSlots
    {
        Dictionary<int, ContractStorageItem> GetQuickSlotItems(string characterID);

        ///<summary>
        /// Used to add the given item to its storage type in character's items.
        ///</summary>
        /// <param name="characterID">The given characters ID.</param>
        /// <param name="item">The given item.</param>
        string AddCharacterItem(string characterID, ContractStorageItem item);

        void UpdateCharacterItem(ContractStorageItem item);

        void DeleteItemFromCharacterItems(string CharacterInventoryIDRef);
    }
}