using System;
using UnityEngine;
using ARAWorks.Base.Timer;
using Sirenix.OdinInspector;
using ARAWorks.Base.Enums;
using ARAWorks.StatusEffectSystem.Handlers;

namespace ARAWorks.StatusEffectSystem.Statuses
{
    [Serializable]
    public class StatusEffectKnockback : StatusEffectBase
    {
        [Title("Knockback")]
        public override int StackMax => 1;
        public override bool CanAddModifier => true;
        public override bool CanRemoveModifier => false;
        public override EStatusEffectType EffectType => EStatusEffectType.Debuff | EStatusEffectType.Movement;
        public override Timer EffectTimer => _stunTimer;

        protected Vector3 _direction;
        protected float _distance;
        protected float _knockbackTime;
        protected Timer _knockbackTimer;
        protected Timer _stunTimer;
        protected IStatusEffectHandler _statusEffectHandler;
        


        public StatusEffectKnockback(IStatusEffectHandler statusEffectHandler, Vector3 direction, float distance, float knockbackTime, float stunTIme)
        {
            _statusEffectHandler = statusEffectHandler;
            _knockbackTime = knockbackTime;
            _direction = direction;
            _distance = distance;

            _knockbackTimer = new Timer(_knockbackTime, false);
            _knockbackTimer.Finished += OnKnockbackFinished;
            _statusEffectSprite = null;

            _stunTimer = new Timer(stunTIme, false);
        }

        public override void StartEffect()
        {
            _knockbackTimer.Start();
        }


        public override void EndEffect()
        { }

        public override void AddModifier(StatusEffectBase effect)
        {
            _knockbackTimer.Restart();
        }

        public override bool Update()
        {
            if (_knockbackTimer.IsRunning == true)
            {
                MoveTarget();
            }
            return _knockbackTimer.IsFinished && _stunTimer.IsFinished;
        }

        protected virtual void MoveTarget()
        { }

        protected virtual void OnKnockbackFinished()
        {
            _stunTimer.Start();
        }
    }
}