using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.BehaviorDesigner.Runtime.Tasks;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("Used to Determin if the \"currentDistanceFromTarget\" is Lesser then or Equal to the \"targetDistance\".")]
    public class DistanceLesserThenTarget : EnemyConditional
    {
        public SharedVariable<float> targetDistance = 5.0f;
        public SharedVariable<float> currentDistanceFromTarget = 0.0f;

        public override TaskStatus OnUpdate()
        {
            return (targetDistance.Value >= currentDistanceFromTarget.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}