using ARAWorks.Base.Enums;
using ARAWorks.Base.Timer;
using ARAWorks.StatusEffectSystem.Statuses.Utility;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.StatusEffectSystem.Statuses
{
    [Serializable]
    public class StatusEffectHealOverTime : StatusEffectBase
    {
        public override int StackMax => 1;
        public override bool CanAddModifier => true;
        public override bool CanRemoveModifier => false;
        public override EStatusEffectType EffectType => EStatusEffectType.Buff | EStatusEffectType.Heal;
        public override Timer EffectTimer => HealthOverTime.IntervalTimer;

        [ShowInInspector] public StatusEffectUtilityIntervalOverTime HealthOverTime { get; set; }
        private bool _isFinished = false;

        public StatusEffectHealOverTime(float maxHealth, float maxTime, float interval = 1)
        {
            HealthOverTime = new StatusEffectUtilityIntervalOverTime(OnHealthTimerFinished, maxHealth, maxTime, interval);
            HealthOverTime.IntervalUpdate += OnHealingOverTImeInterval;
            _statusEffectSprite = null;
        }

        public override void StartEffect()
        {
            _isFinished = false;
            HealthOverTime.Start();
        }

        public override void EndEffect()
        {
            HealthOverTime.Reset();
            _isFinished = false;
        }

        public override void AddModifier(StatusEffectBase effect)
        {
            StatusEffectHealOverTime hot = (StatusEffectHealOverTime)effect;

            if (hot.HealthOverTime.MaxValue > HealthOverTime.MaxValue)
            {
                HealthOverTime.Reset();
                HealthOverTime.IntervalUpdate -= OnHealingOverTImeInterval;
                HealthOverTime = new StatusEffectUtilityIntervalOverTime(OnHealthTimerFinished, hot.HealthOverTime.MaxValue, hot.HealthOverTime.MaxTime, hot.HealthOverTime.Interval);
                HealthOverTime.IntervalUpdate += OnHealingOverTImeInterval;
                StartEffect();
            }
            else if (hot.HealthOverTime.MaxValue == HealthOverTime.MaxValue)
            {
                HealthOverTime.Reset();
                StartEffect();
            }
        }

        public override bool Update()
        {
            HealthOverTime.Update();
            return _isFinished;
        }

        /// <summary>
        /// Triggers what should happen when the Health Timer finishes.
        /// </summary>
        protected virtual void OnHealthTimerFinished()
        {
            HealthOverTime.Reset();
            _isFinished = true;
        }

        /// <summary>
        /// Use to trigger how much to heal over every tick.
        /// ex. character.Health += HealthOverTime.ValuePerTick;
        /// </summary>
        protected virtual void OnHealingOverTImeInterval()
        { }
    }
}