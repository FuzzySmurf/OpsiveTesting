using ARAWorks.Base.Contracts;
using System.Collections.Generic;

namespace ARAWorks.Interstice
{
    public class InventoryItemOverFlowResults
    {
        public IReadOnlyList<ContractItem> overflowItems { get; private set; }
        public bool itemAddedSuccesfully { get; private set; }
        public bool hasOverflow => overflowItems != null && overflowItems.Count > 0;

        public InventoryItemOverFlowResults(bool itemAddedSuccesfully)
        {
            this.overflowItems = new List<ContractItem>();
            this.itemAddedSuccesfully = itemAddedSuccesfully;
        }

        public InventoryItemOverFlowResults(bool itemAddedSuccesfully, List<ContractItem> overflowItems)
        {
            this.overflowItems = overflowItems;
            this.itemAddedSuccesfully = itemAddedSuccesfully;
        }

        public InventoryItemOverFlowResults(bool itemAddedSuccesfully, ContractItem overflowItem)
        {
            List<ContractItem> items = new List<ContractItem>();
            this.itemAddedSuccesfully = itemAddedSuccesfully;

            items.Add(overflowItem);
            overflowItems = items;
        }
    }
}
