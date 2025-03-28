using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("Returns success if the variable value is equal to the compareTo value.")]
    public class CompareSharedCharacterAction : Conditional
    {
        public SharedVariable<ECharActions> variable;

        public SharedVariable<ECharActions> compareTo;

        public override TaskStatus OnUpdate()
        {
            return variable.Value.Equals(compareTo.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void Reset()
        {
            variable.SetValue(ECharActions.None);
            compareTo.SetValue(ECharActions.None);
        }
    }
}