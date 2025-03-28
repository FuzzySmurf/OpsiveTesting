using System.Text;
using UnityEngine;
using UnityEngine.LowLevel;

namespace ARAWorks.Base.Timer.LowLevel
{
    public static class TimerLoopDebugger
    {
        public static bool IsDebug
        {
            get => _isDebug;
            set
            {
                _isDebug = value;
                UpdateIsDebug();
            }
        }

        private static bool _isDebug = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {

            if (IsDebug == true)
            {
                Log();
            }

            UpdateIsDebug(true);

        }

        private static void UpdateIsDebug(bool init = false)
        {
            if (IsDebug == true)
            {
                TimerUpdatePlayerLoop.UpdateCallback += Update;
            }
            else if (IsDebug == false && init == false)
            {
                TimerUpdatePlayerLoop.UpdateCallback -= Update;
            }
        }

        private static void Update()
        {
            Debug.Log($"Number of timer instances: {Timer.TimerInstanceCount}");
            Debug.Log($"Number of timer subscriptions: {Timer.TimerUpdateSubscriptions}");
        }

        public static void Log()
        {
            StringBuilder sb = new StringBuilder();
            ShowPlayerLoop(PlayerLoop.GetCurrentPlayerLoop(), sb, 0);
            Debug.Log(sb);
        }

        private static void ShowPlayerLoop(PlayerLoopSystem playerLoopSystem, StringBuilder text, int inline)
        {
            if (playerLoopSystem.type != null)
            {
                for (var i = 0; i < inline; i++)
                {
                    text.Append("\t");
                }
                text.AppendLine(playerLoopSystem.type.Name);
            }

            if (playerLoopSystem.subSystemList != null)
            {
                inline++;
                foreach (var s in playerLoopSystem.subSystemList)
                {
                    ShowPlayerLoop(s, text, inline);
                }
            }
        }
    }
}