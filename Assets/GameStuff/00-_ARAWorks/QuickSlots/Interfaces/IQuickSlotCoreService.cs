using ARAWorks.Base.Contracts;
using ARAWorks.Interstice;
using ARAWorks.Inventory;
using System;
using System.Collections.Generic;

namespace ARAWorks.QuickSlots
{
    public interface IQuickSlotCoreService
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
        event Action<ContractItem, int> ItemUpdated;

        /// <summary>
        /// A readonly collection of all items currently in the quick slots.
        /// </summary>
        IReadOnlyDictionary<int, ContractItem> QuickSlotItems { get; }

        /// <summary>
        /// The maximum stack amount for items.
        /// </summary>
        int MaxStackAmount { get; set; }

        /// <summary>
        /// The maximum slot amount of items.
        /// </summary>
        int MaxSlotAmount { get; set; }

        /// <summary>
        /// Is there an empty inventory slot?
        /// </summary>
        bool HasEmptySlot { get; }

        /// <summary>
        /// Handles all swap logic going to and from the inventory slots
        /// </summary>
        IItemInterstice QuickSlotInterstice { get; }

        /// <summary>
        /// Used to add Items to the quick slots. If an item is stackable and the quick slots contains one of the same item type, this will
        /// first transfer the quantity of item to any item of the same type until they are all full or item quantity is empty. If any remiander is
        /// left and it is more than the max stack amount, this will create new stacks to distribute the quantity until no new stacks are needed or the quick slots have run out of room.
        /// </summary>
        /// <param name="itemToAdd"></param>
        /// <returns>Returns an InventoryItemOverFlowResults which contains any items that failed being added to the quick slots.</returns>
        InventoryItemOverFlowResults AddItem(ContractItem itemToAdd);

        /// <summary>
        /// Used to add an item to a specific slot. If that slot is occupied, if both items are the same type, and if they are stackable this will transfer the quantity from
        /// the itemToAdd to the item in that slot up to the items max stack amount.
        /// </summary>
        /// <param name="itemToAdd"></param>
        /// <param name="slotNumber"></param>
        /// <returns>Returns an InventoryItemOverFlowResults which contains any items that failed being added to the quick slots.</returns>
        InventoryItemOverFlowResults AddItem(ContractItem itemToAdd, int slotNumber);

        /// <summary>
        /// Attempts to stack the item with any other occurance of the same type.
        /// </summary>
        /// <param name="itemToStack"></param>
        /// <returns>If theres any leftovers, returns a InventoryItemOverFlowResults marked as false with the leftovers. Otherwise return true.</returns>
        InventoryItemOverFlowResults AddItemStack(ContractItem itemToStack);

        /// <summary>
        /// Used to update an item in the quick slots and database.
        /// </summary>
        /// <param name="item"></param>
        void UpdateItem(ContractItem item);

        /// <summary>
        /// Used to remove an item from a specific slot.
        /// </summary>
        /// <param name="slotNumber"></param>
        /// <returns>True if successful, false otherwise.</returns> 
        bool RemoveItem(int slotNumber);

        /// <summary>
        /// Used to remove a specific item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if successful, false otherwise.</returns>
        bool RemoveItem(ContractItem item);

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
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>True if successful, false otherwise.</returns>
        bool RemoveQuantity(ContractItem item, int amount);

        /// <summary>
        /// Checks if an item can stack with an existing provided item.
        /// </summary>
        /// <param name="newItem"></param>
        /// <param name="slotNumber"></param>
        /// <returns>True if newItem can stack with the item in the provided slotNumber.</returns>
        bool CanStack(ContractItem newItem, int slotNumber);

        /// <summary>
        /// Checks if an item can stack with an existing provided item.
        /// </summary>
        /// <param name="newItem"></param>
        /// <param name="itemInQuickSlot"></param>
        /// <returns>True if newItem can stack with itemInQuickSlot.</returns>
        bool CanStack(ContractItem newItem, ContractItem itemInQuickSlot);

        /// <summary>
        /// Used to get an item from the quick slots collection.
        /// </summary>
        /// <param name="slotID"></param>
        /// <returns></returns>
        ContractItem GetItem(int slotID);

        /// <summary>
        /// Gets the items slot number if it exists, otherwise returns -1.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int GetItemSlotNumber(ContractItem item);

        /// <summary>
        /// Used to check if a quick slot is occupied.
        /// </summary>
        /// <param name="slotNumber"></param>
        /// <returns></returns>
        bool IsSlotOccupied(int slotNumber);

        /// <summary>
        /// Used to check if an item can be added to a specific slot.
        /// </summary>
        /// <param name="slotNumber"></param>
        /// <param name="item"></param>
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