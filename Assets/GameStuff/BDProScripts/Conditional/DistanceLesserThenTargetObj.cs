using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("Used to Determin if the \"currentDistanceFromTarget\" is Lesser then or Equal to the \"targetDistance\".")]
    public class DistanceLesserThenTargetObj : EnemyConditional
    {
        public SharedVariable<float> targetDistance = 5.0f;
        public SharedVariable<GameObject> targetGameObject = null;

        public override TaskStatus OnUpdate()
        {
            if (targetGameObject == null || targetGameObject.Value == null) return TaskStatus.Failure;

            var dist = Vector3.Distance(this.transform.position, targetGameObject.Value.transform.position);
            return (targetDistance.Value >= dist) ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}