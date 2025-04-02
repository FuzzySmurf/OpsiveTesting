using ARAWorks.BehaviourDesignerPro;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using PolyGame.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyGame.BehaviourDesignerPro
{
    public class BattleManagerAction : EnemyAction
    {
        [UnityEngine.Tooltip("If Action is '0' will Remove. If Action is '1', Will add. if Action is '-1' will do nothing.")]
        public SharedVariable<int> AddCharacter = -1;

        private BaseCharacter _character;

        public override void OnStart()
        {
            base.OnStart();

            if (_character == null)
                _character = this.transform.parent.GetComponent<BaseCharacter>();

            TriggerAction();
        }

        public override TaskStatus OnUpdate()
        {
            return base.OnUpdate();
        }

        private void TriggerAction()
        {

        }
    }
}