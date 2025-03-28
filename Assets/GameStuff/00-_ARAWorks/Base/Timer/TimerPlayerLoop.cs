using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace ARAWorks.Base.Timer.LowLevel
{
    public static class TimerUpdatePlayerLoop
    {
        public static event Action UpdateCallback;

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public struct HoochCustomUpdate
        {
            [StructLayout(LayoutKind.Sequential, Size = 1)]
            public struct TimerUpdate { }
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            PlayerLoopSystem defaultSystems = PlayerLoop.GetCurrentPlayerLoop();

            PlayerLoopSystem hoochCustomUpdate = new PlayerLoopSystem()
            {
                updateDelegate = null,
                type = typeof(HoochCustomUpdate),
                subSystemList = new PlayerLoopSystem[1]
                {
                    new PlayerLoopSystem()
                    {
                        updateDelegate = InternalUpdate,
                        type = typeof(HoochCustomUpdate.TimerUpdate),
                    }
                }
            };

            PlayerLoopSystem timerUpdateLoop = AddSystemBefore<Update>(in defaultSystems, hoochCustomUpdate);
            PlayerLoop.SetPlayerLoop(timerUpdateLoop);
        }

        private static void InternalUpdate()
        {
            if (Application.isPlaying)
            {
                UpdateCallback?.Invoke();
            }
        }

        private static PlayerLoopSystem GetSystem<T>(in PlayerLoopSystem loopSystem) where T : struct
        {
            PlayerLoopSystem found = default;
            Type typeToFind = typeof(T);

            foreach (PlayerLoopSystem subSystem in loopSystem.subSystemList)
            {
                if (subSystem.type == typeToFind)
                {
                    found = subSystem;
                }
            }

            return found;
        }

        private static PlayerLoopSystem AddSystemAfter<T>(in PlayerLoopSystem loopSystem, PlayerLoopSystem systemToAdd) where T : struct
        {
            PlayerLoopSystem newPlayerLoop = new PlayerLoopSystem()
            {
                loopConditionFunction = loopSystem.loopConditionFunction,
                type = loopSystem.type,
                updateDelegate = loopSystem.updateDelegate,
                updateFunction = loopSystem.updateFunction
            };

            List<PlayerLoopSystem> newSubSystemList = new List<PlayerLoopSystem>();
            Type typeToFind = typeof(T);

            foreach (PlayerLoopSystem subSystem in loopSystem.subSystemList)
            {
                newSubSystemList.Add(subSystem);

                if (subSystem.type == typeToFind)
                {
                    newSubSystemList.Add(systemToAdd);
                }
            }

            newPlayerLoop.subSystemList = newSubSystemList.ToArray();
            return newPlayerLoop;
        }

        private static PlayerLoopSystem AddSystemBefore<T>(in PlayerLoopSystem loopSystem, PlayerLoopSystem systemToAdd) where T : struct
        {
            PlayerLoopSystem newPlayerLoop = new PlayerLoopSystem()
            {
                loopConditionFunction = loopSystem.loopConditionFunction,
                type = loopSystem.type,
                updateDelegate = loopSystem.updateDelegate,
                updateFunction = loopSystem.updateFunction
            };

            List<PlayerLoopSystem> newSubSystemList = new List<PlayerLoopSystem>();
            Type typeToFind = typeof(T);

            foreach (PlayerLoopSystem subSystem in loopSystem.subSystemList)
            {
                if (subSystem.type == typeToFind)
                {
                    newSubSystemList.Add(systemToAdd);
                }

                newSubSystemList.Add(subSystem);
            }

            newPlayerLoop.subSystemList = newSubSystemList.ToArray();
            return newPlayerLoop;
        }

        private static PlayerLoopSystem RemoveSystem<T>(in PlayerLoopSystem loopSystem) where T : struct
        {
            PlayerLoopSystem newPlayerLoop = new PlayerLoopSystem()
            {
                loopConditionFunction = loopSystem.loopConditionFunction,
                type = loopSystem.type,
                updateDelegate = loopSystem.updateDelegate,
                updateFunction = loopSystem.updateFunction
            };

            List<PlayerLoopSystem> newSubSystemList = new List<PlayerLoopSystem>();

            Type typeToFind = typeof(T);
            foreach (PlayerLoopSystem subSystem in loopSystem.subSystemList)
            {
                if (subSystem.type == typeToFind) continue;

                newSubSystemList.Add(subSystem);
            }

            newPlayerLoop.subSystemList = newSubSystemList.ToArray();
            return newPlayerLoop;
        }
    }

}