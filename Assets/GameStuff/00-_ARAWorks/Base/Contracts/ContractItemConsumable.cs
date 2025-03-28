using ARAWorks.Base.Enums;
using System.Collections;
using System.Collections.Generic;

namespace ARAWorks.Base.Contracts
{
    public class ContractItemConsumable : ContractItem
    {
        public List<ContractDamageValues> damageTaken { get; set; }
        public List<ContractDamageValues> damageBuff { get; set; }
        public Dictionary<EAttributeTypes, int> attributeBuffs { get; set; }
        public Dictionary<EAttributeTypes, int> attributeDebuffs { get; set; }
        public Dictionary<EStatTypes, float> statBuffs { get; set; }
        public Dictionary<EStatTypes, float> statDebuffs { get; set; }
        public bool isEffectOverTime { get; set; }
        public float effectOverTimeDuration { get; set; }
        public EStatusEffectType effectType { get; set; }

        public ContractItemConsumable()
        {
            damageTaken = new List<ContractDamageValues>();
            damageBuff = new List<ContractDamageValues>();
            attributeBuffs = new Dictionary<EAttributeTypes, int>();
            attributeDebuffs = new Dictionary<EAttributeTypes, int>();
            statBuffs = new Dictionary<EStatTypes, float>();
            statDebuffs = new Dictionary<EStatTypes, float>();
        }
    }
}