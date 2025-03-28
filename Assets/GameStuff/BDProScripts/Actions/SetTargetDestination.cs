using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    public class SetTargetDestination : EnemyAction
    {
        public SharedVariable<Vector3> targetPosition;

        public override void OnStart()
        {
            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            if (targetPosition == null) return TaskStatus.Failure;
            if (targetPosition.Value == Vector3.zero) return TaskStatus.Failure;

            _aStarAgent.SetDestination(targetPosition.Value);

            return base.OnUpdate();
        }
    }
}