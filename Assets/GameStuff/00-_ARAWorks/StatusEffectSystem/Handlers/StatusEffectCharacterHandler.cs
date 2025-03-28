using System;
using System.Collections.Generic;
using ARAWorks.Base.Enums;
using ARAWorks.StatusEffectSystem.Statuses;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ARAWorks.StatusEffectSystem.Handlers
{
    public class StatusEffectCharacterHandler : MonoBehaviour, IStatusEffectHandler
    {
        /// <summary>
        /// Blocks the agent from being able to move. WARNING: Do not use BlockMovement in an update loop
        /// </summary>
        public bool IsMovementBlocked
        {
            get
            {
                return _blockMovementCount > 0;
            }
            set
            {
                if (value == true)
                {
                    _blockMovementCount++;
                }
                else if (_blockMovementCount > 0)
                {
                    _blockMovementCount--;
                }
            }
        }

        public event Action<StatusEffectBase, bool> OnEffectActive;

        private int _blockMovementCount = 0;
        // public StatusEffectType removalTest;

        [ReadOnly, ShowInInspector] private List<StatusEffectBase> _effects = new List<StatusEffectBase>();

        private void Update()
        {
            //Update backwards because if the state of an effect returns that it is finished, we will have to remove it.
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                if (_effects[i].Update() == true)
                {
                    RemoveEffect(_effects[i]);
                }
            }
        }

        public List<StatusEffectBase> GetStatusEffectList()
        {
            return _effects;
        }

        public List<StatusEffectBase> GetStatusEffectsByType(EStatusEffectType seType)
        {
            List<StatusEffectBase> list = new List<StatusEffectBase>();

            foreach(StatusEffectBase status in _effects)
            {
                if ((status.EffectType & seType) != 0)
                {
                    list.Add(status);
                }
            }

            return list;
        }

        public bool HasStatusEffect(EStatusEffectType seType)
        {
            foreach (StatusEffectBase status in _effects)
            {
                if ((status.EffectType & seType) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasStatusEffect(Type type)
        {
            foreach (StatusEffectBase status in _effects)
            {
                if (status.GetType() == type)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasStatusEffect(StatusEffectBase effect)
        {
            return HasStatusEffect(effect.GetType());
        }

        public bool CanActivateEffect(Type type)
        {
            foreach (StatusEffectBase status in _effects)
            {
                if (status.GetType() == type)
                {
                    return status.CanActivate();
                }
            }

            return true;
        }

        public void AddEffect(StatusEffectBase effect)
        {
            //Attempt to find an existing effect
            Type effectType = effect.GetType();
            StatusEffectBase cachedEffect = _effects.Find(x => x.GetType() == effectType);

            if (cachedEffect != null)
            {
                //If the effect can add a modifier then add it and return, otherwise just return.
                if (cachedEffect.CanAddModifier)
                {
                    cachedEffect.AddModifier(effect);
                }
                return;
            }

            //Lastly, add the effect and start it.
            _effects.Add(effect);
            effect.SetHandler(this);
            effect.StartEffect();
            OnEffectActive?.Invoke(effect, true);
        }

        public void RemoveEffect(StatusEffectBase effect)
        {
            Type effectType = effect.GetType();
            RemoveEffect(effectType);
        }

        public void RemoveEffect(Type effectType)
        {
            //Attempt to find an existing effect
            StatusEffectBase cachedEffect = _effects.Find(x => x.GetType() == effectType);

            if (cachedEffect == null) return;

            //Check if we can remove a modifier
            if (cachedEffect.CanRemoveModifier == true)
            {
                cachedEffect.RemoveModifier();
                return;
            }

            //Other wise, end the effect and remove it.
            cachedEffect.EndEffect();
            _effects.Remove(cachedEffect);
            OnEffectActive?.Invoke(cachedEffect, false);
        }

        public void RemoveAllEffects()
        {
            //End all effects and clear the list
            foreach (StatusEffectBase effect in _effects)
            {
                effect.EndEffect();
            }
            _effects.Clear();
        }



        public void RemoveEffectsByType(EStatusEffectType seType)
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                if ((_effects[i].EffectType & seType) != 0)
                {
                    _effects[i].EndEffect();
                    _effects.RemoveAt(i);
                }
            }
        }

    }
}