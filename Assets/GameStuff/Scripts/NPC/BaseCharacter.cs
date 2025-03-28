using ARAWorks.Base.Timer;
using ARAWorks.Damage.Interfaces;
using ARAWorks.StatusEffectSystem.Handlers;
using Opsive.BehaviorDesigner.Runtime;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using UnityEngine;

namespace PolyGame.Character
{
    public class BaseCharacter : BaseAIAgent
    {
        public bool CanProcessInput { get; set; } = true;
        private bool IsPlayer => this.CompareTag("Player");
        public bool IsDead { get; protected set; } = false;

        public bool IsAttacking { get; set; }
        [field: SerializeField] public Collider CharacterCollider { get; private set; }

        protected Timer RegenTimer;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            InitializeResolverReferences();
        }

        private bool _isInitialized = false;

        protected virtual void InitializeResolverReferences()
        {
            if (_isInitialized) return;

            _isInitialized = true;
            //Initialize References.
            UltimateCharacterLocomotion ucc = gameObject.GetComponent<UltimateCharacterLocomotion>();
            BehaviorTree bt = gameObject.GetComponentInChildren<BehaviorTree>();
            Die toDie = ucc.GetAbility<Die>();

        }

    }
}