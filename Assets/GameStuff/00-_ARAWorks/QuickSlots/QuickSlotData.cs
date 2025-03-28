using ARAWorks.Base.Contracts;
using ARAWorks.Inventory.Contracts;
using ARAWorks.Inventory.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ARAWorks.QuickSlots
{
    public class QuickSlotData
    {
        public int MaxStackAmount { get; set; } = 99;
        public int MaxSlotAmount { get; set; } = 4;

        public bool IsFull => _quickSlotItemsInternal.Count == MaxSlotAmount;

        public IReadOnlyDictionary<int, ContractItem> QuickSlotItems => GetQuickSlotsInternal();

        private Dictionary<int, ContractStorageItem> _quickSlotItemsInternal;

        public QuickSlotData(Dictionary<int, ContractStorageItem> items)
        {
            _quickSlotItemsInternal = items;
        }

        /// <summary>
        /// Attemps to find any items with the same GlobalItemIDRef as the provided item.
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <returns>A list of slotID's</returns>
        public List<ContractStorageItem> FindItems(string globalItemIDRef)
        {
            List<ContractStorageItem> items = new List<ContractStorageItem>();
            foreach (KeyValuePair<int, ContractStorageItem> pair in _quickSlotItemsInternal)
            {
                if (pair.Value.Item.GlobalItemIDRef == globalItemIDRef)
                    items.Add(pair.Value);
            }

            return items;
        }

        public ContractStorageItem GetItem(int slotID)
        {
            ContractStorageItem storageItem = null;

            if (_quickSlotItemsInternal.TryGetValue(slotID, out storageItem) == false) return null;

            return storageItem;
        }

        public ContractStorageItem GetItem(ContractItem item)
        {
            return _quickSlotItemsInternal.Where(x => x.Value.Item == item).FirstOrDefault().Value;
        }

        public int GetEmptySlot()
        {
            // Full slots, bail early.
            if (IsFull == true) return -1;

            for (int i = 1; i <= MaxSlotAmount; i++)
            {
                if (_quickSlotItemsInternal.ContainsKey(i) == false)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool IsSlotOccupied(int slotID)
        {
            return _quickSlotItemsInternal.ContainsKey(slotID);
        }

        public void AddItem(ContractStorageItem itemData)
        {
            itemData.TypeCharacterStorage = EEnumCharacterStorageType.QuickSlots;
            _quickSlotItemsInternal.Add(itemData.SlotNumber, itemData);
        }

        public void RemoveItem(int slotNumber)
        {
            _quickSlotItemsInternal.Remove(slotNumber);
        }

        private IReadOnlyDictionary<int, ContractItem> GetQuickSlotsInternal()
        {
            Dictionary<int, ContractItem> quickSlots = new Dictionary<int, ContractItem>();
            foreach (KeyValuePair<int, ContractStorageItem> inventoryInternal in _quickSlotItemsInternal)
            {
                quickSlots.Add(inventoryInternal.Key, inventoryInternal.Value.Item);
            }

            return quickSlots;
        }

    }
}