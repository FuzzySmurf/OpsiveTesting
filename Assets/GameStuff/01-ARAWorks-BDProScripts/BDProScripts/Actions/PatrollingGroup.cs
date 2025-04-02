using ARAWorks.BehaviourDesignerPro;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PolyGame.BehaviourDesignerPro
{
    public class PatrollingGroup : EnemyAction
    {
        //protected WaypointGroupHandler _waypointGroundHandler;

        public override void OnAwake()
        {
            //_waypointGroundHandler = transform.gameObject.GetComponentInParent<WaypointGroupHandler>();
            base.OnAwake();
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            //if (_waypointGroundHandler == null) return TaskStatus.Failure;

            GetNextFormation();
            NavivateToNextWaypoint();
            return TaskStatus.Failure;
            //return base.OnUpdate();
        }

        private void NavivateToNextWaypoint()
        {
            //if (_model == null) return;

            //_aStarAgent.SetDestination(_model.position);
        }

        private void GetNextFormation()
        {
            //if (_waypointGroundHandler != null)
            //{
            //    var form = _waypointGroundHandler.GetObjectFormation(_characterLocomotion.name, _characterLocomotion.gameObject.GetInstanceID());
            //    //if (form != null)
            //    //    _model = form;

            //}

        }
    }
}