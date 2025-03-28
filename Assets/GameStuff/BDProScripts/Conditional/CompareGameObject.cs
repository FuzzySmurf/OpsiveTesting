using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("Returns success if the variable value is equal to the compareTo value.")]
    public class CompareGameObject : Conditional
    {
        public SharedVariable<GameObject> variable;
        public SharedVariable<GameObject> compareTo;

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