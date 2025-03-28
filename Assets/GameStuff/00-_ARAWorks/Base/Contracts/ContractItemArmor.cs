using System;
using ARAWorks.Base.Enums;

namespace ARAWorks.Base.Contracts
{
    public class ContractItemArmor : ContractItemEquipable
    {
        public string GlobalItemArmorID { get; set; }
        public EEquipmentOptionType EquipmentOption { get; set; }
    }
}