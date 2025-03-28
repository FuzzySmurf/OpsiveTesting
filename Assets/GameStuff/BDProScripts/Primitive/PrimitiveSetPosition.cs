using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("Used to assign the position for the given primitive.")]
    public class PrimitiveSetPosition : EnemyAction
    {
        public SharedVariable<GameObject> primitiveReference;
        public SharedVariable<Vector3> positionForPrimitive;

        public override void OnStart()
        {
            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            if (primitiveReference == null || primitiveReference.Value == null) return TaskStatus.Failure;
            if (positionForPrimitive == null || positionForPrimitive.Value == null) return TaskStatus.Failure;

            primitiveReference.Value.transform.position = positionForPrimitive.Value;

            return TaskStatus.Success;
        }
    }
}