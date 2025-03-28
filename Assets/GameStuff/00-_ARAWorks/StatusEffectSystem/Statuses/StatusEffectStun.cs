using System;
using ARAWorks.Base.Enums;
using ARAWorks.Base.Timer;
using ARAWorks.StatusEffectSystem.Handlers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ARAWorks.StatusEffectSystem.Statuses
{

    [Serializable]
    public class StatusEffectStun : StatusEffectBase
    {
        [Title("Stun")]
        public override int StackMax => 1;
        public override bool CanAddModifier => false;
        public override bool CanRemoveModifier => false;
        public override EStatusEffectType EffectType => EStatusEffectType.Debuff | EStatusEffectType.Movement;
        public override Timer EffectTimer => _stunTimer;

        public float StunTime => _stunTime;

        private Timer _stunTimer;
        private float _stunTime;
        private bool _isFinished = false;
        public Action OnCharacterStunned;

        public StatusEffectStun(float stunTime)
        {
            _stunTime = stunTime;
            _stunTimer = new Timer(_stunTime, OnStunFinished);
            _statusEffectSprite = null;
        }

        public override void StartEffect()
        {
            OnCharacterStunned?.Invoke();
            _stunTimer.Start();
        }

        public override void EndEffect()
        {
            _stunTimer.Stop();
            _isFinished = false;
        }

        public override void AddModifier(StatusEffectBase effect)
        {
            StatusEffectStun newStun = effect as StatusEffectStun;
            _stunTimer.Restart();
            _stunTimer.SetGoal(newStun.StunTime);
        }

        public override bool Update()
        {
            return _isFinished;
        }

        private void OnStunFinished()
        {
            _isFinished = true;
        }
    }
}