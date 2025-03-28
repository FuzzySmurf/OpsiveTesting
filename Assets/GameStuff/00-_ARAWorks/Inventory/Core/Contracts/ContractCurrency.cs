using ARAWorks.Inventory.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Inventory.Contracts
{
    public class ContractCurrency
    {
        public ECurrencyType CurrencyType { get; set; }
        public long Amount { get; set; }
    }
}
