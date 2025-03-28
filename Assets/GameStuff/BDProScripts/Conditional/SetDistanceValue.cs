using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    public class SetDistanceValue : EnemyConditional
    {
        public SharedVariable<float> setSharedDistance = 0.0f;
        public SharedVariable<GameObject> targetObject;

        private Transform _parentObject;

        public override void OnAwake()
        {
            base.OnAwake();
            _parentObject = this.transform.parent;
        }

        public override TaskStatus OnUpdate()
        {
            setSharedDistance.Value = Vector3.Distance(targetObject.Value.transform.position, _parentObject.position);
            return TaskStatus.Success;
        }
    }
}