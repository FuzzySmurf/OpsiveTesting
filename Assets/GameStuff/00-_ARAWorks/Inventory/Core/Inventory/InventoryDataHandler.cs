using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ARAWorks.Inventory.Contracts;
using ARAWorks.Inventory.Enums;
using ARAWorks.Base.Contracts;
using ARAWorks.Base.Enums;
using ARAWorks.Base.Interfaces;
using ARAWorks.Interstice;

namespace ARAWorks.Inventory
{
    public class InventoryDataHandler : IItemInterstice
    {
        public event Action<ContractItem, int> ItemAdded;
        public event Action<ContractItem, int> ItemRemoved;
        public event Action<ContractItem, int> ItemUpdated;

        public IReadOnlyDictionary<int, ContractItem> Inventory => _inventoryData.inventory;
        public bool HasEmptyInventorySlot => _inventoryData.IsFull == false;
        public int MaxStackAmount { get => _inventoryData.MaxStackAmount; set => _inventoryData.MaxStackAmount = value; }
        public int MaxSlotAmount { get => _inventoryData.MaxSlotAmount; set => _inventoryData.MaxSlotAmount = value; }

        private IDBItemInventory _itemService;
        private IDBItemService _itemObjectService;
        private InventoryData _inventoryData;
        private string _characterID;


        public InventoryDataHandler(string characterID, IDBItemInventory itemService, IDBItemService itemObjectService)
        {
            _characterID = characterID;
            _itemService = itemService;
            _itemObjectService = itemObjectService;

            _inventoryData = new InventoryData(_itemService.GetInventoryItems(_characterID));
        }


        ////////////////////////// IInterstice Interface Implementation //////////////////////////
        public bool IsItemValid(ContractItem item, int slotNumber)
        {
            return true;
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

        public ContractItem GetItemInSlot(int slotNumber)
        {
            ContractStorageItem storageItem = _inventoryData.GetItem(slotNumber);
            if (storageItem == null) return null;

            return storageItem.Item;
        }
        ////////////////////////// End IInterstice Interface Implementation //////////////////////////

        public InventoryItemOverFlowResults AddItem(ContractItem itemToAdd, int slotNumber)
        {
            // CanAddItem also handles whether the slot is occupied and can be stacked on the occupied item
            if (CanAddItem(itemToAdd, slotNumber) == false) return new InventoryItemOverFlowResults(false, itemToAdd);

            if (_inventoryData.IsSlotOccupied(slotNumber) == true)
            {
                ContractStorageItem itemInSlot = _inventoryData.GetItem(slotNumber);
                int remainder = StackItem(itemInSlot.Item, itemToAdd.Quantity);
                UpdateItem(itemInSlot);

                itemToAdd.Quantity = remainder;

                if (remainder > 0)
                {
                    return new InventoryItemOverFlowResults(true, itemToAdd);
                }

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

            int remainder = 0;

            // Handle filling in multiple items of the same type.
            if (itemToAdd.IsStackable == true)
            {
                List<ContractStorageItem> stackables = _inventoryData.FindItems(itemToAdd.GlobalItemIDRef);
                
                if (stackables.Count > 0)
                {
                    stackables = stackables.OrderBy(x => x.SlotNumber).Where(x => x.Item.Quantity < MaxStackAmount).ToList();

                    remainder = itemToAdd.Quantity;

                    foreach(ContractStorageItem item in stackables)
                    {
                        remainder = StackItem(item.Item, remainder);
                        UpdateItem(item);
                        
                        if (remainder == 0)
                        {
                            itemToAdd.Quantity = remainder;
                            // All of our item was used to fill other items so no item is left.
                            return new InventoryItemOverFlowResults(true);
                        }
                    }

                    itemToAdd.Quantity = remainder;
                }
            }


            remainder = 0;
            List<ContractItem> itemsPastLimit = new List<ContractItem>();
            bool itemAdded = false;

            // If our item is stackable, check if our current amount is more then our max
            // and add it to the invnetory.

            // If we cannot stack then the max quantity is 1
            int maxStackQuantity = itemToAdd.IsStackable == true ? MaxStackAmount : 1;
            if (itemToAdd.Quantity > maxStackQuantity)
            {
                remainder = itemToAdd.Quantity - maxStackQuantity;
                itemToAdd.Quantity = maxStackQuantity;
            }

            if (HasEmptyInventorySlot == true)
            {
                itemAdded = true;
                AddItemInternal(itemToAdd, _inventoryData.GetEmptySlot());
            }
            else
            {
                itemsPastLimit.Add(itemToAdd);
            }


            // If we have more remaining, then try to create them and add them.
            while(remainder > 0)
            {
                ContractItem newItem = CreateNewInventoryItem(itemToAdd);

                if (newItem.IsStackable == true)
                {
                    remainder = StackItem(newItem, remainder);
                }
                else
                {
                    newItem.Quantity = 1;
                    remainder -= 1;
                }

                if (HasEmptyInventorySlot == true)
                {
                    AddItemInternal(newItem, _inventoryData.GetEmptySlot());
                }
                else
                {
                    itemsPastLimit.Add(newItem);
                }
            }

            return new InventoryItemOverFlowResults(itemAdded, itemsPastLimit);
        }

        public void UpdateItem(ContractItem inventoryItem)
        {
            ContractStorageItem storageItem = _inventoryData.GetItem(inventoryItem);
            UpdateItem(storageItem);
        }

        public void UpdateItem(ContractStorageItem storageItem)
        {
            if (string.IsNullOrEmpty(storageItem.CharacterInventoryIDRef) == true) 
            {
                Debug.LogError("InventoryDataHandler::UpdateItem -- Attempting to update an item that does not have their \"CharInventoryIDRef\" set.");
                return;
            }

            _itemService.UpdateCharacterItem(storageItem);
            ItemUpdated?.Invoke(storageItem.Item, storageItem.SlotNumber);
        }

        public bool RemoveItem(int slotNumber)
        {
            ContractStorageItem item = _inventoryData.GetItem(slotNumber);
            if (item != null)
                return RemoveItem(item.Item);
            else
                return false;
        }

        public bool RemoveItem(ContractItem inventoryItem)
        {
            ContractStorageItem storageItem = _inventoryData.GetItem(inventoryItem);
            if (storageItem == null || CanRemoveItem(storageItem.SlotNumber) == false) return false;

            _itemService.DeleteItemFromCharacterItems(storageItem.CharacterInventoryIDRef);
            _inventoryData.RemoveItem(storageItem.SlotNumber);
            ItemRemoved?.Invoke(inventoryItem, storageItem.SlotNumber);
            return true;
        }

        public bool RemoveQuantity(int slotNumber, int amount)
        {
            return RemoveQuantity(_inventoryData.GetItem(slotNumber).Item, amount);
        }

        public bool RemoveQuantity(ContractItem inventoryItem, int amount)
        {
            if (inventoryItem.IsStackable == false) return false;

            if (inventoryItem.Quantity - amount <= 0)
            {
                inventoryItem.Quantity = 0;
                RemoveItem(inventoryItem);
                return false;
            }

            inventoryItem.Quantity -= amount;
            UpdateItem(inventoryItem);

            return true;
        }

        public bool CanStack(ContractItem newItem, int slotNumber)
        {
            ContractItem itemInInventory = GetItem(slotNumber);
            return CanStack(newItem, itemInInventory);
        }

        public bool CanStack(ContractItem newItem, ContractItem itemInInventory)
        {
            if (itemInInventory == null || itemInInventory.IsStackable == false)
                return false;

            return itemInInventory.GlobalItemIDRef == newItem.GlobalItemIDRef && itemInInventory.Quantity < MaxStackAmount;
        }

        public ContractStorageItem GetStorageItem(ContractItem inventoryitem)
        {
            return _inventoryData.GetItem(inventoryitem);
        }

        public ContractItem GetItem(int slotNumber)
        {
            ContractItem item = null;

            if (IsInventorySlotOccupied(slotNumber) == true)
            {
                item = _inventoryData.GetItem(slotNumber).Item;
            }

            return item;
        }

        public int GetItemSlotNumber(ContractItem item)
        {
            ContractStorageItem storageItem = _inventoryData.GetItem(item);

            if (storageItem != null)
            {
                return storageItem.SlotNumber;
            }

            return -1;
        }

        public int GetEmptySlot()
        {
            return _inventoryData.GetEmptySlot();
        }

        public bool IsInventorySlotOccupied(int slotNumber)
        {
            return _inventoryData.IsSlotOccupied(slotNumber);
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
            bool IsOccupied = _inventoryData.IsSlotOccupied(slotNumber);
            if (IsOccupied == true)
            {
                if (itemToAdd.IsStackable == false) return false;

                ContractItem itemInSlot = _inventoryData.GetItem(slotNumber).Item;
                return CanStack(itemToAdd, itemInSlot);
            }
            return IsOccupied == false;
        }

        public bool CanRemoveItem(int slotNumber)
        {
            return _inventoryData.IsSlotOccupied(slotNumber) == true;
        }

        private void AddItemInternal(ContractItem inventoryItem, int slotNumber)
        {
            ContractStorageItem storageItem = new ContractStorageItem()
            {
                Item = inventoryItem,
                SlotNumber = slotNumber,
                TypeCharacterStorage = EEnumCharacterStorageType.Inventory
            };

            storageItem.CharacterInventoryIDRef = _itemService.AddCharacterItem(_characterID, storageItem);

            _inventoryData.AddItem(storageItem);
            ItemAdded?.Invoke(storageItem.Item, storageItem.SlotNumber);
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

        private ContractItem CreateNewInventoryItem(ContractItem inventoryItem)
        {
            ERarityType rarity = ERarityType.Common;

            if(inventoryItem is ContractItemEquipable)
            {
                rarity = ((ContractItemEquipable)inventoryItem).rarityType;
            }
            ContractItem item = _itemObjectService.GetGlobalItemData(inventoryItem.GlobalItemIDRef, rarity);

            //This is set to 0 here because if we are creating a new item and setting its amount
            item.Quantity = 0;

            return item;
        }

    }
}