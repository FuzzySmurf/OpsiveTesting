using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character.Abilities.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.BehaviourDesignerPro
{
    public class TriggerAttack : EnemyAction
    {
        public SharedVariable<GameObject> targetObj;
        public SharedVariable<float> attackDistance;
        [Tooltip("Used to reset a previous attack Timer.")]
        public SharedVariable<bool> resetAttackTimer;
        [Tooltip("Used to determine how far the Character should be the target.")]
        public SharedVariable<float> stoppingDistance = 1.5f;

        public SharedVariable<SVMoveWhileAttacking> moveWhileAttacking;

        protected Use _useObj;
        protected RotateTowards _rotateTowards;
        private bool _isCharacterItemActive = false;
        private bool _attackStarted = false;

        //Used to determine if the character can move again after X time.
        private float _curTime = 0.0f;
        private bool _canMoveAgain = false;

        /// <summary>
        /// the attackDistance is sometimes used to 'get in range' of an attack. to avoid issues, we want to add an extra '0.2' distance
        /// </summary>
        protected float _totalAttackDistance
        {
            get { return attackDistance.Value + 0.2f; }
        }

        public override void OnAwake()
        {
            base.OnAwake();
            _useObj = _characterLocomotion.GetAbility<Use>();
            _rotateTowards = _characterLocomotion.GetAbility<RotateTowards>();

            if (_totalAttackDistance < stoppingDistance.Value)
            {
                Debug.Log($"The <color=red>attack</color> for NPC: ({this.transform.parent.name}) has a greater 'StoppingDistance' then 'attackDistance'. May cause issues as the NPC wont be able to attack the target.");
            }
        }

        public override TaskStatus OnUpdate()
        {
            //Run to target.
            float distance = Vector3.Distance(this.transform.parent.position, targetObj.Value.transform.position);

            TimeUntilCanMove();

            if (!CanMoveWhileAttack())
            {
                //only triggers if we dont want the NPC to move while the Attack is running.
                StopMoving();
                return ResetValues();
            }
            else if (distance <= _totalAttackDistance)
            {
                return RunAttackAbility();
            }
            else
            {
                //not close enough to Target, move closer.
                MoveToTarget();
                return TaskStatus.Running;
            }
        }

        /// <summary>
        /// processes time, after the attack, to determine when the NPC can move again.
        /// OTHERWISE, will use the default UseEventComplete Time instead.
        /// </summary>
        private void TimeUntilCanMove()
        {
            if (!_attackStarted) return;
            var moveAttack = moveWhileAttacking;

            if (moveAttack.Value.moveAfterXTime.Value >= 0.0f) return;

            _curTime += Time.deltaTime;
            if (_curTime >= moveAttack.Value.moveAfterXTime.Value)
                _canMoveAgain = true;

        }

        /// <summary>
        /// After the attack started, should the NPC be able to move?
        /// </summary>
        /// <returns></returns>
        private bool CanMoveWhileAttack()
        {
            //attack hasn't started yet, they CAN move.
            if (!_attackStarted) return true;
            if (_canMoveAgain && _attackStarted) return true;
            bool canMoveWhileAttacking = moveWhileAttacking.Value.canMoveWhileAttacking.Value;
            return (canMoveWhileAttacking && _attackStarted);
        }

        private TaskStatus RunAttackAbility()
        {
            //once within hitting distance, punch target.
            if (!_useObj.CanStartAbility() && !_useObj.IsActive)
                Debug.LogWarning("Could not start USE Ability. Is Item Equipped?");

            //start ability if not started yet.
            if (!_useObj.IsActive && !_isCharacterItemActive)
            {
                _isCharacterItemActive = true;
                _attackStarted = true;
                _rotateTowards.Target = null;
                bool bStarted = _characterLocomotion.TryStartAbility(_useObj);
                Debug.Log($"Triggered start ability: <color=red>{bStarted}</color>");
            }
            return ResetValues();
        }

        private void StopMoving()
        {
            //if (!_aStarAgent.IsActive && !_characterLocomotion.MoveTowardsAbility.IsActive) return;
            if (!_aStarAgent.IsActive) return;


            Debug.Log("Attempting to Stop Moving.");
            bool bOk = false;
            //if(_characterLocomotion.MoveTowardsAbility.IsActive)
            //    bOk = _characterLocomotion.TryStopAbility(_characterLocomotion.MoveTowardsAbility, true);
            if (_aStarAgent.IsActive)
                bOk = _characterLocomotion.TryStopAbility(_aStarAgent, true); //some reason this causes it to crash..?

            Debug.Log("did we stop moving? " + bOk);
        }

        private TaskStatus ResetValues()
        {
            //if USE finished, or stopped playing...
            if ((!_useObj.IsActive && _isCharacterItemActive) || (_canMoveAgain && _isCharacterItemActive))
            {
                //Reset AttackTimer values.
                _isCharacterItemActive = false;
                resetAttackTimer.Value = true;
                _attackStarted = false;

                //resets the MoveAgain Values.
                _canMoveAgain = false;
                _curTime = 0.0f;
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }

        private void MoveToTarget()
        {
            //if attack timer was just reset, we shouldn't be moving towards the character.
            if (resetAttackTimer.Value) return;

            //we need to move closer to the target.
            Vector3 stopLocation = DetermineStoppingDestination(targetObj.Value, stoppingDistance.Value);
            _aStarAgent.SetDestination(stopLocation);
        }
    }
}