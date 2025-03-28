using ARAWorks.Base.Contracts;
using ARAWorks.Base.Timer;
using ARAWorks.UIUtilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace ARAWorks.Damage.UIToolkit
{
    public class VEDamageNumberAnimation
    {
        public VisualElement damageNumberContainer { get; private set; }
        public Label damageNumberLabel { get; private set; }
        private Camera _mainCamera;
        private Vector3 _targetPos;

        private Timer _timer;

        private Vector2 _start;
        private Vector2 _goal;

        private float _containerOffset;
        private string _colorClass;
        private string _outlineClass;


        public VEDamageNumberAnimation(Camera mainCamera)
        {
            _mainCamera = mainCamera;

            this.damageNumberContainer = damageNumberContainer;
            this.damageNumberLabel = damageNumberLabel;

            //_timer = new Timer(1.5f, false, TimerMemoryManagementType.ClearOnObjectNullOrSceneUnload);
            _timer = new Timer(1.5f, false, TimerMemoryManagementType.ClearOnSceneUnload);

            damageNumberContainer = new VisualElement();
            damageNumberContainer.usageHints = UsageHints.DynamicTransform;
            damageNumberLabel = new Label();

            damageNumberContainer.Add(damageNumberLabel);
            damageNumberContainer.style.position = Position.Absolute;
            damageNumberContainer.pickingMode = PickingMode.Ignore;

            damageNumberLabel.name = UIConstantDamage.DamageNumbers.DamageNumberLabel;
            damageNumberLabel.usageHints = UsageHints.DynamicTransform;
            damageNumberLabel.AddToClassList(UIConstantDamage.DamageNumbers.DamageNumberClass);
            damageNumberLabel.AddToClassList(UIConstantDamage.DamageNumbers.DamageNumberFontClass);
            damageNumberLabel.pickingMode = PickingMode.Ignore;
        }

        public void Start(ContractDamageInfo info, float height, float range, float containerOffset)
        {
            _targetPos = info.target.transform.position;
            _start = damageNumberLabel.transform.position;
            _goal = new Vector2(Random.Range(-range, range), height);
            _containerOffset = containerOffset;


            _colorClass = AdditivesMapper.GetTextClass(info.calculatedDamage);
            _outlineClass = AdditivesMapper.GetOutlineClass(info.calculatedDamage);
            damageNumberLabel.text = SettingsRounding.RoundValue(info.calculatedDamage.Value, ESettingsRoundingType.FloatingNumbers);
            damageNumberLabel.AddToClassList(_colorClass);
            damageNumberLabel.AddToClassList(_outlineClass);

            SetPositon();
            _timer.Restart();
        }

        public bool Update()
        {
            if (_timer.IsRunning == true)
            {
                SetPositon();
                Vector3 EaseOutCirc = new Vector3(Helpers.EasingFunction.EaseOutCirc(_start.x, _goal.x, _timer.PercentComplete), Helpers.EasingFunction.EaseOutCirc(_start.y, _goal.y, _timer.PercentComplete));
                damageNumberLabel.transform.position = EaseOutCirc;
            }

            return _timer.IsFinished;
        }

        public void Complete()
        {
            damageNumberLabel.transform.position = Vector3.zero;
            damageNumberLabel.RemoveFromClassList(_colorClass);
            damageNumberLabel.RemoveFromClassList(_outlineClass);
        }


        private void SetPositon()
        {
            Vector2 pos = RuntimePanelUtils.CameraTransformWorldToPanel(damageNumberContainer.panel, _targetPos, _mainCamera);
            pos = new Vector2(pos.x - damageNumberContainer.layout.width / 2, pos.y + _containerOffset);
            damageNumberContainer.transform.position = pos;
        }
    }
}
