using System;
using UnityEngine;
using ARAWorks.Base.Enums;

namespace ARAWorks.Base.Contracts
{
    public class ContractItemEquipable : ContractItem
    {

        public int level { get; set; }

        public ERarityType rarityType { get; set; }

        public ContractStatDetails statDetailsLeveled { get; set; }

        public EEquipmentSlotType TypeEquipmentSlot { get; set; }
    }
}
