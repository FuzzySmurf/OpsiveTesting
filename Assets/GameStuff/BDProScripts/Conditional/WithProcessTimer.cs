using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("Will return failure until the given \"maxTime\" has been met. then return success. ")]
    public class WithProcessTimer : EnemyConditional
    {
        public SharedVariable<bool> isResetTime = false;
        public SharedVariable<SVMinMaxWaitTime> minMaxWaitTime;

        private SharedVariable<float> maxTime = 0.0f;
        private SharedVariable<float> curTime = 0.0f;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            ResetTimer();

            if (curTime.Value < maxTime.Value)
            {
                curTime.Value += Time.deltaTime;
                return TaskStatus.Failure;
                //return TaskStatus.Running;
            }
            return TaskStatus.Success;
        }

        /// <summary>
        /// Resets the running curTimer, and sets a new MaxTime value.
        /// </summary>
        private void ResetTimer()
        {
            if (maxTime.Value == 0.0f || isResetTime.Value)
            {
                if (minMaxWaitTime == null)
                    Debug.Log("Hello! minMaxWaitTime is null. " + this.transform.parent.name);

                if (minMaxWaitTime != null)
                    if (minMaxWaitTime.Value.minWaitTime == null)
                        Debug.Log("Hello! minWaitTime is null. " + this.transform.parent.name);
                maxTime.Value = Random.Range(minMaxWaitTime.Value.minWaitTime.Value, minMaxWaitTime.Value.maxWaitTime.Value);
                curTime.Value = 0.0f;
                isResetTime.Value = false;
            }
        }
    }
}