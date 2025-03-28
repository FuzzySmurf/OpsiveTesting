using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Base.Contracts
{
    public class ContractDamageInfo
    {
        public ContractDamageValues rawDamage;
        public ContractDamageValues calculatedDamage;
        public GameObject target;

        public ContractDamageInfo(ContractDamageValues rawDamage, ContractDamageValues calculatedDamage, GameObject target)
        {
            this.rawDamage = rawDamage;
            this.calculatedDamage = calculatedDamage;
            this.target = target;
        }
    }
}