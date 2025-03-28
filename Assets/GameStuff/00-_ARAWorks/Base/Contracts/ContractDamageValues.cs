using System;
using ARAWorks.Base.Enums;

namespace ARAWorks.Base.Contracts
{
    public class ContractDamageValues
    {
        public float Value => _value;
        public EDamageType ModifierType => _modiferType;
        public EDamageType DamageType => _damageType;

        private float _value;

        private EDamageType _modiferType;

        private EDamageType _damageType;

        public ContractDamageValues(EDamageType modiferType, EDamageType damageType, float value)
        {
            _value = value;
            _modiferType = modiferType;
            _damageType = damageType;
        }

        public ContractDamageValues(EDamageType type, float amount)
            : this(EDamageType.Null, type, amount)
        {   }
    }
}