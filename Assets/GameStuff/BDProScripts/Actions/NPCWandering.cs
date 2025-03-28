using ARAWorks.Base.Extensions;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.GraphDesigner.Runtime.Variables;
using Opsive.UltimateCharacterController.Character.Abilities;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ARAWorks.BehaviourDesignerPro
{
    public class NPCWandering : EnemyAction
    {
        [Tooltip("The search radius for the NPC to determine a wandering point.")]
        public SharedVariable<float> radius = 5.0f;
        [Tooltip("Add obstacles that may be on the scene that should be ignored.")]
        public SharedVariable<LayerMask> obstaclesToAvoidMask;
        [Tooltip("Add the groundMask to here to find navmesh.")]
        public SharedVariable<LayerMask> groundMask;
        public SharedVariable<Vector3> _destination;
        public SharedVariable<bool> _hasReachedDestination;
        public SharedVariable<bool> _canGetNewDestination;
        public SharedVariable<float> stoppingDistance;
        private bool _navMeshSpawning = false;
        private float _radiusWithMoveTowards { get { return radius.Value + 0.5f; } }
        //Used as the Tether position.
        private Vector3 _startingPosition;
        private Vector3 _dest;

        public SharedVariable<GameObject> primitiveLookAt;
        protected RotateTowards _rotationTowards;

        //Used to measure the last 'set' position used.
        private Vector3 _lastPos;
        private float _curTime;
        private float _maxTime = 1.0f;
        private AIDestinationSetter _pathDestination;

        public override void OnAwake()
        {
            base.OnAwake();
            _rotationTowards = _characterLocomotion.GetAbility<RotateTowards>();
        }

        public override void OnStart()
        {
            //set Initial Position.
            if (_startingPosition == Vector3.zero)
                _startingPosition = this.transform.position;

            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            //Find next Position to set.
            SetNextDestinationPoint();

            //Set Character Rotation. (rotation speed is m_CharacterLocomotion.MotorRotationSpeed).
            //RotateTowardsDestination();

            //Check if we've reached the destination set.
            CheckWithinRange();

            //In case we're stuck looking for a destination. (might not be needed?)
            //WhenStuckFindNewSpot();

            return base.OnUpdate();
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            DrawAreaWandering();
        }

        private void RotateTowardsDestination()
        {
            //we only need to update if the NPC is still far off from the position.
            var dist = Vector3.Distance(_destination.Value, this.transform.position);
            if (dist > 0.1f)
            {
                float adjValue = 1.05f;
                Vector3 diff = _destination.Value - this.transform.parent.position;
                Vector3 dir = diff.normalized;
                float xVal = (dir.x > 0) ? _destination.Value.x + adjValue : _destination.Value.x - adjValue;
                float zVal = (dir.z > 0) ? _destination.Value.z + adjValue : _destination.Value.z - adjValue;
                primitiveLookAt.Value.transform.position = new Vector3(xVal, _destination.Value.y, zVal);
                _rotationTowards.Target = primitiveLookAt.Value.transform;
            }
        }

        /// <summary>
        /// If the NPC hasn't moved in more then 0.5 seconds, and hasn't reached the 'destination' point.
        /// have it search for a new navmesh Point.
        /// </summary>
        /// <remarks> Might not need this but leave it for now. (11/30/24)</remarks>
        private void WhenStuckFindNewSpot()
        {
            //if they can get a new destination, character is not stuck.
            if (_canGetNewDestination.Value) return;
            //if they're within range, no point in checking.
            if (DistanceFromDestination()) return;

            if (_lastPos == Vector3.zero)
            {
                _lastPos = this.transform.position;
                return;
            }

            if (_curTime < _maxTime)
            {
                _curTime = _curTime + Time.deltaTime;
                return;
            }

            //The Character hasn't really moved. Assume they hit a wall. find a new position.
            var dist = Vector3.Distance(_lastPos, transform.position);
            if (dist < 0.01f)
            {
                _curTime = 0.0f;
                _canGetNewDestination.Value = true;
            }

            _lastPos = transform.position;
        }

        private void CheckWithinRange()
        {
            if (_hasReachedDestination.Value) return;
            if (_canGetNewDestination.Value) return;
            if (!DistanceFromDestination()) return;

            _hasReachedDestination.Value = true;
        }

        private void SetNextDestinationPoint()
        {
            if (_destination.Value == Vector3.zero || _canGetNewDestination.Value)
            {
                _dest = GetRandomPointInSphere(_radiusWithMoveTowards);
                //if new position is Within stopping distance, search for a new value.
                if (Vector3.Distance(transform.position, _dest) < stoppingDistance.Value)
                {
                    _dest = GetRandomPointInSphere(_radiusWithMoveTowards);
                }
                //_characterLocomotion.MoveTowardsAbility.MoveTowardsLocation(_dest);
                _aStarAgent.SetDestination(_dest);
                _destination.Value = _dest;
                //we got a new destination, set to false.
                _canGetNewDestination.Value = false;
            }
        }

        private bool DistanceFromDestination()
        {
            return Vector3.Distance(this.transform.position, _dest) < stoppingDistance.Value;
        }

        public Vector3 GetRandomPointInSphere(float rad)
        {
            // Generate a random point within a unit sphere
            Vector3 randomOffset = Random.insideUnitSphere * rad;
            //use the higher height + offset value.
            float yPosA = (randomOffset.y > transform.position.y) ? randomOffset.y : transform.position.y;
            //add an offset height
            float yPosB = yPosA + 3.0f;
            //Vector3 playerOffset = new Vector3(randomOffset.x, yPosB, randomOffset.z);
            Vector3 desiredDestination = new Vector3(_startingPosition.x + randomOffset.x, yPosB, _startingPosition.z + randomOffset.z);

            Vector3? navMeshDestination = ObstacleAvoidanceCheck(desiredDestination);

            if (navMeshDestination == null)
            {
                Debug.Log($"Could not find OffsetValue for <color=yellow>{this.gameObject.name}</color>");
                return _startingPosition;
            }

            // Add the offset to the center to get the final point
            return navMeshDestination.Value;
        }

        /// <summary>
        /// Used to raycast down and determine what was hit. Will determine if the target was an 
        /// obstacle or ground.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="navArea"></param>
        /// <returns></returns>
        private Vector3? ObstacleAvoidanceCheck(Vector3 position)
        {
            RaycastHit hit;
            LayerMask combined = obstaclesToAvoidMask.Value + groundMask.Value;
            if (Physics.Raycast(position, Vector3.down, out hit, 1000f, groundMask.Value))
            {
                if (groundMask.Value.Contains(hit.collider.gameObject.layer))
                {
                    if (!_navMeshSpawning)
                    {
                        Vector3? navMeshPoint = CheckOnNavMesh(hit.point);

                        if (navMeshPoint != null)
                            return navMeshPoint.Value;
                        else
                            Debug.Log("Could not find NavMeshSample Position");
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
        /// Determines if pos is on navmash
        /// </summary>
        /// <param name="pos">Position to check if is on navmesh</param>
        /// <returns>True if it's on navmesh, False if it is not </returns>
        private Vector3? CheckOnNavMesh(Vector3? pos)
        {
            if (pos == null) return null;

            NavMeshHit navHit;
            NNInfo inf = AstarPath.active.GetNearest(pos.Value);
            return inf.position;
            //if (NavMesh.SamplePosition(pos.Value, out navHit, 10f, navArea))
            //    return navHit.position;
            //else
            //    return null;
        }

        //Draw the wandering area for the NPC.
        private void DrawAreaWandering()
        {
            using (new UnityEditor.Handles.DrawingScope(Matrix4x4.TRS(_startingPosition, Quaternion.identity, Vector3.one)))
            {
                UnityEditor.Handles.color = Color.cyan;
                UnityEditor.Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.forward, 360, radius.Value);
            }
        }
    }
}
