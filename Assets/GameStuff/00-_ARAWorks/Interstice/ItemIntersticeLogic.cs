using ARAWorks.Base.Contracts;
using ARAWorks.Base.Extensions;
using UnityEngine;

namespace ARAWorks.Interstice
{
    /// <summary>
    /// An extension class that handles item interstice logic from wherever item swapping is needed.
    /// </summary>
    public static class ItemIntersticeLogic
    {
        public static InventoryItemOverFlowResults SwapIntersticeLogic(IItemInterstice source, IItemInterstice destination, ContractItem itemTransferring, int destinationSlot, int sourceSlot)
        {
            if (InterfaceHelper.IsNull(source) == true || InterfaceHelper.IsNull(destination) == true
                || (source.Equals(destination) && sourceSlot == destinationSlot))
                return new InventoryItemOverFlowResults(false);

            InventoryItemOverFlowResults itemsPastLimit = null;
            ContractItem itemAtDestination = destination.GetItemInSlot(destinationSlot);
            if (itemAtDestination == null)
            {
                if (destination.IsItemValid(itemTransferring, destinationSlot) == false) return new InventoryItemOverFlowResults(false);

                // Move the item to the destination slot.
                source.RemoveItem(sourceSlot);
                itemsPastLimit = destination.InsertItem(itemTransferring, destinationSlot);
                return itemsPastLimit;
            }

            if (destination.CanStack(itemTransferring, destinationSlot) == true)
            {
                // Add the item onto the quantity of item at destination
                source.RemoveItem(sourceSlot);
                itemsPastLimit = destination.InsertItem(itemTransferring, destinationSlot);
                return itemsPastLimit;
            }

            bool canSwapItems = source.IsItemValid(itemAtDestination, sourceSlot) && destination.IsItemValid(itemTransferring, destinationSlot);
            if (canSwapItems == false) return new InventoryItemOverFlowResults(false);

            // Swap item at destination with item dropped on it
            source.RemoveItem(sourceSlot);
            destination.RemoveItem(destinationSlot);
            itemsPastLimit = destination.InsertItem(itemTransferring, destinationSlot);
            source.InsertItem(itemAtDestination, sourceSlot);
            return itemsPastLimit;
        }
    }
}