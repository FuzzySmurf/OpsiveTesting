using System;
using ARAWorks.Base.Timer;
using ARAWorks.Base.Enums;
using Sirenix.OdinInspector;
using ARAWorks.StatusEffectSystem.Handlers;
using UnityEngine;

namespace ARAWorks.StatusEffectSystem.Statuses
{

    [Serializable]
    public abstract class StatusEffectBase
    {
        [ShowInInspector] private string _name => GetName();
        [ShowInInspector] public abstract bool CanAddModifier { get; }
        [ShowInInspector] public abstract bool CanRemoveModifier { get; }
        [ShowInInspector] public abstract EStatusEffectType EffectType { get; }
        [ShowInInspector] public abstract Timer EffectTimer { get; }
        [ShowInInspector] public abstract int StackMax { get; }
        [ShowInInspector] public int StackCurrent
        {
            get { return _stackCurrent; }
            protected set
            {
                int prevStack = _stackCurrent;
                _stackCurrent = value;

                if (_stackCurrent > StackMax)
                    _stackCurrent = StackMax;
                else if (_stackCurrent < 0)
                    _stackCurrent = 0;

                if (_stackCurrent != prevStack)
                    modifiedStackCount?.Invoke(GetType(), _stackCurrent);
            }
        }

        [ShowInInspector] public Sprite StatusEffectSprite { get { return _statusEffectSprite; } }

        protected Sprite _statusEffectSprite;


        public event Action<Type, int> modifiedStackCount;

        protected IStatusEffectHandler _handler;


        private int _stackCurrent = 1;
        

        public void SetHandler(IStatusEffectHandler handler) => _handler = handler;

        /// <summary>
        /// Used to Initiate the effect. 
        /// Extend off this so your given character will receive the status effect.
        /// </summary>
        public abstract void StartEffect();

        /// <summary>
        /// Used to End the effect that was triggerred.
        /// Extend off this so your given character will end the status effect.
        /// </summary>
        public abstract void EndEffect();
        public abstract bool Update();
        
        public virtual bool CanActivate()
        {
            return true;
        }

        public virtual void AddModifier(StatusEffectBase effect) { }

        public virtual void RemoveModifier() { }

        private string GetName()
        {
            string name = GetType().Name;

            name = name.Replace("StatusEffect", "");

            return name;
        }
    
    }
}
