using ARAWorks.Base.Contracts;
using ARAWorks.Interstice;
using ARAWorks.Inventory.Contracts;
using System;
using System.Collections.Generic;

namespace ARAWorks.Inventory
{
    public interface IInventoryDataHandler
    {
        /// <summary>
        /// Event when an item has been added. Returns the item that was added and its slot number.
        /// </summary>
        event Action<ContractItem, int> ItemAdded;

        /// <summary>
        /// Event when an item has been removed. Returns the item that was removed and its slot number.
        /// </summary>
        event Action<ContractItem, int> ItemRemoved;

        /// <summary>
        /// Event when an item has been updated. Returns the item that was updated and its slot number.
        /// </summary>
        event Action<ContractItem, int> ItemUpdatedInventory;

        /// <summary>
        /// A readonly collection of all items currently in the inventory.
        /// </summary>
        public IReadOnlyDictionary<int, ContractItem> Inventory { get; }

        /// <summary>
        /// The maximum stack amount for items.
        /// </summary>
        public int MaxStackAmount { get; set; }

        /// <summary>
        /// The maximum slot amount of items.
        /// </summary>
        public int MaxSlotAmount { get; set; }

        /// <summary>
        /// Is there an empty inventory slot?
        /// </summary>
        public bool HasEmptyInventorySlot { get; }

        /// <summary>
        /// Handles all swap logic going to and from the inventory slots
        /// </summary>
        IItemInterstice InventoryInterstice { get; }

        /// <summary>
        /// Used to add Items to the inventory as they are collected. If an item is stackable and the inventory contains the same item type, this will
        /// first transfer the quantity of inventoryItem to any item of the same type until they are all full or inventoryItem is empty. If any remiander is
        /// left and it is more then the max stack amount, this will create new stacks to distribute the quantity until no new stacks are needed or the inventory has run out of room.
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <returns>Returns an InventoryItemOverFlowResults which contains any items that failed being added to the inventory.</returns>
        InventoryItemOverFlowResults AddItem(ContractItem inventoryItem);

        /// <summary>
        /// Used to add an item to a specific slot. If that slot is occupied, if both items are the same type, and if they are stackable this will transfer the quantity from
        /// the inventoryItem to the item in that slot up to the items max stack amount.
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <param name="slotNumber"></param>
        /// <returns>Returns an InventoryItemOverFlowResults which contains any items that failed being added to the inventory.</returns>
        InventoryItemOverFlowResults AddItem(ContractItem inventoryItem, int slotNumber);

        /// <summary>
        /// Used to update an item in the inventory and database.
        /// </summary>
        /// <param name="inventoryItem"></param>
        void UpdateItem(ContractItem inventoryItem);

        /// <summary>
        /// Used to remove an item from a specific slot.
        /// </summary>
        /// <param name="slotNumber"></param>
        /// <returns>True if successful, false otherwise.</returns> 
        bool RemoveItem(int slotNumber);

        /// <summary>
        /// Used to remove a specific item.
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <returns>True if successful, false otherwise.</returns>
        bool RemoveItem(ContractItem inventoryItem);


        /// <summary>
        /// Removes a quantity amount from a stackable item. If the item falls at or below 0, it will be removed.
        /// </summary>
        /// <param name="slotNumber"></param>
        /// <param name="amount"></param>
        /// <returns>True if successful, false otherwise.</returns>
        bool RemoveQuantity(int slotNumber, int amount);

        /// <summary>
        /// Removes a quantity amount from a stackable item. If the item falls at or below 0, it will be removed.
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <param name="amount"></param>
        /// <returns>True if successful, false otherwise.</returns>
        bool RemoveQuantity(ContractItem inventoryItem, int amount);

        /// <summary>
        /// Checks if an item can stack with an existing provided inventory item.
        /// </summary>
        /// <param name="newItem"></param>
        /// <param name="slotNumber"></param>
        /// <returns>True if newItem can stack with the item in the provided slotNumber.</returns>
        bool CanStack(ContractItem newItem, int slotNumber);

        /// <summary>
        /// Checks if an item can stack with an existing provided inventory item.
        /// </summary>
        /// <param name="newItem"></param>
        /// <param name="itemInInventory"></param>
        /// <returns>rue if newItem can stack with itemInInventory.</returns>
        bool CanStack(ContractItem newItem, ContractItem itemInInventory);

        /// <summary>
        /// Used to get an item from the inventory collection.
        /// </summary>
        /// <param name="slotID"></param>
        /// <returns></returns>
        ContractItem GetItem(int slotID);

        /// <summary>
        /// Used to get a stored items slot number from the inventory colletion.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int GetItemSlotNumber(ContractItem item);

        /// <summary>
        /// Used to get an empty inventory slot.
        /// </summary>
        /// <returns>-1 if no empty spot is found, or 1 to MaxSlotAmount</returns>
        int GetEmptySlot();

        /// <summary>
        /// Used to check if an inventory slot is occupied.
        /// </summary>
        /// <param name="slotNumber"></param>
        /// <returns></returns>
        bool IsInventorySlotOccupied(int slotNumber);

        /// <summary>
        /// Used to check if an item can be added to a specific slot.
        /// </summary>
        /// <param name="slotNumber"></param>
        /// <param name="inventoryItem"></param>
        /// <returns></returns>
        bool CanAddItem(ContractItem item, int slotNumber);

        /// <summary>
        /// Used to check if an item can be removed from a specific slot.
        /// </summary>
        /// <param name="slotNumber"></param>
        /// <returns></returns>
        bool CanRemoveItem(int slotNumber);
    }
}