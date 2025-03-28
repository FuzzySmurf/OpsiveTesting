using ARAWorks.Base.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ARAWorks.Spawner
{
    [System.Serializable]
    public class ObstacleAvoidanceData
    {
        public bool navMeshSpawn = false;
        public SpawnerObstacleAvoidanceHandler.AvoidancePrecision obstacleAvoidPrecision = SpawnerObstacleAvoidanceHandler.AvoidancePrecision.None;
        public LayerMask obstaclesToAvoidLayers = new LayerMask();
    }

    public class SpawnerObstacleAvoidanceHandler
    {
        public enum AvoidancePrecision { None, Low, Medium, High }

        public bool NavMeshSpawning => _obstacleAvoidanceData.navMeshSpawn;
        public AvoidancePrecision ObstacleAvoidPercision => _obstacleAvoidanceData.obstacleAvoidPrecision;
        public LayerMask ObstaclesToAvoidLayers => _obstacleAvoidanceData.obstaclesToAvoidLayers;
        public int AvoidanceAttemptLimit { get; private set; } = 0;

        private LayerMask _combinedMask;
        private LayerMask _groundMask;
        private SpawnerRandomPointPicker _randomPointPicker;
        private ObstacleAvoidanceData _obstacleAvoidanceData;
        private string _spawnerName;

        // Manually Adjustable Variables
        private const int lowPrecisionPoint = 50;
        private const int medPrecisionPoint = 150;
        private const int highPrecisionPoint = 400;


        public SpawnerObstacleAvoidanceHandler(ObstacleAvoidanceData obstacleAvoidanceData, SpawnerRandomPointPicker randomPointPicker, LayerMask groundMask, string spawnerName)
        {
            _obstacleAvoidanceData = obstacleAvoidanceData;
            _randomPointPicker = randomPointPicker;
            _groundMask = groundMask;
            _combinedMask = _groundMask + ObstaclesToAvoidLayers;
            _spawnerName = spawnerName;

            InitializeAvoidPrecision();
        }


        /// <summary>
        /// Initialize how many loops to try avoiding obstacles before quitting
        /// </summary>
        private void InitializeAvoidPrecision()
        {
            AvoidanceAttemptLimit = 0;
            if (NavMeshSpawning)
                AvoidanceAttemptLimit += 15;

            switch (ObstacleAvoidPercision)
            {
                case AvoidancePrecision.None:
                    AvoidanceAttemptLimit += 1;
                    break;
                case AvoidancePrecision.Low:
                    AvoidanceAttemptLimit += lowPrecisionPoint;
                    break;
                case AvoidancePrecision.Medium:
                    AvoidanceAttemptLimit += medPrecisionPoint;
                    break;
                case AvoidancePrecision.High:
                    AvoidanceAttemptLimit += highPrecisionPoint;
                    break;
                default:
                    AvoidanceAttemptLimit += 1;
                    break;
            }
        }


        /// <summary>
        /// Raycasts downward and decides if it should spawn or avoid obstacle
        /// </summary>
        /// <param name="loopCount"> Number of times the raycast has run (used as recursive check if running too much).</param>
        /// <returns>A clear position to spawn (after obstacle avoidance)</returns>
        public Vector3 RaycastPosObstacleAvoid(int loopCount = 0)
        {
            loopCount++;
            if (CheckAvoidancePrecisionFail(loopCount))
            {
#if UNITY_EDITOR
                Debug.Log("Spawner \"" + _spawnerName + "\" could not avoid obstacles when spawning.");
#endif
                return Vector3.zero;
            }

            Vector3 position = _randomPointPicker.GetSpawnPosition();
            Vector3? posPostAvoidance = SingleObstacleAvoidCheck(position, NavMesh.AllAreas);

            if (posPostAvoidance != null)
                return posPostAvoidance.Value;

            return RaycastPosObstacleAvoid(loopCount);
        }

        public Vector3? SingleObstacleAvoidCheck(Vector3 position, int navArea)
        {
            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.down, out hit, 1000f, _combinedMask))
            {
                if (_groundMask.Contains(hit.collider.gameObject.layer))
                {
                    if (NavMeshSpawning == true)
                    {
                        Vector3? navMeshPoint = CheckOnNavMesh(hit.point, navArea);

                        if (navMeshPoint != null)
                            return navMeshPoint.Value;
                    }
                    else
                    {
                        return hit.point;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Checks it should stop trying to avoid obstacles (for performance sake)
        /// </summary>
        /// <param name="loopCount">Number of times the obstacle avoidance has run</param>
        /// <returns> True if avoidance should stop, False if it can keep running</returns>
        private bool CheckAvoidancePrecisionFail(int loopCount)
        {
            if (loopCount > AvoidanceAttemptLimit)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Determines if pos is on navmash
        /// </summary>
        /// <param name="pos">Position to check if is on navmesh</param>
        /// <returns>True if it's on navmesh, False if it is not </returns>
        private Vector3? CheckOnNavMesh(Vector3? pos, int navArea)
        {
            if (pos == null) return null;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(pos.Value, out navHit, 1f, navArea))
                return navHit.position;
            else
                return null;
        }
    }
}