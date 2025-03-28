using Opsive.BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    public class IdleAction : EnemyAction
    {
        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            base.OnUpdate();

            return TaskStatus.Success;
        }
    }
}