using ARAWorks.Base.Enums;
using ARAWorks.Inventory.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Inventory
{
    public interface IDBItemInventory
    {
        ///<summary>
        /// Used to add the given item to its storage type in character's items.
        ///</summary>
        /// <param name="characterID">The given characters ID.</param>
        /// <param name="item">The given item.</param>
        string AddCharacterItem(string characterID, ContractStorageItem item);

        /// <summary>
        /// Used to hard delete an item from the character inventoryID.
        /// the CharacterItemID should be a unique ID to 'this' slot.
        /// </summary>
        /// <param name="charactedItemID">The CharacterItemID from the CharacterItem table.</param>
        void DeleteItemFromCharacterItems(string charactedItemID);

        void UpdateCharacterItem(string characterItemID, ContractStorageItem storageItem);

        void UpdateCharacterItem(ContractStorageItem storageItem);

        Dictionary<int, ContractStorageItem> GetInventoryItems(string characterID);

        Dictionary<EEquipmentSlotType, ContractStorageItem> GetEquippedArmor(string characterID);

        Dictionary<EEquipmentSlotType, ContractStorageItem> GetEquippedWeapons(string characterID);
        
    }
}
