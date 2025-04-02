using ARAWorks.BehaviourDesignerPro;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PolyGame.BehaviourDesignerPro
{
    public class BasePatrolling : EnemyAction
    {
        public int NextNodeNumber = 0;
        protected bool _destinationAssigned;

        //protected WaypointSceneController _waypointSceneController;
        //protected WaypointNavmeshHandler _waypointNavmeshHandler;
        public SharedVariable<string> waypointTag;
        //public SVIntList waypointInspectPoints;

        public override void OnAwake()
        {
            base.OnAwake();
            _destinationAssigned = false;

            DetermineWaypointPath();
        }

        public override void OnStart()
        {
            ResumeWaypointPath();
        }

        private void ResumeWaypointPath()
        {
            if (string.IsNullOrEmpty(waypointTag.Value)) return;

            //_waypointNavmeshHandler.ResumePath();
        }

        /// <summary>
        /// Used to Initialize and Determine the waypoint path used by the unit.
        /// </summary>
        private void DetermineWaypointPath()
        {
            if (string.IsNullOrEmpty(waypointTag.Value)) return;

            //_waypointSceneController = GameObject.FindObjectOfType<WaypointSceneController>();

            //_waypointNavmeshHandler = new WaypointNavmeshHandler(_waypointSceneController, _characterLocomotion.MoveTowardsAbility);
            //WaypointRequest request = new WaypointRequest(waypointTag.Value);
            //IReadOnlyWaypoint waypoint = _waypointNavmeshHandler.SceneController.GetWaypoint(request).FirstOrDefault();
            //_waypointNavmeshHandler.SetPath(waypoint.ID);

            //If there are 'inspect' points, we want to feed them to the handler.
            //if (waypointInspectPoints.Value.Any())
            //    _waypointNavmeshHandler.EnableInspection(waypointInspectPoints.Value.ToArray());
        }

        public override TaskStatus OnUpdate()
        {
            NavigateToNextWaypoint();
            return TaskStatus.Success;
        }

        protected void NavigateToNextWaypoint()
        {
            if (string.IsNullOrEmpty(waypointTag.Value)) return;

            //if (_waypointNavmeshHandler != null)
            //    _waypointNavmeshHandler.UpdateSimulation();
        }
    }
}