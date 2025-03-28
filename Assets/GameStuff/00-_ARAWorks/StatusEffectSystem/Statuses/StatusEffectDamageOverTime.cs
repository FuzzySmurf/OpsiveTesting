using ARAWorks.Base.Enums;
using ARAWorks.Base.Timer;
using ARAWorks.StatusEffectSystem.Statuses.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ARAWorks.StatusEffectSystem.Statuses
{
    public class StatusEffectDamageOverTime : StatusEffectBase
    {
        public override int StackMax => 1;
        public override bool CanAddModifier => true;
        public override bool CanRemoveModifier => StackCurrent > 1;
        public override EStatusEffectType EffectType => EStatusEffectType.Debuff | EStatusEffectType.DoT;
        public override Timer EffectTimer => DamageOverTime?.IntervalTimer;

        [ShowInInspector] public StatusEffectUtilityIntervalOverTime DamageOverTime { get; set; }

        protected bool _isFinished = false;
        protected float _damageValueCache = 0;

        public StatusEffectDamageOverTime(float maxDamage, float maxTime, float interval = 1)
        {
            _damageValueCache = maxDamage;
            _statusEffectSprite = null;

            DamageOverTime = new StatusEffectUtilityIntervalOverTime(OnDOTTimerFinished, maxDamage, maxTime, interval);
            DamageOverTime.IntervalUpdate += OnDamageOverTimeInterval;
        }

        public override void StartEffect()
        {
            _isFinished = false;
            StackCurrent = 1;
            DamageOverTime.Start();
        }

        public override void EndEffect()
        {
            DamageOverTime.Reset();
            _isFinished = false;
            StackCurrent = 1;
        }

        public override void AddModifier(StatusEffectBase effect)
        {
            // Reset the current DOT
            // TODO: There is a tick modifier that could overlap when the duration resets, and will need to be prevented.
            //          This could cause the 'ticks' for damage to be 'reset' everytime a modifier is added.
            _isFinished = false;

            if (StackCurrent < StackMax)
            {
                StackCurrent++;
                DamageOverTime.ModifyMaxValue(DamageOverTime.MaxValue + _damageValueCache);
            }

            TriggerModifier();
            DamageOverTime.Restart();
        }

        protected virtual void TriggerModifier()
        { }

        public override void RemoveModifier()
        {
            _isFinished = false;
            StackCurrent--;
            DamageOverTime.Restart();
        }

        public override bool Update()
        {
            DamageOverTime.Update();
            return _isFinished;
        }

        private void OnDOTTimerFinished()
        {
            DamageOverTime.Reset();
            _isFinished = true;
        }

        protected virtual void OnDamageOverTimeInterval()
        { }
    }
}