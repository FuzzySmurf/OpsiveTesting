using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    public class SVMinMaxWaitTime
    {
        public SharedVariable<float> minWaitTime = 0.0f;
        public SharedVariable<float> maxWaitTime = 1.0f;
    }
}