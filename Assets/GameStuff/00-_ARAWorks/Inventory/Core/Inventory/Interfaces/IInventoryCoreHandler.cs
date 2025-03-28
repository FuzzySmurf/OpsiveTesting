using ARAWorks.Base.Contracts;
using ARAWorks.Inventory.Enums;

namespace ARAWorks.Inventory
{
    public interface IInventoryCoreHandler
    {
        /// <summary>
        /// Checks where this it is located based on its SlotNumber.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        EEnumCharacterStorageType GetItemStorageLocation(ContractItem item);
    }
}
