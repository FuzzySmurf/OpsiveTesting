using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.UltimateCharacterController.Character.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    public class FightRotate : EnemyAction
    {
        [Tooltip("The target to rotate towards.")]
        public SharedVariable<GameObject> targetCharacter;
        protected RotateTowards _rotationTowards;

        public override void OnAwake()
        {
            base.OnAwake();
            _rotationTowards = _characterLocomotion.GetAbility<RotateTowards>();

        }

        public override TaskStatus OnUpdate()
        {
            RunRotation();
            //if (_agent.RotationOverride != RotationOverrideMode.Character)
            //    _agent.RotationOverride = RotationOverrideMode.Character;

            return base.OnUpdate();
        }

        private void RunRotation()
        {
            if (!_rotationTowards.IsActive)
            {
                _rotationTowards.Target = targetCharacter.Value.transform;
                bool bOk = _rotationTowards.StartAbility();
            }
        }
    }
}