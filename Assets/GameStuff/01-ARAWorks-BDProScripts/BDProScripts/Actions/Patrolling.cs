using Opsive.BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyGame.BehaviourDesignerPro
{
    public class Patrolling : BasePatrolling
    {
        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            if (waypointTag.Value == null) return TaskStatus.Failure;

            return base.OnUpdate();
        }
    }
}