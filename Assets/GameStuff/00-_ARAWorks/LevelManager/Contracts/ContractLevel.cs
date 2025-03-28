using ARAWorks.LevelManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Contracts
{
    public class ContractLevel
    {
        public string name { get; set; }
        public string levelImageAddress { get; set; }
        public ELevelType levelType { get; set; }
        public List<ContractLevelDetail> levels { get; set; }
    }
}
