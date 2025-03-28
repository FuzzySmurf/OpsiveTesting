using ARAWorks.Base.Contracts;
using ARAWorks.Base.Enums;
using ARAWorks.Base.Interfaces;
using ARAWorks.Interstice;
using ARAWorks.Inventory.Enums;
using System;
using System.Collections.Generic;

namespace ARAWorks.Inventory
{
    public class InventoryCoreService : IInventoryCoreService
    {
        public event Action<ContractItemEquipable, EEquipmentSlotType> ItemEquipped { add => _inventoryCoreHandler.EquipmentDataHandler.ItemEquipped += value; remove => _inventoryCoreHandler.EquipmentDataHandler.ItemEquipped -= value; }
        public event Action<ContractItemEquipable, EEquipmentSlotType> ItemUnequipped { add => _inventoryCoreHandler.EquipmentDataHandler.ItemUnequipped += value; remove => _inventoryCoreHandler.EquipmentDataHandler.ItemUnequipped -= value; }
        public event Action<ContractItemEquipable, EEquipmentSlotType> ItemUpdatedEquipment { add => _inventoryCoreHandler.EquipmentDataHandler.ItemUpdated += value; remove => _inventoryCoreHandler.EquipmentDataHandler.ItemUpdated -= value; }

        public event Action<ContractItem, int> ItemAdded { add => _inventoryCoreHandler.InventoryDataHandler.ItemAdded += value; remove => _inventoryCoreHandler.InventoryDataHandler.ItemAdded -= value; }
        public event Action<ContractItem, int> ItemRemoved { add => _inventoryCoreHandler.InventoryDataHandler.ItemRemoved += value; remove => _inventoryCoreHandler.InventoryDataHandler.ItemRemoved -= value; }
        public event Action<ContractItem, int> ItemUpdatedInventory { add => _inventoryCoreHandler.InventoryDataHandler.ItemUpdated += value; remove => _inventoryCoreHandler.InventoryDataHandler.ItemUpdated -= value; }

        public IReadOnlyDictionary<int, ContractItem> Inventory => _inventoryCoreHandler.InventoryDataHandler.Inventory;
        public int MaxStackAmount { get => _inventoryCoreHandler.InventoryDataHandler.MaxStackAmount; set => _inventoryCoreHandler.InventoryDataHandler.MaxStackAmount = value; }
        public int MaxSlotAmount { get => _inventoryCoreHandler.InventoryDataHandler.MaxSlotAmount; set => _inventoryCoreHandler.InventoryDataHandler.MaxSlotAmount = value; }
        public bool HasEmptyInventorySlot => _inventoryCoreHandler.InventoryDataHandler.HasEmptyInventorySlot;

        public IReadOnlyDictionary<EEquipmentSlotType, ContractItemWeapon> EquippedWeapons => _inventoryCoreHandler.EquipmentDataHandler.EquippedWeapons;
        public IReadOnlyDictionary<EEquipmentSlotType, ContractItemArmor> EquippedArmor => _inventoryCoreHandler.EquipmentDataHandler.EquippedArmor;

        public IItemInterstice InventoryInterstice => _inventoryCoreHandler.InventoryDataHandler;
        public IItemInterstice EquipmentInterstice => _inventoryCoreHandler.EquipmentDataHandler;

        private InventoryCoreHandler _inventoryCoreHandler;


        public InventoryCoreService(string characterID, IDBItemInventory itemService, IDBItemService itemObjectService)
        {
            _inventoryCoreHandler = new InventoryCoreHandler(characterID, itemService, itemObjectService);
        }


        public EEnumCharacterStorageType GetItemStorageLocation(ContractItem item)
        {
            return _inventoryCoreHandler.GetItemStorageLocation(item);
        }

        #region Inventory

        public InventoryItemOverFlowResults AddItem(ContractItem inventoryItem)
        {
            return _inventoryCoreHandler.InventoryDataHandler.AddItem(inventoryItem);
        }

        public InventoryItemOverFlowResults AddItem(ContractItem inventoryItem, int slotNumber)
        {
            return _inventoryCoreHandler.InventoryDataHandler.AddItem(inventoryItem, slotNumber);
        }

        public bool RemoveItem(int slotNumber)
        {
            return _inventoryCoreHandler.InventoryDataHandler.RemoveItem(slotNumber);
        }

        public bool RemoveItem(ContractItem inventoryItem)
        {
            return _inventoryCoreHandler.InventoryDataHandler.RemoveItem(inventoryItem);
        }

        public bool RemoveQuantity(int slotNumber, int amount)
        {
            return _inventoryCoreHandler.InventoryDataHandler.RemoveQuantity(slotNumber, amount);
        }

        public bool RemoveQuantity(ContractItem inventoryItem, int amount)
        {
            return _inventoryCoreHandler.InventoryDataHandler.RemoveQuantity(inventoryItem, amount);
        }

        public bool CanStack(ContractItem newItem, int slotNumber)
        {
            return _inventoryCoreHandler.InventoryDataHandler.CanStack(newItem, slotNumber);
        }

        public bool CanStack(ContractItem newItem, ContractItem itemInInventory)
        {
            return _inventoryCoreHandler.InventoryDataHandler.CanStack(newItem, itemInInventory);
        }

        public void UpdateItem(ContractItem inventoryItem)
        {
            _inventoryCoreHandler.InventoryDataHandler.UpdateItem(inventoryItem);
        }

        public bool IsInventorySlotOccupied(int slotNumber)
        {
            return _inventoryCoreHandler.InventoryDataHandler.IsInventorySlotOccupied(slotNumber);
        }

        public ContractItem GetItem(int slotID)
        {
            return _inventoryCoreHandler.InventoryDataHandler.GetItem(slotID);
        }

        public int GetItemSlotNumber(ContractItem item)
        {
            return _inventoryCoreHandler.InventoryDataHandler.GetItemSlotNumber(item);
        }

        public int GetEmptySlot()
        {
            return _inventoryCoreHandler.InventoryDataHandler.GetEmptySlot();
        }

        public bool CanAddItem(ContractItem item, int slotNumber)
        {
            return _inventoryCoreHandler.InventoryDataHandler.CanAddItem(item, slotNumber);
        }

        public bool CanRemoveItem(int slotNumber)
        {
            return _inventoryCoreHandler.InventoryDataHandler.CanRemoveItem(slotNumber);
        }

        #endregion

        #region Equipment

        public IReadOnlyDictionary<EEquipmentSlotType, ContractItemEquipable> GetAllEquipment()
        {
            return _inventoryCoreHandler.EquipmentDataHandler.GetAllEquipment();
        }

        public bool EquipItem(ContractItemEquipable inventoryItem)
        {
            return _inventoryCoreHandler.EquipmentDataHandler.EquipItem(inventoryItem);
        }

        public bool UnequipItem(ContractItemEquipable inventoryItem)
        {
            return _inventoryCoreHandler.EquipmentDataHandler.UnequipItem(inventoryItem);
        }

        public void UpdateItem(ContractItemEquipable inventoryItem)
        {
            _inventoryCoreHandler.EquipmentDataHandler.UpdateItem(inventoryItem);
        }

        public ContractItemEquipable GetEquippable(EEquipmentSlotType slotType)
        {
            return _inventoryCoreHandler.EquipmentDataHandler.GetEquippable(slotType);
        }

        public bool IsEquipmentSlotOccupied(EEquipmentSlotType slotType)
        {
            return _inventoryCoreHandler.EquipmentDataHandler.IsEquipmentSlotOccupied(slotType);
        }

        public bool CanEquipItem(ContractItemEquipable inventoryItem)
        {
            return _inventoryCoreHandler.EquipmentDataHandler.CanEquipItem(inventoryItem);
        }

        public bool CanUnequipItem(ContractItemEquipable inventoryItem)
        {
            return _inventoryCoreHandler.EquipmentDataHandler.CanUnequipItem(inventoryItem);
        }

        #endregion

    }
}