using ARAWorks.Base.Contracts;
using ARAWorks.Base.Enums;
using ARAWorks.Base.Extensions;
using ARAWorks.Inventory.Contracts;
using ARAWorks.Inventory.Enums;
using System.Collections.Generic;

namespace ARAWorks.Inventory
{
    public class EquipmentData
    {
        public IReadOnlyDictionary<EEquipmentSlotType, ContractItemWeapon> EquippedWeapons => GetWeaponsInternal();
        public IReadOnlyDictionary<EEquipmentSlotType, ContractItemArmor> EquippedArmor => GetArmorInternal();

        private Dictionary<EEquipmentSlotType, ContractStorageItem> _equippedWeaponsInternal = new Dictionary<EEquipmentSlotType, ContractStorageItem>();
        private Dictionary<EEquipmentSlotType, ContractStorageItem> _equippedArmorInternal = new Dictionary<EEquipmentSlotType, ContractStorageItem>();


        public EquipmentData(Dictionary<EEquipmentSlotType, ContractStorageItem> weapons, Dictionary<EEquipmentSlotType, ContractStorageItem> armor)
        {
            _equippedWeaponsInternal = weapons;
            _equippedArmorInternal = armor;
        }

        public bool AddEquipment(ContractStorageItem storageItem)
        {
            if (storageItem.Item is not ContractItemEquipable equipable) return false;

            bool result = false;
            EEquipmentSlotType est = equipable.TypeEquipmentSlot;

            if (est.IsWeapon())
            {
                _equippedWeaponsInternal.Add(est, storageItem);
                result = true;
            }
            else if (est.IsAccessory() || est.IsArmor())
            {
                _equippedArmorInternal.Add(est, storageItem);
                result = true;
            }

            return result;

        }

        public bool RemoveEquipment(ContractStorageItem storageItem)
        {
            if (storageItem.Item is not ContractItemEquipable equipable) return false;

            bool result = false;

            EEquipmentSlotType est = equipable.TypeEquipmentSlot;

            bool hasArmor = _equippedArmorInternal.ContainsKey(est);
            bool hasWeapon = _equippedWeaponsInternal.ContainsKey(est);

            if (hasArmor)
            {
                _equippedArmorInternal.Remove(est);
                result = true;
            }
            else if (hasWeapon)
            {
                _equippedWeaponsInternal.Remove(est);
                result = true;
            }

            return result;

        }

        public ContractStorageItem GetEquipment(EEquipmentSlotType slotType)
        {
            ContractStorageItem equipable = null;

            if (slotType.IsWeapon())
            {
                if (_equippedWeaponsInternal.TryGetValue(slotType, out ContractStorageItem weapon) == true)
                {
                    equipable = weapon;
                }
            }
            else if (slotType.IsAccessory() || slotType.IsArmor())
            {
                if (_equippedArmorInternal.TryGetValue(slotType, out ContractStorageItem armor) == true)
                {
                    equipable = armor;
                }
            }

            return equipable;
        }

        public bool IsEquipmentSlotOccupied(EEquipmentSlotType slotType)
        {
            bool occupied = false;

            if (slotType.IsWeapon())
            {
                occupied = _equippedWeaponsInternal.ContainsKey(slotType);
            }
            else if (slotType.IsAccessory() || slotType.IsArmor())
            {
                occupied = _equippedArmorInternal.ContainsKey(slotType);
            }

            return occupied;
        }

        private IReadOnlyDictionary<EEquipmentSlotType, ContractItemWeapon> GetWeaponsInternal()
        {
            Dictionary<EEquipmentSlotType, ContractItemWeapon> weapons = new Dictionary<EEquipmentSlotType, ContractItemWeapon>();
            foreach (KeyValuePair<EEquipmentSlotType, ContractStorageItem> weaponsInternal in _equippedWeaponsInternal)
            {
                weapons.Add(weaponsInternal.Key, (ContractItemWeapon)weaponsInternal.Value.Item);
            }

            return weapons;
        }

        private IReadOnlyDictionary<EEquipmentSlotType, ContractItemArmor> GetArmorInternal()
        {
            Dictionary<EEquipmentSlotType, ContractItemArmor> armor = new Dictionary<EEquipmentSlotType, ContractItemArmor>();
            foreach (KeyValuePair<EEquipmentSlotType, ContractStorageItem> armorInternal in _equippedArmorInternal)
            {
                armor.Add(armorInternal.Key, (ContractItemArmor)armorInternal.Value.Item);
            }

            return armor;
        }
    }
}