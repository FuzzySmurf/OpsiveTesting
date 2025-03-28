using System;
using ARAWorks.Base.Timer.LowLevel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ARAWorks.Base.Timer
{
    [Serializable]
    public class Timer : IReadOnlyTimer, IDisposable
    {
        public static int TimerInstanceCount { get; private set; }
        public static int TimerUpdateSubscriptions { get; private set; }

        public event Action Started;

        /// <summary>
        /// Triggers on each update call when the timer is running. Sends the Elapsed time.
        /// </summary>
        public event Action<float> Tick;
        public event Action Finished;

        public float Elapsed { get; private set; }

        public float Goal { get; private set; }

        public bool IsRunning { get; private set; }

        public bool IsFinished { get { return Elapsed >= Goal; } }

        public bool AutoStop { get; private set; }

        public float PercentComplete { get { return Elapsed / Goal; } }

        public float RemainingTime { get { return Goal - Elapsed; } }

        private bool _set;
        private bool _disposed;

        private TimerMemoryManagementContainer _memoryManagement;
        private Scene _sceneReference;


        ///<param name="goal">The length of the timer</param>
        ///<param name="finishedCallback">Callback for when the timer has finished</param>
        ///<param name="autoStop">When set to true the timer calls Stop() when finished. When set to false Pause() is called when finished. Default set to true</param>
        ///<param name="managementType">How should this timers memory be managed?</param>
        public Timer(float goal, Action finishedCallback, bool autoStop = true, TimerMemoryManagementType managementType = TimerMemoryManagementType.ClearOnSceneUnload) : this(goal, finishedCallback, new TimerMemoryManagementContainer(managementType), autoStop) { }


        ///<param name="goal">The length of the timer</param>
        ///<param name="autoStop">When set to true the timer calls Stop() when finished. When set to false Pause() is called when finished. Default set to true</param>
        ///<param name="finishedCallback">Callback for when the timer has finished</param>
        ///<param name="memoryManagementContainer">How should this timers memory be managed?</param>
        public Timer(float goal, Action finishedCallback, TimerMemoryManagementContainer memoryManagementContainer, bool autoStop = true) : this(goal, memoryManagementContainer, autoStop)
        {
            Finished += finishedCallback;
        }

        public Timer(float goal, bool autoStop = true, TimerMemoryManagementType managementType = TimerMemoryManagementType.ClearOnSceneUnload) : this(goal, new TimerMemoryManagementContainer(managementType), autoStop) { }


        ///<param name="goal">The length of the timer</param>
        ///<param name="autoStop">When set to true the timer calls Stop() when finished. When set to false Pause() is called when finished. Default set to true</param>
        ///<param name="memoryManagementContainer">How should this timers memory be managed?</param>
        public Timer(float goal, TimerMemoryManagementContainer memoryManagementContainer, bool autoStop = true)
        {
            Goal = goal;
            AutoStop = autoStop;
            _memoryManagement = memoryManagementContainer;

            if (_memoryManagement.ManagementType != TimerMemoryManagementType.ClearManually)
            {
                SceneManager.sceneUnloaded += OnSceneUnloaded;
                _sceneReference = SceneManager.GetActiveScene();
            }
            TimerInstanceCount++;
        }

        ~Timer()
        {
            if (Validation(true) == true)
            {
                Dispose();
            }
        }

        public void SetGoal(float goal)
        {
            if (Validation() == false) return;

            Goal = goal;
        }

        public void SetElapsedTime(float elapsed)
        {
            if (Validation() == false) return;

            Elapsed = elapsed;
        }

        public void Start()
        {
            if (Validation() == false) return;

            if (_set == false)
            {
                TimerUpdatePlayerLoop.UpdateCallback += Update;
                TimerUpdateSubscriptions++;
                _set = true;
            }
            IsRunning = true;

        }

        public void Stop()
        {
            if (Validation() == false) return;

            if (_set == true)
            {
                TimerUpdatePlayerLoop.UpdateCallback -= Update;
                TimerUpdateSubscriptions--;
                _set = false;
            }

            IsRunning = false;
            Elapsed = 0;
        }

        public void Restart()
        {
            if (Validation() == false) return;

            Elapsed = 0;
            Start();
        }

        public void Pause()
        {
            if (Validation() == false) return;

            IsRunning = false;
        }

        public void Dispose()
        {
            if (_memoryManagement != null && _memoryManagement.ManagementType != TimerMemoryManagementType.ClearManually)
            {
                SceneManager.sceneUnloaded -= OnSceneUnloaded;
            }

            if (_set == true)
            {
                TimerUpdatePlayerLoop.UpdateCallback -= Update;
                TimerUpdateSubscriptions--;
                _set = false;
            }
            _memoryManagement = null;
            Started = null;
            Tick = null;
            Finished = null;
            _disposed = true;
            TimerInstanceCount--;
        }

        private void Update()
        {
            if (_memoryManagement.ManagementType == TimerMemoryManagementType.ClearOnObjectNullOrSceneUnload && _memoryManagement.Reference.IsAlive == false)
            {
                Dispose();
                return;
            }

            if (IsRunning == true)
            {
                if (Elapsed == 0)
                {
                    Started?.Invoke();
                }

                Elapsed += Time.deltaTime;
                Tick?.Invoke(Elapsed);
                if (Elapsed >= Goal)
                {
                    Elapsed = Goal;
                    if (AutoStop == true)
                    {
                        Stop();
                    }
                    else
                    {
                        Pause();
                    }
                    Finished?.Invoke();
                }
            }
        }

        private bool Validation(bool finalizer = false)
        {
            if (_disposed == true)
            {
                if (finalizer == false)
                {
                    Debug.LogError($"Still trying to access this timer. It has been disposed and should be cleaned up by the GC.");
                }
                return false;
            }

            return true;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (scene == _sceneReference)
            {
                Dispose();
            }
        }
    }
}