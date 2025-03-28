using ARAWorks.Base.Contracts;
using ARAWorks.Damage.UIToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Damage
{
    public abstract class DamageHandlerBase : MonoBehaviour
    {
        public event Action<ContractDamageValues> OnDamaged;

        [SerializeField] protected bool _showDamageNumbers = true;
        [SerializeField] protected bool _isDebug = false;

        protected abstract Color debugColor { get; }

        protected virtual void Start()
        {
        }

        /// <summary>
        /// Receive and process damage with resistance.
        /// </summary>
        /// <param name="damage"></param>
        public virtual void ProcessDamage(ContractDamageValues damage)
        {
            ContractDamageValues newDamage = ProcessDamageCaluculation(damage);

            if (_isDebug == true)
            {
                PrintDebug(newDamage);
            }

            OnDamaged?.Invoke(newDamage);
        }

        /// <summary>
        /// Receive and process damage without resistance.
        /// </summary>
        /// <param name="damage"></param>
        public void ProcessDamageRaw(ContractDamageValues damage)
        {
            if (_isDebug == true)
            {
                PrintDebug(damage);
            }

            OnDamaged?.Invoke(damage);
        }

        protected virtual ContractDamageValues ProcessDamageCaluculation(ContractDamageValues damage) => damage;

        protected virtual string GetDebugMessage(ContractDamageValues damage) => "";

        protected virtual void PrintDebug(ContractDamageValues damage)
        {
            Debug.Log($"<color={debugColor}>{name}</color> is taking <color=red>{damage.Value}</color> [{damage.DamageType}] damage. {GetDebugMessage(damage)}");
        }
    }
}
