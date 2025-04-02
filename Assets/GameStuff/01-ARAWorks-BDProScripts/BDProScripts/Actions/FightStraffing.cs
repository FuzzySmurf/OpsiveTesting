using ARAWorks.BehaviourDesignerPro;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.UltimateCharacterController.Character.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyGame.BehaviourDesignerPro
{
    public class FightStraffing : EnemyAction
    {
        public SharedVariable<GameObject> targetCharacter;
        public SharedVariable<float> npcChasingDistance;
        public SharedVariable<bool> resetPosition = false;

        public float rotationalAnglePositions = 60.0f;
        private float _straffingDistance { get { return npcChasingDistance.Value - 1; } }

        [Tooltip("How far can 'this' NPC be from there target waypoint.")]
        private float _distanceFromTarget = 1.5f;

        private Vector3 rotatedLeftPoint;
        private Vector3 rotatedRightPoint;
        private Vector3 currentDestination;

        protected RotateTowards _rotationTowards;

        public override void OnAwake()
        {
            base.OnAwake();
            _rotationTowards = _characterLocomotion.GetAbility<RotateTowards>();
        }

        public override TaskStatus OnUpdate()
        {
            RunRotation();

            currentDestination = CheckNextDestination();
            //if(_agent.RotationOverride != RotationOverrideMode.Character)
            //    _agent.RotationOverride = RotationOverrideMode.Character;

            _aStarAgent.SetDestination(currentDestination);

            return base.OnUpdate();
        }

        private void RunRotation()
        {
            if (!_rotationTowards.IsActive)
            {
                _rotationTowards.Target = targetCharacter.Value.transform;
                bool bOk = _rotationTowards.StartAbility();
            }
        }

        private Vector3 CheckNextDestination()
        {
            //check if 'this' NPC is close to the destination yet. or resetPosition was initiated.
            if (CheckWithinDistance(currentDestination, this.transform.position) || currentDestination == Vector3.zero || resetPosition.Value)
            {
                int val = -1; //only used to reset the position, and determine if we're going left or right.
                if (resetPosition.Value)
                {
                    val = Random.Range(0, 1);
                    resetPosition.Value = false;
                }

                //check if the currentDestination was set to the 'left' waypoint or the right.
                if (CheckWithinDistance(currentDestination, rotatedLeftPoint) || currentDestination == Vector3.zero || val == 0)
                {
                    //Generate new direction endpoints for left and right.
                    GetDirection(_straffingDistance, out rotatedLeftPoint, out rotatedRightPoint);
                    currentDestination = rotatedRightPoint;
                }
                else if (CheckWithinDistance(currentDestination, rotatedRightPoint) || val == 1)
                {
                    GetDirection(_straffingDistance, out rotatedLeftPoint, out rotatedRightPoint);
                    currentDestination = rotatedLeftPoint;
                }
            }

            return currentDestination;
        }

        /// <summary>
        /// Check the destination differences between our currentDestination and the target destination.
        /// </summary>
        /// <param name="curDestination">our Current Destination.</param>
        /// <param name="targetDest"></param>
        /// <returns></returns>
        private bool CheckWithinDistance(Vector3 curDestination, Vector3 targetDest)
        {
            return Vector3.Distance(curDestination, targetDest) < _distanceFromTarget;
        }

        /// <summary>
        /// Get new directional left and right points for the player.
        /// </summary>
        /// <param name="distance">How far should the new points be from the NPC?</param>
        /// <param name="rotatedLeft">The left point that will be used.</param>
        /// <param name="rotatedRight">The right point that will be used.</param>
        private void GetDirection(float distance, out Vector3 rotatedLeft, out Vector3 rotatedRight)
        {
            //Get a normalized value to check V3 direction.
            Vector3 difference = targetCharacter.Value.transform.position - this.transform.parent.position;
            Vector3 direction = difference.normalized;

            Vector3 invertedDirection = -direction * distance;

            //rotate direction. will need to get the characters 'up' direction in this eventually.
            Quaternion rotateRight = Quaternion.Euler(0, -rotationalAnglePositions, 0);
            rotatedRight = targetCharacter.Value.transform.position + (rotateRight * invertedDirection);
            Quaternion rotateLeft = Quaternion.Euler(0, rotationalAnglePositions, 0);
            rotatedLeft = targetCharacter.Value.transform.position + (rotateLeft * invertedDirection);
        }
    }
}