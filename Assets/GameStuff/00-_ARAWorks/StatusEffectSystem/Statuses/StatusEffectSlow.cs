using ARAWorks.Base.Enums;
using ARAWorks.Base.Timer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ARAWorks.StatusEffectSystem.Statuses
{
    public class StatusEffectSlow : StatusEffectBase
    {
        public override int StackMax => 1;
        public override bool CanAddModifier => false;
        public override bool CanRemoveModifier => false;
        public override EStatusEffectType EffectType => EStatusEffectType.Debuff | EStatusEffectType.Movement;
        public override Timer EffectTimer => _slowTimer;

        protected Timer _slowTimer;

        [ShowInInspector] protected float _speedReduction;
        protected bool _isFinished = false;

        public StatusEffectSlow(float slowTime, float speedReduction)
        {
            _speedReduction = speedReduction;
            _slowTimer = new Timer(slowTime, OnSlowFinished);
            _statusEffectSprite = null;
        }

        public override void StartEffect()
        {
            _isFinished = false;
            _slowTimer.Restart();
        }

        public override void EndEffect()
        { }

        public override bool Update()
        {
            return _isFinished;
        }

        protected virtual void OnSlowFinished()
        {
            _isFinished = true;
        }
    }
}
