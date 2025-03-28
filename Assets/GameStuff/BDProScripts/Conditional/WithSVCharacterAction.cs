using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("Used to determine if the \"targetCharAction\" matches the \"setCharAction\". if so, return success. else failure. ")]
    public class WithSVCharacterAction : EnemyConditional
    {
        public SharedVariable<ECharActions> targetCharAction;
        public SharedVariable<ECharActions> setCharAction;

        public override TaskStatus OnUpdate()
        {
            return (targetCharAction.Value == setCharAction.Value) ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}