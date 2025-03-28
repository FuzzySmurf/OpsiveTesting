using System;
using ARAWorks.Base.Enums;

namespace ARAWorks.Base.Contracts
{
    public class ContractStatUtility
    {
        public ContractStatUtility(EStatTypes ns, float v)
        {
            StatType = ns;
            Value = v;
        }

        public ContractStatUtility()
        {

        }

        public EStatTypes StatType { get; set; }
        public float Value { get; set; }

        public override string ToString()
        {
            return
            $@"--ContractStatUtility--
            StatType: {StatType}
            Value: {Value}";
        }
    }

}