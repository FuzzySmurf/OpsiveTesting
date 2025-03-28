using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Contracts
{
    public class ContractLevelDetail
    {
        public string name { get; set; }
        public int buildIndex { get; set; }
        public int loadOrder { get; set; }

        public ContractLevelDetail()
        {
            buildIndex = -1;
        }
    }
}