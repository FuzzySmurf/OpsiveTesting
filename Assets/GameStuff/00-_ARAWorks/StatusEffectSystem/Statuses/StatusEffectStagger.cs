using System;
using ARAWorks.Base.Enums;
using ARAWorks.Base.Timer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ARAWorks.StatusEffectSystem.Statuses
{
    [Serializable]
    public class StatusEffectStagger : StatusEffectBase
    {
        [Title("Stagger")]
        public override int StackMax => 1;
        public override bool CanAddModifier => false;
        public override bool CanRemoveModifier => false;
        public override EStatusEffectType EffectType => EStatusEffectType.Debuff | EStatusEffectType.Movement;
        public override Timer EffectTimer => _staggerTimer;

        private float _staggerTime;
        private Timer _staggerTimer;
        private bool _isFinished = false;

        public StatusEffectStagger(float staggerTime)
        {
            _staggerTime = staggerTime;
            _staggerTimer = new Timer(_staggerTime, OnStaggerFinish);
            _statusEffectSprite = null;
        }

        public override void StartEffect()
        {
            _staggerTimer.Restart();
        }

        public override void EndEffect()
        {
            _isFinished = false;
        }

        public override bool Update()
        {
            return _isFinished;
        }

        protected virtual void OnStaggerFinish()
        {
            _isFinished = true;
        }
    }
}