using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
using Opsive.UltimateCharacterController.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ARAWorks.BehaviourDesignerPro
{
    public abstract class EnemyConditional : Conditional
    {
        protected Rigidbody _body;
        protected Animator _animator;
        protected NavMeshAgent _agent;
        protected UltimateCharacterLocomotion _characterLocomotion;

        public override void OnAwake()
        {
            base.OnAwake();
            _body = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            if (_agent == null)
                _agent = this.gameObject.GetComponentInParent<NavMeshAgent>();
            _characterLocomotion = this.gameObject.GetComponentInParent<UltimateCharacterLocomotion>();
        }
    }
}