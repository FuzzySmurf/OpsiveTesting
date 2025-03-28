using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using PolyGame.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    public class GetWithinFightingRange : EnemyAction
    {
        public SharedVariable<bool> isDebug;
        public SharedVariable<float> maxFightingDistance = 5.0f;
        public SharedVariable<float> distanceFromTarget = 0;

        public SharedVariable<GameObject> targetCharacter;
        public SharedVariable<ECharActions> characterActions;
        [Tooltip("Use this to set a 'stopping distance'. Leave as '0.0' to use default. How far from the target should we stop at.")]
        public SharedVariable<float> adjustedStoppingDistance;

        protected LookAtTarget _targetLookAt;
        protected EquipUnequip _equipUnequp;

        private float _stoppingDistance;
        private float _defaultStoppingDistance = 1.5f;

        public override void OnAwake()
        {
            base.OnAwake();
            _targetLookAt = _characterLocomotion.GetAbility<LookAtTarget>();
            _equipUnequp = _characterLocomotion.GetAbility<EquipUnequip>();
        }

        public override void OnStart()
        {
            base.OnStart();
            //use default, otherwise use set value.
            _stoppingDistance = (adjustedStoppingDistance.Value == 0.0) ? adjustedStoppingDistance.Value : _defaultStoppingDistance;
        }

        public override TaskStatus OnUpdate()
        {
            _targetLookAt.Update();
            if (targetCharacter == null) return TaskStatus.Failure;
            if (targetCharacter.Value == null) return TaskStatus.Failure;
            //character is already within attack range, return early.

            //if distance is greater then fighting distance, I want to Run towards the target
            //to get within fighting distance.
            if (distanceFromTarget.Value > maxFightingDistance.Value)
            {
                //EquipSword();
                characterActions.Value = ECharActions.WithinChasingRange;
                //_characterLocomotion.MoveTowardsAbility.MoveTowardsLocation(DetermineStoppingDestination(targetCharacter.Value, _stoppingDistance));
                bool isDest = _aStarAgent.SetDestination(DetermineStoppingDestination(targetCharacter.Value, _stoppingDistance));
                //ResetMovementDetails();
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }
    }
}