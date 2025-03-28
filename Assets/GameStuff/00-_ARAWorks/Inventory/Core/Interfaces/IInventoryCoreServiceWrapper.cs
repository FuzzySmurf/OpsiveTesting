using ARAWorks.Inventory.Currency;
using ARAWorks.Inventory.Contracts;
using ARAWorks.Base.Contracts;

namespace ARAWorks.Inventory
{
    public interface IInventoryCoreServiceWrapper : IInventoryCoreService, ICurrencyCoreService
    {
        bool IsInitialized { get; }

        void Initialize(string charcterID);
        void InitializeSceneData();

        /// <summary>
        /// Checks if an item meets the minimum level requiremnt to use.
        /// </summary>
        /// <param name="inventoryItem"></param>
        /// <returns>True if an item meets the level requirment, false otherwise.</returns>
        bool CheckItemMinimumLevelRequirement(ContractItemEquipable inventoryItem);
    }
}
