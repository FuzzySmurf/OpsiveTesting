using ARAWorks.Base.Contracts;
using ARAWorks.Base.Interfaces;
using ARAWorks.Inventory.Contracts;
using ARAWorks.Inventory.Enums;

namespace ARAWorks.Inventory
{
    public class InventoryCoreHandler : IInventoryCoreHandler
    {
        public InventoryDataHandler InventoryDataHandler { get; private set; }
        public EquipmentDataHandler EquipmentDataHandler { get; private set; }

        private IDBItemInventory _itemService;


        public InventoryCoreHandler(string characterID, IDBItemInventory itemService, IDBItemService itemObjectService)
        {
            _itemService = itemService;
            InventoryDataHandler = new InventoryDataHandler(characterID, itemService, itemObjectService);
            EquipmentDataHandler = new EquipmentDataHandler(characterID, itemService);
        }


        public EEnumCharacterStorageType GetItemStorageLocation(ContractItem item)
        {
            ContractStorageItem storageItem = GetContractStorageItem(item);

            if (storageItem == null) return EEnumCharacterStorageType.None;

            return storageItem.TypeCharacterStorage;
        }

        private ContractStorageItem GetContractStorageItem(ContractItem inventoryItem)
        {
            if (inventoryItem is ContractItemEquipable equipable)
            {
                ContractStorageItem storageItem = EquipmentDataHandler.GetStorageItem(equipable);

                if (storageItem != null)
                {
                    return storageItem;
                }
            }

            return InventoryDataHandler.GetStorageItem(inventoryItem);
        }
    }
}
