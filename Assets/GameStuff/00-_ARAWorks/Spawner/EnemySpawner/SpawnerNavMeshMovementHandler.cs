using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ARAWorks.Spawner
{
    [System.Serializable]
    public class NavMeshMovementData
    {
        public bool startOutsideMainNavArea = false;
        public string startingNavArea;
        public Transform endPosition;
        public bool useRandomPointNearEnd = false;
        [Range(1, 100)] public float endRadius = 1f;
    }

    public class SpawnerNavMeshMovementHandler
    {
        private NavMeshMovementData _data;
        private SpawnerRandomPointPicker _pointPicker;
        private SpawnerObstacleAvoidanceHandler _obstacleAvoidance;
        private MonoBehaviour _spawner;


        public SpawnerNavMeshMovementHandler(NavMeshMovementData data, SpawnerRandomPointPicker pointPicker, SpawnerObstacleAvoidanceHandler obstacleAvoidance, MonoBehaviour spawner)
        {
            _data = data;
            _pointPicker = pointPicker;
            _obstacleAvoidance = obstacleAvoidance;
            _spawner = spawner;
        }


        public void MoveToEndPoint(GameObject mob, SpawnerCommunicator communicator)
        {
            NavMeshAgent agent = mob.GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError($"{mob.name}: has no nav mesh agent to move target. Please add one or turn off NavMeshSpawning / StartOutsideMainNavArea.");
                return;
            }

            if (communicator != null)
                communicator.StartedMoveOffNavMesh?.Invoke();

            Vector3? endPosition = GetEndPointOnNavMesh(agent.areaMask);
            if (endPosition == null)
            {
                Debug.LogError($"{_spawner.name}: cannot find end point on navmesh to move {mob.name} to. Please reposition end position or turn off StartOutsideMainNavArea.");
                return;
            }

            agent.SetDestination(endPosition.Value);
            _spawner.StartCoroutine(DestinationReached(agent, communicator));
        }

        public IEnumerator DestinationReached(NavMeshAgent agent, SpawnerCommunicator communicator)
        {
            yield return new WaitForSeconds(0.5f);
            while (agent != null && agent.remainingDistance > agent.stoppingDistance)
                yield return new WaitForEndOfFrame();

            bool agentHasArea = AreaMaskContains(agent, _data.startingNavArea);
            if (agentHasArea == false && agent != null && agent.remainingDistance <= agent.stoppingDistance)
                RemoveAreaMask(agent, _data.startingNavArea);

            if (communicator != null)
                communicator.FinishedMoveOffNavMesh?.Invoke();

            yield break;
        }

        /// <summary>
        /// Add a layer to the area mask of the agent
        /// </summary>
        /// <param name="areaName"></param>
        public void AddAreaMask(GameObject mob)
        {
            NavMeshAgent agent = mob.GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError($"{mob.name}: has no nav mesh agent to move target. Please add one or turn off NavMeshSpawning / StartOutsideMainNavArea.");
                return;
            }

            if (AreaMaskContains(agent, _data.startingNavArea))
                return;

            int area = NavMesh.GetAreaFromName(_data.startingNavArea);
            agent.areaMask += 1 << area;
        }

        public Vector3? GetEndPointOnNavMesh(int areaMask)
        {
            Vector3 endPosition = _data.endPosition.position;
            if (_data.useRandomPointNearEnd)
            {
                for (int i = 0; i < _obstacleAvoidance.AvoidanceAttemptLimit; i++)
                {
                    Vector3? newEnd = _pointPicker.GetRandomPositionInCircle(endPosition, _data.endRadius);
                    if (newEnd == null) continue;
                    newEnd = _obstacleAvoidance.SingleObstacleAvoidCheck(newEnd.Value, areaMask);
                    if (newEnd != null)
                    {
                        endPosition = newEnd.Value;
                        break;
                    }
                }
            }
            else
            {
                Vector3? newEnd = _obstacleAvoidance.SingleObstacleAvoidCheck(endPosition, areaMask);
                if (newEnd != null)
                    endPosition = newEnd.Value;
            }

            NavMeshHit hit;
            if (NavMesh.SamplePosition(endPosition, out hit, 1f, areaMask))
                return hit.position;

            return null;
        }

        /// <summary>
        /// Remove a layer from the area mask of the agent
        /// </summary>
        /// <param name="areaName"></param>
        private void RemoveAreaMask(NavMeshAgent agent, string areaName)
        {
            int area = NavMesh.GetAreaFromName(areaName);
            agent.areaMask -= 1 << area;
        }

        private bool AreaMaskContains(NavMeshAgent agent, string areaName)
        {
            string playerTagName = "Player";
            int l = LayerMask.NameToLayer(playerTagName);
            int area = NavMesh.GetAreaFromName(areaName);
            return agent.areaMask == (agent.areaMask | (1 << area));
        }
    }
}