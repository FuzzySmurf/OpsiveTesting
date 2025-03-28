using Opsive.BehaviorDesigner.Integrations.UltimateCharacterController;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.GraphDesigner.Runtime;
using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.Shared.Game;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    [NodeDescription("Tries to start or stop the ability.")]
    [NodeIcon("Assets/Behavior Designer/Integrations/UltimateCharacterController/Editor/Icon.png")]
    [HelpURL("https://www.opsive.com/support/documentation/behavior-designer/integrations/opsive-character-controllers/")]
    public class StartStopAbilityExt : StartStopAbility
    {
        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        protected override void InitializeTarget()
        {
            m_TargetGameObject.Value = m_GameObject.transform.parent.gameObject;
            base.InitializeTarget();
        }
    }
}
