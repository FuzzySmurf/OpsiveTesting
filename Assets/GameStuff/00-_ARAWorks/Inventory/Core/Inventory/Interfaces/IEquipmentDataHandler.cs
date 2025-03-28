using ARAWorks.Base.Contracts;
using ARAWorks.Base.Enums;
using ARAWorks.Interstice;
using System;
using System.Collections.Generic;

namespace ARAWorks.Inventory
{
    public interface IEquipmentDataHandler
    {
        /// <summary>
        /// Event when an item has been equipped. Returns the item that was equipped and its equipment slot type.
        /// </summary>
        event Action<ContractItemEquipable, EEquipmentSlotType> ItemEquipped;

        /// <summary>
        /// Event when an item has been unequipped. Returns the item that was unequipped and its equipment slot type.
        /// </summary>
        event Action<ContractItemEquipable, EEquipmentSlotType> ItemUnequipped;

        /// <summary>
        /// Event when an item has been updated. Returns the item that was updated and its equipment slot type.
        /// </summary>
        event Action<ContractItemEquipable, EEquipmentSlotType> ItemUpdatedEquipment;

        /// <summary>
        /// A readonly collection of equipped weapons.
        /// </summary>
        IReadOnlyDictionary<EEquipmentSlotType, ContractItemWeapon> EquippedWeapons { get; }

        /// <summary>
        /// A readonly collection of equipped armor.
        /// </summary>
        IReadOnlyDictionary<EEquipmentSlotType, ContractItemArmor> EquippedArmor { get; }

        /// <summary>
        /// Get a readonly collection of all currently equipped items.
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<EEquipmentSlotType, ContractItemEquipable> GetAllEquipment();

        /// <summary>
        /// Handles all swap logic going to and from the equipment slots
        /// </summary>
        IItemInterstice EquipmentInterstice { get; }


        /// <summary>
        /// Used to equip an item.
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <returns>True if successful, false otherwise.</returns>
        bool EquipItem(ContractItemEquipable inventoryItem);

        /// <summary>
        /// Used to unequip an item.
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <returns>True if successful, false otherwise.</returns>
        bool UnequipItem(ContractItemEquipable inventoryItem);

        /// <summary>
        /// Used to update an item in the equipment and database.
        /// </summary>
        /// <param name="inventoryItem"></param>
        void UpdateItem(ContractItemEquipable inventoryItem);

        /// <summary>
        /// Used to get and item from the equipment container.
        /// </summary>
        /// <param name="slotType"></param>
        ContractItemEquipable GetEquippable(EEquipmentSlotType slotType);

        /// <summary>
        /// Used to check if an equipment slot is occupied.
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <returns></returns>
        bool IsEquipmentSlotOccupied(EEquipmentSlotType slotType);

        /// <summary>
        /// Used to check if an equipable can be equipped.
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <returns></returns>
        bool CanEquipItem(ContractItemEquipable inventoryItem);

        /// <summary>
        /// Used to check if an equipable can be unequipped.
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <returns></returns>
        bool CanUnequipItem(ContractItemEquipable inventoryItem);
    }
}
