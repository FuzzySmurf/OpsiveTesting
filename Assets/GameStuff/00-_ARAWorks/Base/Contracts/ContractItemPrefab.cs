using System;
using ARAWorks.Base.Enums;

namespace ARAWorks.Base.Contracts
{
    public class ContractItemPrefab
    {
        public string globalItemID { get; set; }
        public string globalItemPrefabID { get; set; }
        public string name { get; set; }
        public string itemAddress { get; set; }
        public EEquipmentSlotType? slotType { get; set; }
    }
}
