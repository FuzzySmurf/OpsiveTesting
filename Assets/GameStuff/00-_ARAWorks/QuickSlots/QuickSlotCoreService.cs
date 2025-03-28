using ARAWorks.Base.Contracts;
using ARAWorks.Interstice;
using ARAWorks.Inventory;
using System;
using System.Collections.Generic;

namespace ARAWorks.QuickSlots
{
    public class QuickSlotCoreService : IQuickSlotCoreService
    {
        public event Action<ContractItem, int> ItemAdded { add => _quickSlotDataHandler.ItemAdded += value; remove => _quickSlotDataHandler.ItemAdded -= value; }
        public event Action<ContractItem, int> ItemRemoved { add => _quickSlotDataHandler.ItemRemoved += value; remove => _quickSlotDataHandler.ItemRemoved -= value; }
        public event Action<ContractItem, int> ItemUpdated { add => _quickSlotDataHandler.ItemUpdated += value; remove => _quickSlotDataHandler.ItemUpdated -= value; }

        public IReadOnlyDictionary<int, ContractItem> QuickSlotItems => _quickSlotDataHandler.QuickSlotItems;
        public int MaxStackAmount { get => _quickSlotDataHandler.MaxStackAmount; set => _quickSlotDataHandler.MaxStackAmount = value; }
        public int MaxSlotAmount { get => _quickSlotDataHandler.MaxSlotAmount; set => _quickSlotDataHandler.MaxSlotAmount = value; }
        public bool HasEmptySlot => _quickSlotDataHandler.HasEmptySlot;
        public IItemInterstice QuickSlotInterstice => _quickSlotDataHandler;

        private QuickSlotDataHandler _quickSlotDataHandler;


        public QuickSlotCoreService(string characterID, IDBItemQuickSlots dbQuickSlots)
        {
            _quickSlotDataHandler = new QuickSlotDataHandler(characterID, dbQuickSlots);
        }


        public InventoryItemOverFlowResults AddItem(ContractItem itemToAdd)
        {
            return _quickSlotDataHandler.AddItem(itemToAdd);
        }

        public InventoryItemOverFlowResults AddItem(ContractItem itemToAdd, int slotNumber)
        {
            return _quickSlotDataHandler.AddItem(itemToAdd, slotNumber);
        }

        public InventoryItemOverFlowResults AddItemStack(ContractItem itemToStack)
        {
            return _quickSlotDataHandler.AddItemStack(itemToStack);
        }

        public bool CanAddItem(ContractItem item, int slotNumber)
        {
            return _quickSlotDataHandler.CanAddItem(item, slotNumber);
        }

        public bool CanRemoveItem(int slotNumber)
        {
            return _quickSlotDataHandler.CanRemoveItem(slotNumber);
        }

        public bool CanStack(ContractItem newItem, int slotNumber)
        {
            return _quickSlotDataHandler.CanStack(newItem, slotNumber);
        }

        public bool CanStack(ContractItem newItem, ContractItem itemInQuickSlot)
        {
            return _quickSlotDataHandler.CanStack(newItem, itemInQuickSlot);
        }

        public ContractItem GetItem(int slotID)
        {
            return _quickSlotDataHandler.GetItem(slotID);
        }

        public int GetItemSlotNumber(ContractItem item)
        {
            return _quickSlotDataHandler.GetItemSlotNumber(item);
        }

        public bool IsSlotOccupied(int slotNumber)
        {
            return _quickSlotDataHandler.IsSlotOccupied(slotNumber);
        }

        public bool RemoveItem(int slotNumber)
        {
            return _quickSlotDataHandler.RemoveItem(slotNumber);
        }

        public bool RemoveItem(ContractItem item)
        {
            return _quickSlotDataHandler.RemoveItem(item);
        }

        public bool RemoveQuantity(int slotNumber, int amount)
        {
            return _quickSlotDataHandler.RemoveQuantity(slotNumber, amount);
        }

        public bool RemoveQuantity(ContractItem item, int amount)
        {
            return _quickSlotDataHandler.RemoveQuantity(item, amount);
        }

        public void UpdateItem(ContractItem item)
        {
            _quickSlotDataHandler.UpdateItem(item);
        }
    }
}