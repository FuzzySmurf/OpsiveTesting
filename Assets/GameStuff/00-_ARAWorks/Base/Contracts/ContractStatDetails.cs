using System;
using System.Collections.Generic;
using ARAWorks.Base.Enums;

namespace ARAWorks.Base.Contracts
{
    public class ContractStatDetails
    {
        public bool isSet { get; private set; }

        public string name { get; set; }

        public int level { get; set; }

        public bool hasLevel { get; set; }

        public string entityID { get; set; }

        public string entityRef { get; set; }

        public ERarityType TypeRarity { get; set; }

        public ContractAttributes Attributes { get; set; }

        public ContractStatsCore CoreStats { get; set; }

        public List<ContractStatUtility> StatUtilities { get; set; }

        public ContractStatDetails(bool isSetup = true)
        {
            isSet = isSetup;
            Attributes = new ContractAttributes();
            CoreStats = new ContractStatsCore();
            StatUtilities = new List<ContractStatUtility>();
        }

        public ContractStatDetails(string nme)
            : this()
        {
            name = nme;
        }

        public ContractStatDetails(string entityID, string entityRef, string nme, int lvl, bool hasLvl)
            : this()
        {
            this.entityID = entityID;
            this.entityRef = entityRef;
            name = nme;
            level = lvl;
            hasLevel = hasLvl;
        }
    }
}