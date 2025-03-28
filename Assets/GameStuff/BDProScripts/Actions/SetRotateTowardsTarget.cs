using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.UltimateCharacterController.Character.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("Used to set a target for the RotateTowards Ability. Fails if rotate not found.")]
    public class SetRotateTowardsTarget : EnemyAction
    {
        private RotateTowards _rotate;
        public SharedVariable<GameObject> _target;
        public override void OnAwake()
        {
            base.OnAwake();

        }

        public override void OnStart()
        {
            base.OnStart();
            _rotate = _characterLocomotion.GetAbility<RotateTowards>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_rotate == null) return TaskStatus.Failure;
            if (_rotate.Target == null && _target.Value == null) return TaskStatus.Success;
            if (_rotate.Target == _target.Value) return TaskStatus.Success;

            _rotate.Target = (_target.Value == null) ? null : _target.Value.transform;

            return TaskStatus.Success;
        }
    }
}