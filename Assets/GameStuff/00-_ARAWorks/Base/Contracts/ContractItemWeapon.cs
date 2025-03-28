using System;
using System.Collections.Generic;
using ARAWorks.Base.Enums;

namespace ARAWorks.Base.Contracts
{
    public class ContractItemWeapon : ContractItemEquipable
    {
        public string GlobalItemWeaponID { get; set; }

        public EEquipmentOptionType EquipmentOption { get; set; }

        public List<ContractDamageValues> Damages { get; set; }

        public EWeaponRangeType WeaponType { get; set; }

        public int SoulExperience { get; set; }

        public ContractItemWeapon()
        {
            Damages = new List<ContractDamageValues>();
        }
    }
}