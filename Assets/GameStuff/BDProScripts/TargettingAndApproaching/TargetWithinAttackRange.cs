using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    public class TargetWithinAttackRange : EnemyAction
    {
        [Tooltip("How far are we from our attack range?")]
        public SharedVariable<float> npcWithinAttackRange = 1.5f;
        [Tooltip("How far are we until we need to start chasing again?")]
        public SharedVariable<float> npcChasingDistance = 5.0f;
        public SharedVariable<GameObject> targetCharacter;

        public SharedVariable<ECharActions> characterActions;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            if (targetCharacter.Value == null) return TaskStatus.Failure;
            if (characterActions.Value == ECharActions.None) return TaskStatus.Failure;

            var distance = Vector3.Distance(transform.parent.position, targetCharacter.Value.transform.position);

            if (distance < npcWithinAttackRange.Value || distance < npcChasingDistance.Value)
                characterActions.Value = ECharActions.WithinAttackRange;

            return TaskStatus.Success;
        }
    }
}