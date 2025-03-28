using ARAWorks.Base.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace ARAWorks.Damage.UIToolkit
{
    public class VEDamageNumbersController : MonoBehaviour, IDamageNumbersController
    {
        [SerializeField] private float _labelContainerOffset = -50;
        [SerializeField] private float _labelAnimationHeight = -150;
        [SerializeField] private float _labelAnimationXRange = 50;

        protected ObjectPool<VEDamageNumberAnimation> _labelPool;
        private List<VEDamageNumberAnimation> _activeAnimations = new List<VEDamageNumberAnimation>();
        private VisualElement _root;
        private VisualElement _damageNumbersList;

        private Camera _mainCamera;
        public event Action<ContractDamageInfo> OnEntityDamaged;

        protected virtual void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _damageNumbersList = _root.Q<VisualElement>(UIConstantDamage.DamageNumbers.DamageNumbersList);
            _mainCamera = Camera.main;
            _labelPool = new ObjectPool<VEDamageNumberAnimation>(OnCreateDamageNumber, OnGetDamageNumber, OnReturnDamageNumber, OnDestroyDamageNumber, true, 10);
        }

        protected virtual void Start()
        {
            FindNewCamera();

            OnEntityDamaged += EntityDamaged;
        }

        protected virtual void OnDestroy()
        {
            OnEntityDamaged -= EntityDamaged;
        }

        private void Update()
        {
            for (int i = _activeAnimations.Count - 1; i >= 0; i--)
            {
                if (_activeAnimations[i].Update() == true)
                {
                    _labelPool.Release(_activeAnimations[i]);
                    _activeAnimations[i].Complete();
                    _activeAnimations.Remove(_activeAnimations[i]);
                }
            }
        }

        private void EntityDamaged(ContractDamageInfo info)
        {
            VEDamageNumberAnimation animation = _labelPool.Get();

            _activeAnimations.Add(animation);
            animation.Start(info, _labelAnimationHeight, _labelAnimationXRange, _labelContainerOffset);
        }

        protected VEDamageNumberAnimation OnCreateDamageNumber()
        {
            FindNewCamera();

            VEDamageNumberAnimation animation = new VEDamageNumberAnimation(_mainCamera);

            _damageNumbersList.Add(animation.damageNumberContainer);

            return animation;
        }

        /// <summary>
        /// Only triggered if the mainCamera happens to be destroyed while scene switching.
        /// </summary>
        private void FindNewCamera()
        {
            if (_mainCamera != null) return;

            foreach(var cam in Camera.allCameras)
            {
                if (cam != null)
                    _mainCamera = cam;
            }

        }

        protected void OnGetDamageNumber(VEDamageNumberAnimation element)
        {
            element.damageNumberContainer.style.display = DisplayStyle.Flex;
        }

        protected void OnReturnDamageNumber(VEDamageNumberAnimation element)
        {
            element.damageNumberContainer.style.display = DisplayStyle.None;
        }

        protected void OnDestroyDamageNumber(VEDamageNumberAnimation element)
        {
            _damageNumbersList.Remove(element.damageNumberContainer);
        }

        public void InvokeEntityDamage(ContractDamageInfo entityDamageInfo)
        {
            OnEntityDamaged?.Invoke(entityDamageInfo);
        }
    }
}
