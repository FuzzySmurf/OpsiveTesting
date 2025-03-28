using System.Linq;
using System.Collections.Generic;
using ARAWorks.Inventory.Contracts;
using ARAWorks.Base.Contracts;
using ARAWorks.Inventory.Enums;

namespace ARAWorks.Inventory
{
    public class InventoryData
    {
        public int MaxStackAmount { get; set; } = 99;
        public int MaxSlotAmount { get; set; } = 40;

        public bool IsFull => _inventoryItemsInternal.Count == MaxSlotAmount;

        public IReadOnlyDictionary<int, ContractItem> inventory => GetInventoryInternal();

        private Dictionary<int, ContractStorageItem> _inventoryItemsInternal;

        public InventoryData(Dictionary<int, ContractStorageItem> items)
        {
            _inventoryItemsInternal = items;
        }

        /// <summary>
        /// Attemps to find any inventory items with the same GlobalItemIDRef as the provided item.
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <returns>A list of slotID's</returns>
        public List<ContractStorageItem> FindItems(string globalItemIDRef)
        {
            List<ContractStorageItem> items = new List<ContractStorageItem>();
            foreach(KeyValuePair<int, ContractStorageItem> pair in _inventoryItemsInternal)
            {
                if (pair.Value.Item.GlobalItemIDRef == globalItemIDRef)
                {
                    items.Add(pair.Value);
                }
            }

            return items;
        }

        public ContractStorageItem GetItem(int slotID)
        {
            ContractStorageItem storageItem = null;

            if (_inventoryItemsInternal.TryGetValue(slotID, out storageItem) == false) return null;

            return storageItem;
        }

        public ContractStorageItem GetItem(ContractItem item)
        {
            return _inventoryItemsInternal.Where(x => x.Value.Item == item).FirstOrDefault().Value;
        }

        public int GetEmptySlot()
        {
            //Full Inventory early bail.
            if(IsFull == true) return -1;

            for (int i = 1; i <= MaxSlotAmount; i++)
            {
                if (_inventoryItemsInternal.ContainsKey(i) == false)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool IsSlotOccupied(int slotID)
        {
            return _inventoryItemsInternal.ContainsKey(slotID);
        }

        public void AddItem(ContractStorageItem itemData)
        {
            itemData.TypeCharacterStorage = EEnumCharacterStorageType.Inventory;
            _inventoryItemsInternal.Add(itemData.SlotNumber, itemData);
        }

        public void RemoveItem(int slotNumber)
        {
            _inventoryItemsInternal.Remove(slotNumber);
        }

        private IReadOnlyDictionary<int, ContractItem> GetInventoryInternal()
        {
            Dictionary<int, ContractItem> inventory = new Dictionary<int, ContractItem>();
            foreach (KeyValuePair<int, ContractStorageItem> inventoryInternal in _inventoryItemsInternal)
            {
                inventory.Add(inventoryInternal.Key, inventoryInternal.Value.Item);
            }

            return inventory;
        }

    }
}