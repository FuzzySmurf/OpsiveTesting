using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace ARAWorks.Spawner
{
    public class SpawnerCommunicator : MonoBehaviour
    {
        [Tooltip("The entity is not controlled by any spawner and should handle its own initialization (should be false if actually handled by a spawner).")]
        [SerializeField] private bool _isStandalone = false;
        [ShowIf("_isStandalone"), SerializeField, Min(1)] private int _level = 1;

        public Action Initialized;
        [FoldoutGroup("Events")]
        public UnityEvent StartedMoveOffNavMesh;
        [FoldoutGroup("Events")]
        public UnityEvent FinishedMoveOffNavMesh;

        public bool IsInitialized { get; private set; } = false;
        public int Level { get; private set; } = 0;
        public bool FollowWaypoint { get; private set; }
        public uint WaypointID { get; private set; }
        public string WaypointTag { get; private set; }

        private EnemySpawner _spawner;

        public void Awake()
        {
            if (_isStandalone == true)
                InitializeData(null, _level, false, 0, null);
        }

        public void InitializeData(EnemySpawner spawner, int level, bool followWaypoint, uint id, string tag)
        {
            _spawner = spawner;
            Level = level;
            FollowWaypoint = followWaypoint;
            WaypointID = id;
            WaypointTag = tag;

            IsInitialized = true;
            Initialized?.Invoke();
        }

        public void EnemyKilled()
        {
            if (_spawner != null)
                _spawner.EnemyKilledRemoveFromSpawned(gameObject);
        }
    }
}