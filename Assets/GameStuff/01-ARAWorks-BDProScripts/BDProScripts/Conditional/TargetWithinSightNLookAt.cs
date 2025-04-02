using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.UltimateCharacterController.Character;
using PolyGame.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    public class TargetWithinSightNLookAt : TargetWithinSight
    {
        protected LookAtTarget _lookAtTarget;
        protected UltimateCharacterLocomotion _characterLocomotion;

        public SharedVariable<bool> canSearchCharacter;
        public SharedVariable<float> maxDelayUntilSearchAgain = 1.0f;
        private float _curTime;

        public override void OnAwake()
        {
            base.OnAwake();
            _characterLocomotion = gameObject.GetComponentInParent<UltimateCharacterLocomotion>();
            _lookAtTarget = _characterLocomotion.GetAbility<LookAtTarget>();
        }

        public override TaskStatus OnUpdate()
        {
            if (returnedTarget.Value != null) _lookAtTarget.Update();

            TaskStatus status = base.OnUpdate();
            if (status == TaskStatus.Success)
            {
                if (returnedTarget.Value != null)
                {
                    _lookAtTarget.AddObjectDetected(returnedTarget.Value);
                }
            }

            return status;
        }
    }
}