using Opsive.BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyGame.BehaviourDesignerPro
{
    public class PatrolAndIdle : BasePatrolling
    {
        private bool _isWaiting = false;
        private uint wpID = 0;
        private Vector3 dir;

        private float _curWaitTime = 0.0f;
        private float _maxWaitTime = 3.5f;
        private bool _isStopped = false;

        public override void OnAwake()
        {
            base.OnAwake();
            //_waypointNavmeshHandler.WaypointReached += ReachedWaypoint;
        }

        public override TaskStatus OnUpdate()
        {
            //if no waypoint is provided, theres nothing to patrol. just return success.
            if (string.IsNullOrEmpty(waypointTag.Value)) return TaskStatus.Success;

            if (_isWaiting)
            {
                if (_curWaitTime > _maxWaitTime)
                {
                    _curWaitTime = 0;
                    _isWaiting = false;
                    //_waypointNavmeshHandler.canMove = true;
                    //_waypointNavmeshHandler.ResumePath();
                }
                else
                {
                    _curWaitTime += Time.deltaTime;
                }
            }
            else
            {
                return base.OnUpdate();
            }
            return TaskStatus.Success;
        }

        //private void ReachedWaypoint(IReadOnlyWaypoint wp)
        private void ReachedWaypoint()//(IReadOnlyWaypoint wp)
        {
            //_waypointNavmeshHandler.canMove = false;
            _isWaiting = true;
            //dir = wp.Position;
            //_agent.SetDestination(setStopDestin(dir));
            _aStarAgent.SetDestination(setStopDestin(dir));
            //_waypointNavmeshHandler.SuspendPath();
            //wpID = wp.ID;
        }

        private Vector3 setStopDestin(Vector3 destination)
        {
            Vector3 difference = destination - transform.parent.position;
            Vector3 direction = difference.normalized * 0.1f;
            return (transform.position + difference);
        }
    }
}