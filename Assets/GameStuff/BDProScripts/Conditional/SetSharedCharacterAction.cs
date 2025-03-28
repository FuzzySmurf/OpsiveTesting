using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    public class SetSharedCharacterAction : EnemyConditional
    {
        public SharedVariable<ECharActions> currentCharacterAction;
        public SharedVariable<ECharActions> targetCharacterAction;
        public override TaskStatus OnUpdate()
        {
            if (currentCharacterAction == null) return TaskStatus.Failure;
            if (targetCharacterAction == null) return TaskStatus.Failure;

            currentCharacterAction.Value = targetCharacterAction.Value;
            return TaskStatus.Success;
        }
    }
}