using ARAWorks.Base.Contracts;
using ARAWorks.Interstice;
using ARAWorks.Inventory;
using ARAWorks.Inventory.Contracts;
using ARAWorks.Inventory.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ARAWorks.QuickSlots
{
    public class QuickSlotDataHandler : IItemInterstice
    {
        public event Action<ContractItem, int> ItemAdded;
        public event Action<ContractItem, int> ItemRemoved;
        public event Action<ContractItem, int> ItemUpdated;

        public IReadOnlyDictionary<int, ContractItem> QuickSlotItems => _quickSlotData.QuickSlotItems;
        public bool HasEmptySlot => _quickSlotData.IsFull == false;
        public int MaxStackAmount { get => _quickSlotData.MaxStackAmount; set => _quickSlotData.MaxStackAmount = value; }
        public int MaxSlotAmount { get => _quickSlotData.MaxSlotAmount; set => _quickSlotData.MaxSlotAmount = value; }

        private IDBItemQuickSlots _dbService;
        private QuickSlotData _quickSlotData;
        private string _characterID;


        public QuickSlotDataHandler(string characterID, IDBItemQuickSlots dbService)
        {
            _characterID = characterID;
            _dbService = dbService;

            _quickSlotData = new QuickSlotData(_dbService.GetQuickSlotItems(_characterID));
        }


        /////////////// IItermInterstice Implementation /////////////////////
        public ContractItem GetItemInSlot(int slotNumber)
        {
            ContractStorageItem storageItem = _quickSlotData.GetItem(slotNumber);
            if (storageItem == null) return null;

            return storageItem.Item;
        }

        public InventoryItemOverFlowResults InsertItem(ContractItem item, int slotNumber)
        {
            InventoryItemOverFlowResults overFlowResults = AddItem(item, slotNumber);
            if (overFlowResults.hasOverflow == true)
            {
                List<ContractItem> pastLimitItems = new List<ContractItem>();
                foreach (ContractItem overflowItem in overFlowResults.overflowItems)
                {
                    InventoryItemOverFlowResults overflow = AddItem(overflowItem);
                    if (overflow.hasOverflow == true)
                        pastLimitItems.AddRange(overflow.overflowItems);
                }
                return new InventoryItemOverFlowResults(true, pastLimitItems);
            }
            return new InventoryItemOverFlowResults(true);
        }

        public bool IsItemValid(ContractItem item, int slotNumber)
        {
            ContractItemConsumable consumable = item as ContractItemConsumable;
            return consumable != null;
        }

        public bool RemoveItem(int slotNumber)
        {
            ContractStorageItem item = _quickSlotData.GetItem(slotNumber);
            if (item != null)
                return RemoveItem(item.Item);
            else
                return false;
        }
        /////////////// End IItermInterstice Implementation /////////////////

        public InventoryItemOverFlowResults AddItem(ContractItem itemToAdd, int slotNumber)
        {
            // CanAddItem also handles whether the slot is occupied and can be stacked on the occupied item
            if (CanAddItem(itemToAdd, slotNumber) == false) return new InventoryItemOverFlowResults(false, itemToAdd);

            if (_quickSlotData.IsSlotOccupied(slotNumber) == true)
            {
                ContractStorageItem itemInSlot = _quickSlotData.GetItem(slotNumber);
                int remainder = StackItem(itemInSlot.Item, itemToAdd.Quantity);
                UpdateItem(itemInSlot);

                itemToAdd.Quantity = remainder;

                if (remainder > 0)
                    return new InventoryItemOverFlowResults(true, itemToAdd);
            }
            else
            {
                AddItemInternal(itemToAdd, slotNumber);
            }

            return new InventoryItemOverFlowResults(true);
        }

        public InventoryItemOverFlowResults AddItem(ContractItem itemToAdd)
        {
            if (itemToAdd.Quantity == 0) return new InventoryItemOverFlowResults(false);

            AddItemStack(itemToAdd);

            bool itemAdded = false;

            // If our item is stackable, check if our current amount is more then our max
            // and add it to the quick slots.

            // If we cannot stack then the max quantity is 1
            int maxStackQuantity = itemToAdd.IsStackable == true ? MaxStackAmount : 1;
            if (itemToAdd.Quantity > maxStackQuantity)
            {
                itemToAdd.Quantity = maxStackQuantity;
            }

            if (HasEmptySlot == true)
            {
                itemAdded = true;
                AddItemInternal(itemToAdd, _quickSlotData.GetEmptySlot());
            }

            return new InventoryItemOverFlowResults(itemAdded, itemToAdd);
        }

        public void UpdateItem(ContractItem inventoryItem)
        {
            ContractStorageItem storageItem = _quickSlotData.GetItem(inventoryItem);
            UpdateItem(storageItem);
        }

        public void UpdateItem(ContractStorageItem storageItem)
        {
            if (string.IsNullOrEmpty(storageItem.CharacterInventoryIDRef) == true)
            {
                Debug.LogError("QuickSlotDataHandler::UpdateItem -- Attempting to update an item that does not have their \"CharInventoryIDRef\" set.");
                return;
            }

            _dbService.UpdateCharacterItem(storageItem);
            ItemUpdated?.Invoke(storageItem.Item, storageItem.SlotNumber);
        }

        /// <summary>
        /// Returns true if the slot is empty or if the slot is occupied
        /// our new item must be: the same item as the one in slot, stackable, and the item in slot must be less than max quantity.
        /// </summary>
        /// <param name="slotNumber"></param>
        /// <param name="itemToAdd"></param>
        /// <returns></returns>
        public bool CanAddItem(ContractItem itemToAdd, int slotNumber)
        {
            bool IsOccupied = _quickSlotData.IsSlotOccupied(slotNumber);
            if (IsOccupied == true)
            {
                if (itemToAdd.IsStackable == false) return false;

                ContractItem itemInSlot = _quickSlotData.GetItem(slotNumber).Item;
                return CanStack(itemToAdd, itemInSlot);
            }
            return IsOccupied == false;
        }

        public InventoryItemOverFlowResults AddItemStack(ContractItem itemToStack)
        {
            // Handle filling in multiple items of the same type.
            if (itemToStack.IsStackable == true)
            {
                int remainder = 0;
                List<ContractStorageItem> stackables = _quickSlotData.FindItems(itemToStack.GlobalItemIDRef);

                if (stackables.Count > 0)
                {
                    stackables = stackables.OrderBy(x => x.SlotNumber).Where(x => x.Item.Quantity < MaxStackAmount).ToList();

                    remainder = itemToStack.Quantity;

                    foreach (ContractStorageItem item in stackables)
                    {
                        remainder = StackItem(item.Item, remainder);
                        UpdateItem(item);

                        if (remainder == 0)
                        {
                            itemToStack.Quantity = remainder;
                            // All of our item was used to fill other items so no item is left.
                            return new InventoryItemOverFlowResults(true);
                        }
                    }

                    itemToStack.Quantity = remainder;
                }

            }

            return new InventoryItemOverFlowResults(false, itemToStack);

        }

        public ContractItem GetItem(int slotNumber)
        {
            if (IsSlotOccupied(slotNumber) == true)
                return _quickSlotData.GetItem(slotNumber).Item;

            return null;
        }

        public int GetItemSlotNumber(ContractItem item)
        {
            ContractStorageItem storageItem = _quickSlotData.GetItem(item);

            if (storageItem != null)
            {
                return storageItem.SlotNumber;
            }

            return -1;
        }

        public bool CanStack(ContractItem newItem, int slotNumber)
        {
            ContractItem itemInSlot = GetItem(slotNumber);
            return CanStack(newItem, itemInSlot);
        }

        public bool CanStack(ContractItem newItem, ContractItem itemInSlot)
        {
            if (itemInSlot == null || itemInSlot.IsStackable == false)
                return false;

            return itemInSlot.GlobalItemIDRef == newItem.GlobalItemIDRef && itemInSlot.Quantity < MaxStackAmount;
        }

        public bool IsSlotOccupied(int slotNumber)
        {
            return _quickSlotData.IsSlotOccupied(slotNumber);
        }

        public bool RemoveItem(ContractItem inventoryItem)
        {
            ContractStorageItem storageItem = _quickSlotData.GetItem(inventoryItem);
            if (storageItem == null || CanRemoveItem(storageItem.SlotNumber) == false)
                return false;

            _dbService.DeleteItemFromCharacterItems(storageItem.CharacterInventoryIDRef);
            _quickSlotData.RemoveItem(storageItem.SlotNumber);
            ItemRemoved?.Invoke(inventoryItem, storageItem.SlotNumber);
            return true;
        }

        public bool RemoveQuantity(int slotNumber, int amount)
        {
            return RemoveQuantity(_quickSlotData.GetItem(slotNumber).Item, amount);
        }

        public bool RemoveQuantity(ContractItem item, int amount)
        {
            if (item.IsStackable == false) return false;

            if (item.Quantity - amount <= 0)
            {
                item.Quantity = 0;
                RemoveItem(item);
                return false;
            }

            item.Quantity -= amount;
            UpdateItem(item);

            return true;
        }

        public bool CanRemoveItem(int slotNumber)
        {
            return _quickSlotData.IsSlotOccupied(slotNumber) == true;
        }


        /// <summary>
        /// Adds to an items stack without going over their limit.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>Any remainder after stacking.</returns>
        private int StackItem(ContractItem item, int amount)
        {
            if (item.IsStackable == false || amount <= 0) return 0;

            int quantityToAdd = item.Quantity + amount;
            int newRemainder = quantityToAdd - MaxStackAmount;

            if (quantityToAdd > MaxStackAmount)
                quantityToAdd = MaxStackAmount;

            if (newRemainder < 0)
                newRemainder = 0;

            item.Quantity = quantityToAdd;

            return newRemainder;
        }

        private void AddItemInternal(ContractItem quickSlotItem, int slotNumber)
        {
            ContractStorageItem storageItem = new ContractStorageItem()
            {
                Item = quickSlotItem,
                SlotNumber = slotNumber,
                TypeCharacterStorage = EEnumCharacterStorageType.QuickSlots
            };

            storageItem.CharacterInventoryIDRef = _dbService.AddCharacterItem(_characterID, storageItem);

            _quickSlotData.AddItem(storageItem);
            ItemAdded?.Invoke(storageItem.Item, storageItem.SlotNumber);
        }
    }
}