using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("Returns success if the variable value is equal to the compareTo value.")]
    public class CompareVector3 : Conditional
    {
        public SharedVariable<Vector3> variable;
        public SharedVariable<Vector3> compareTo;

        public override TaskStatus OnUpdate()
        {
            if (variable.Value == null && compareTo.Value != null)
                return TaskStatus.Failure;
            if (variable.Value == null && compareTo.Value == null)
                return TaskStatus.Success;

            return variable.Value.Equals(compareTo.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void Reset()
        {
            variable = null;
            compareTo = null;
        }
    }
}