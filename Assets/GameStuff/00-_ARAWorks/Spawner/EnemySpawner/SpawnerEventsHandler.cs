using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ARAWorks.Spawner
{
    [System.Serializable]
    public class SpawnerEventsHandler
    {
        public OnWaveStart waveStartEvent;
        public OnWaveEnd waveEndEvent;
        public UnityEvent allWavesCompleted;
        public UnityEvent allEnemiesDefeated;

        public void Invoke_WaveStartEvent(int waveNum)
        {
            waveStartEvent?.Invoke(waveNum);
        }

        public void Invoke_WaveEndEvent(int waveNum)
        {
            waveEndEvent?.Invoke(waveNum);
        }

        public void Invoke_AllWavesDoneEvent()
        {
            allWavesCompleted?.Invoke();
        }

        public void Invoke_AllEnemiesDefeated()
        {
            allEnemiesDefeated?.Invoke();
        }
    }
}