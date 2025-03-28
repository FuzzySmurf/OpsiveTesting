using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    public class TimeUntilCanSearchAgain : Action
    {
        public SharedVariable<bool> canSearchAgain;
        public SharedVariable<float> timeUntilCanSearchAgain;
        public float curTimeUntilCanSearchAgain;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            if (canSearchAgain.Value) return TaskStatus.Success;
            if (curTimeUntilCanSearchAgain < timeUntilCanSearchAgain.Value)
            {
                curTimeUntilCanSearchAgain += Time.deltaTime;
                return TaskStatus.Success;
            }
            else
            {
                canSearchAgain.Value = true;
                curTimeUntilCanSearchAgain = 0;
                return base.OnUpdate();
            }
        }
    }
}