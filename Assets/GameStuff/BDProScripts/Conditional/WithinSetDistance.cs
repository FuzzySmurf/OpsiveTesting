using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("Used to Determine if the \"TargetCharacter\" is within the \"maxTargetDistance\" " +
        "If 'this' distance from \"TargetCharacter\" is less then \"maxTargetDistance\" The return success. " +
        "Else return failure. Leave \"SetCharAction\" to None if you dont want to set a CharacterAction.")]
    public class WithinSetDistance : EnemyConditional
    {
        public SharedVariable<float> maxTargetDistance = 5.0f;
        public SharedVariable<GameObject> targetCharacter;
        public SharedVariable<ECharActions> characterActions;
        public SharedVariable<ECharActions> setCharAction;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            //if target character is not assigned, fail it. nothing to compare.
            if (targetCharacter.Value == null) return TaskStatus.Failure;

            var distance = Vector3.Distance(transform.parent.position, targetCharacter.Value.transform.position);

            if (distance < maxTargetDistance.Value)
            {
                if (setCharAction.Value != ECharActions.None)
                    characterActions = setCharAction;

                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}