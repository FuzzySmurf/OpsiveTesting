using ARAWorks.Base.Contracts;

namespace ARAWorks.Interstice
{
    public interface IItemInterstice
    {
        /// <summary>
        /// Checks if the given item is allowed to be in the given slot.
        /// </summary>
        /// <param name="item">The item we're checking.</param>
        /// <param name="slotNumber">The slot we're checking against.</param>
        /// <returns>Returns TRUE if the item is allowed to be in the given slot</returns>
        bool IsItemValid(ContractItem item, int slotNumber);

        /// <summary>
        /// Checks if the given item can stack with an item in the given slot.
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <param name="slotNumber">The slot to check against</param>
        /// <returns>Returns TRUE if there is an item in the given slot and the given item can stack on it</returns>
        bool CanStack(ContractItem item, int slotNumber);

        /// <summary>
        /// Inserts the given item into the given slot.
        /// </summary>
        /// <param name="item">The item to insert</param>
        /// <param name="slotNumber">The slot to insert the item into</param>
        /// <returns>Returns an overflow containing any items that could not be inserted.</returns>
        InventoryItemOverFlowResults InsertItem(ContractItem item, int slotNumber);

        /// <summary>
        /// Removes the item in the given slot (if applicable).
        /// </summary>
        /// <param name="slotNumber">The slot to remove any item from.</param>
        /// <returns>Returns TRUE if the given slot is now empty</returns>
        bool RemoveItem(int slotNumber);

        /// <summary>
        /// Attempts to get an item in the given slot.
        /// </summary>
        /// <param name="slotNumber">The slot to look for an item in</param>
        /// <returns>Returns the item in the given slot or null if none exists</returns>
        ContractItem GetItemInSlot(int slotNumber);
    }
}