using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.UltimateCharacterController.Character.Abilities;
using PolyGame.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("If the \"targetCharacter\" is further then the 'leash' distance, then break the leash.")]
    public class TargetAwayFromLeashRange : EnemyAction
    {
        public SharedVariable<GameObject> targetCharacter;
        public SharedVariable<ECharActions> characterActions;
        public SharedVariable<float> maxLeashDistance = 20.0f;

        private LookAtTarget _lookAtTarget;
        private RotateTowards _rotationTowards;

        public override void OnAwake()
        {
            base.OnAwake();
            _rotationTowards = _characterLocomotion.GetAbility<RotateTowards>();
            _lookAtTarget = _characterLocomotion.GetAbility<LookAtTarget>();
        }

        public override TaskStatus OnUpdate()
        {
            var distance = Vector3.Distance(transform.parent.position, targetCharacter.Value.transform.position);

            if (distance > maxLeashDistance.Value)
            {
                //remove character from list of possible 'look at' targets.
                _lookAtTarget.RemoveObjectDetected(targetCharacter.Value);
                //_agent.RotationOverride = RotationOverrideMode.NoOverride;
                if (_rotationTowards != null)
                {
                    _rotationTowards.Target = null;
                    _rotationTowards.StopAbility();
                }
                characterActions.Value = ECharActions.None;
                targetCharacter.Value = null;
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}