using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("Used to check if we can search for a character or not, after a set time.")]
    public class CanSetBoolValueAfterXTime : EnemyConditional
    {
        public SharedVariable<bool> canSearchCharacter;
        [Tooltip("How long should we delay for?")]
        public SharedVariable<float> maxDelayUntilSearchAgain = 1.0f;
        private float _curTime;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            if (!CanSearchToTrue()) return TaskStatus.Failure;

            return base.OnUpdate();
        }

        private bool CanSearchToTrue()
        {
            if (canSearchCharacter.Value) return true;

            if (_curTime < maxDelayUntilSearchAgain.Value)
            {
                _curTime = _curTime + Time.deltaTime;
                return false;
            }

            canSearchCharacter.Value = true;
            _curTime = 0;
            return true;
        }
    }
}