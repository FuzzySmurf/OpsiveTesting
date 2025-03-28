using ARAWorks.Base.Contracts;
using ARAWorks.Inventory.Enums;

namespace ARAWorks.Inventory.Contracts
{
    public class ContractStorageItem
    {
        public ContractItem Item { get; set; }
        public string CharacterInventoryIDRef { get; set; }
        public int SlotNumber { get; set; }
        public EEnumCharacterStorageType TypeCharacterStorage { get; set; }

        public override string ToString()
        {
            return $@"Item Name: {Item.Name}
                CharInventoryIDRef: {CharacterInventoryIDRef}
                Slot Number: {SlotNumber}
                Storage: {TypeCharacterStorage}";
        }
    }
}