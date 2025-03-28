using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("Used to Determin if the \"currentDistanceFromTarget\" is Greater then the \"targetDistance\".")]
    public class DistanceGreaterThenTarget : EnemyConditional
    {
        public SharedVariable<float> targetDistance = 5.0f;
        public SharedVariable<float> currentDistanceFromTarget = 0.0f;

        public override TaskStatus OnUpdate()
        {
            return (targetDistance.Value < currentDistanceFromTarget.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}