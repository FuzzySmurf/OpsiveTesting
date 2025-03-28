using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.UltimateCharacterController.Character.Abilities.AI;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Integrations.AstarPathfindingProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Opsive.UltimateCharacterController.Character.Abilities.AI.NavMeshAgentMovement;

namespace ARAWorks.BehaviourDesignerPro
{
    public class EnemyAction : Action
    {
        protected Rigidbody _body;
        protected Animator _animator;
        protected NavMeshAgent _navAgent;
        protected NavMeshAgentMovement _agent;
        protected AStarAIAgentMovement _aStarAgent;
        protected UltimateCharacterLocomotion _characterLocomotion;

        public override void OnAwake()
        {
            base.OnAwake();
            _body = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _characterLocomotion = gameObject.GetComponentInParent<UltimateCharacterLocomotion>();
            _aStarAgent = _characterLocomotion.GetAbility<AStarAIAgentMovement>();

            SetNavMeshAgent();
        }

        /// <summary>
        /// If AStarAgent is set, There shouldn't be any NavMeshAgent that we need to set.
        /// </summary>
        private void SetNavMeshAgent()
        {
            if (_aStarAgent != null) return;

            _agent = _characterLocomotion.GetAbility<NavMeshAgentMovement>();
            _navAgent = GetComponent<NavMeshAgent>();
            if (_navAgent == null)
                _navAgent = transform.parent.GetComponent<NavMeshAgent>();
        }

        protected void ResetMovementDetails()
        {
            if (_agent.IsActive)
            {
                _agent.RotationOverride = RotationOverrideMode.NavMesh;
                //_navAgent.velocity = 1.0f;
            }
        }

        /// <summary>
        /// Sets the stoping destination + direction where the AI should stop at.
        /// </summary>
        /// <param name="target">The target you're facing.</param>
        /// <returns></returns>
        protected Vector3 SetStopDestination(Transform target)
        {
            //find the distance between enemy and player. (player pos - this.pos).
            Vector3 difference = target.transform.position - transform.parent.position;
            //Normalize it. (should give direction)
            Vector3 direction = difference.normalized * 0.1f;

            return (transform.parent.position + direction);
        }

        protected Vector3 DetermineStoppingDestination(GameObject targetCharacter, float stopDistance)
        {
            //find the distance between enemy and player. (player pos - this.pos).
            Vector3 difference = targetCharacter.transform.position - transform.parent.position;
            //Normalize it. (should give direction)
            Vector3 direction = difference.normalized;
            float currentDistance = difference.magnitude;

            if (currentDistance > stopDistance)
            {
                float moveDistance = currentDistance - stopDistance;
                Vector3 tgtPosition = this.transform.position + direction * moveDistance;
                return tgtPosition;
            }
            //already within circle, nothing to calculate.
            return this.transform.position;
        }
    }
}