using System;
using Sirenix.OdinInspector;
using ARAWorks.Base.Timer;

namespace ARAWorks.StatusEffectSystem.Statuses.Utility
{

    [Serializable]
    public class StatusEffectUtilityIntervalOverTime
    {
        public event Action IntervalUpdate;
        [ShowInInspector] public float MaxValue { get; private set; }
        [ShowInInspector] public float MaxTime { get; private set; }
        [ShowInInspector] public float Interval { get; private set; }
        [ShowInInspector] public float ValueEveryInterval { get; private set; }
        public Timer IntervalTimer { get; private set; }

        [ShowInInspector] private float _elapsedTime => IntervalTimer == null ? 0 : IntervalTimer.Elapsed;
        private float _timeCount;


        public StatusEffectUtilityIntervalOverTime(Action healthTimerFinished, float maxValue, float maxTime, float interval = 1)
        {
            MaxValue = maxValue;
            MaxTime = maxTime;
            Interval = interval;
            IntervalTimer = new Timer(maxTime, healthTimerFinished, false);

            UpdateValueEveryInterval();
        }

        public void Start()
        {
            _timeCount = Interval;
            IntervalTimer.Start();
        }

        public void Reset()
        {
            _timeCount = 0;
            IntervalTimer.Stop();
        }

        public void Restart()
        {
            _timeCount = Interval;
            IntervalTimer.Restart();
        }

        public void Update()
        {
            if (IntervalTimer.IsRunning == true && IntervalTimer.Elapsed >= _timeCount)
            {
                _timeCount += Interval;
                IntervalUpdate?.Invoke();
            }
        }

        public void ModifyMaxValue(float maxValue)
        {
            MaxValue = maxValue;
            UpdateValueEveryInterval();
        }

        public void ModifyMaxTime(float maxTime)
        {
            Reset();
            MaxTime = maxTime;
            IntervalTimer.SetGoal(MaxTime);
            UpdateValueEveryInterval();
        }

        public void ModifyIntervalTime(float interval)
        {
            Interval = interval;
            UpdateValueEveryInterval();
        }

        private void UpdateValueEveryInterval()
        {
            ValueEveryInterval = MaxValue / (MaxTime / Interval);
        }
    }
}