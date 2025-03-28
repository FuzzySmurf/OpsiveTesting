using ARAWorks.Base.Contracts;
using ARAWorks.Base.Enums;
using ARAWorks.Interstice;
using ARAWorks.Inventory.Contracts;
using ARAWorks.Inventory.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Inventory
{
    public class EquipmentDataHandler : IItemInterstice
    {
        public event Action<ContractItemEquipable, EEquipmentSlotType> ItemEquipped;
        public event Action<ContractItemEquipable, EEquipmentSlotType> ItemUnequipped;
        public event Action<ContractItemEquipable, EEquipmentSlotType> ItemUpdated;

        public IReadOnlyDictionary<EEquipmentSlotType, ContractItemWeapon> EquippedWeapons => _equipmentData.EquippedWeapons;
        public IReadOnlyDictionary<EEquipmentSlotType, ContractItemArmor> EquippedArmor => _equipmentData.EquippedArmor;

        private EquipmentData _equipmentData;
        private string _characterID;
        private IDBItemInventory _itemService;


        public EquipmentDataHandler(string characterID, IDBItemInventory itemService)
        {
            _itemService = itemService;
            _characterID = characterID;
            _equipmentData = new EquipmentData(_itemService.GetEquippedWeapons(_characterID), _itemService.GetEquippedArmor(_characterID));
        }


        ////////////////////////// IInterstice Interface Implementation //////////////////////////////
        public bool IsItemValid(ContractItem item, int slotNumber)
        {
            ContractItemEquipable equipable = item as ContractItemEquipable;
            if (equipable == null) return false;

            if ((int)equipable.TypeEquipmentSlot != slotNumber) return false;

            return true;
        }

        public bool CanStack(ContractItem item, int slotNumber)
        {
            return false;
        }

        public InventoryItemOverFlowResults InsertItem(ContractItem item, int slotNumber)
        {
            ContractItemEquipable equipable = item as ContractItemEquipable;
            if (equipable == null) return new InventoryItemOverFlowResults(false);

            EquipItem(equipable);
            return new InventoryItemOverFlowResults(true);
        }

        public bool RemoveItem(int slotNumber)
        {
            ContractItemEquipable equipable = GetEquippable((EEquipmentSlotType)slotNumber);
            if (equipable == null) return false;

            UnequipItem(equipable);
            return true;
        }

        public ContractItem GetItemInSlot(int slotNumber)
        {
            ContractStorageItem equipable = _equipmentData.GetEquipment((EEquipmentSlotType)slotNumber);
            if (equipable == null) return null;

            return equipable.Item;
        }
        ////////////////////////// End IInterstice Interface Implementation //////////////////////////

        public IReadOnlyDictionary<EEquipmentSlotType, ContractItemEquipable> GetAllEquipment()
        {
            IReadOnlyDictionary<EEquipmentSlotType, ContractItemArmor> armor = _equipmentData.EquippedArmor;
            IReadOnlyDictionary<EEquipmentSlotType, ContractItemWeapon> weapons = _equipmentData.EquippedWeapons;

            Dictionary<EEquipmentSlotType, ContractItemEquipable> equips = new Dictionary<EEquipmentSlotType, ContractItemEquipable>();
            
            foreach (KeyValuePair<EEquipmentSlotType, ContractItemArmor> ar in armor)
            {
                equips.Add(ar.Key, ar.Value);
            }

            foreach (KeyValuePair<EEquipmentSlotType, ContractItemWeapon> wp in weapons)
            {
                equips.Add(wp.Key, wp.Value);
            }

            return equips;
        }

        public bool EquipItem(ContractItemEquipable equipable)
        {
            if (CanEquipItem(equipable) == false) return false;

            ContractStorageItem storageItem = new ContractStorageItem();
            storageItem.Item = equipable;
            storageItem.SlotNumber = -1;
            storageItem.TypeCharacterStorage = EEnumCharacterStorageType.Equipped;

            bool isItemEquipped = _equipmentData.AddEquipment(storageItem);
            if (isItemEquipped == true)
            {
                storageItem.CharacterInventoryIDRef = _itemService.AddCharacterItem(_characterID, storageItem);
                // Inform all listeners that an item has been equipped
                ItemEquipped?.Invoke(equipable, equipable.TypeEquipmentSlot);
                return true;
            }

            return false;
        }

        public bool UnequipItem(ContractItemEquipable equipable)
        {
            if (CanUnequipItem(equipable) == false) return false;

            ContractStorageItem storageItem = GetStorageItem(equipable);

            if (_equipmentData.RemoveEquipment(storageItem))
            {
                _itemService.DeleteItemFromCharacterItems(storageItem.CharacterInventoryIDRef);

                ItemUnequipped?.Invoke(equipable, equipable.TypeEquipmentSlot);
                return true;
            }

            return false;
        }

        public void UpdateItem(ContractItemEquipable equipable)
        {
            ContractStorageItem storageItem = GetStorageItem(equipable);

            if (string.IsNullOrEmpty(storageItem.CharacterInventoryIDRef) == true) 
            {
                Debug.LogError("EquipmentDataHandler::UpdateItem -- Attempting to update an item that does not have their \"CharInventoryIDRef\" set.");
                return;
            }

            _itemService.UpdateCharacterItem(storageItem);
            ItemUpdated?.Invoke(equipable, equipable.TypeEquipmentSlot);
        }

        public ContractItemEquipable GetEquippable(EEquipmentSlotType slotType)
        {
            ContractStorageItem storageItem = _equipmentData.GetEquipment(slotType);
            if (storageItem == null) return null;

            return (ContractItemEquipable)storageItem.Item;
        }

        public bool IsEquipmentSlotOccupied(EEquipmentSlotType slotType)
        {
            return _equipmentData.IsEquipmentSlotOccupied(slotType);
        }

        public bool CanEquipItem(ContractItemEquipable equipable)
        {

            return _equipmentData.IsEquipmentSlotOccupied(equipable.TypeEquipmentSlot) == false;
        }

        public bool CanUnequipItem(ContractItemEquipable equipable)
        {
            return _equipmentData.IsEquipmentSlotOccupied(equipable.TypeEquipmentSlot) == true;
        }

        public ContractStorageItem GetStorageItem(ContractItemEquipable equipable)
        {
            ContractStorageItem storageItem = _equipmentData.GetEquipment(equipable.TypeEquipmentSlot);
            if (storageItem == null || storageItem.Item != equipable)
                storageItem = null;

            return storageItem;
        }

    }
}