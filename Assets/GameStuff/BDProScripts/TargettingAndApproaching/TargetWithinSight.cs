using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
using Opsive.GraphDesigner.Runtime.Variables;
using PolyGame.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARAWorks.Base.Extensions;
using PolyGame.Damage;

namespace ARAWorks.BehaviourDesignerPro
{
    public class TargetWithinSight : Conditional
    {
        private Vector3 _raycastStartPos { get { return transform.position + searchOffset.Value; } }
        private float _innerCircleFOV = 300;

        private Collider[] _foundColliders = new Collider[100];
        private RaycastHit[] _lineOfSightHits = new RaycastHit[50];

        [Tooltip("The behavior designer variable where we cache the target we find")]
        public SharedVariable<GameObject> returnedTarget;
        [Tooltip("The LayerMask of the target that we are searching for")]
        public SharedVariable<LayerMask> targetSearchLayerMask;
        [Tooltip("The LayerMask of the objects which will block your view")]
        public SharedVariable<LayerMask> viewBlockingLayers;
        [Tooltip("The field of view angle of the agent (in degrees)")]
        public SharedVariable<float> fieldOfViewAngle = 90;
        [Tooltip("The field of view distance of the agents small inner circle")]
        public SharedVariable<float> innerCircleViewDistance = 5;
        [Tooltip("The distance that the agent can see")]
        public SharedVariable<float> viewDistance = 15;
        [Tooltip("The raycast search offset relative to the pivot position of the searching character")]
        public SharedVariable<Vector3> searchOffset;
        [Tooltip("Should a debug look ray be drawn to the scene view?")]
        public SharedVariable<bool> drawDebugLines;

        public SharedVariable<ECharActions> characterActions;
        protected BaseCharacter _character;

        public override void OnAwake()
        {
            base.OnAwake();
            _character = GetComponent<BaseCharacter>();
            if (_character == null)
            {
                _character = this.transform.GetComponentInParent<BaseCharacter>();
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (characterActions.Value is ECharActions.FoundTarget
                                    or ECharActions.WithinChasingRange
                                    or ECharActions.WithinAttackRange
                                    or ECharActions.WithinFightingRange) return TaskStatus.Success;
            if (characterActions.Value is not ECharActions.None) return TaskStatus.Failure;

            if (targetSearchLayerMask == null)
            {
                Debug.LogError($"{gameObject.name}: target search LayerMask is unassigned in Behavior Tree");
                return TaskStatus.Failure;
            }

            GameObject foundTarget = DetectFirstValidTarget();

            if (foundTarget != null)
            {
                if (drawDebugLines.Value == true)
                    Debug.Log($"{gameObject.name} found target {foundTarget.name}");

                SetFoundTarget(foundTarget);
                return TaskStatus.Success;
            }

            characterActions.Value = ECharActions.None;
            return TaskStatus.Failure;
        }

        private void SetFoundTarget(GameObject gObj)
        {
            returnedTarget.Value = gObj;
            characterActions.Value = ECharActions.FoundTarget;
        }

        private GameObject DetectFirstValidTarget()
        {
            int hitTargets = Physics.OverlapSphereNonAlloc(_raycastStartPos, viewDistance.Value, _foundColliders, targetSearchLayerMask.Value);

            if (hitTargets > 0)
            {
                for (int i = 0; i < hitTargets; i++)
                {
                    // Make sure to skip any null fields
                    if (_foundColliders[i] == null) continue;

                    Transform potentialTarget = _foundColliders[i].transform;
                    float fovToSearch = fieldOfViewAngle.Value;
                    // Distance check for inner circle FOV
                    if (potentialTarget.position.WithinDistanceOf(this.transform.position, innerCircleViewDistance.Value) == true)
                        fovToSearch = _innerCircleFOV;

                    // Check if target is in FOV and a hostile faction
                    if (IsTargetInFOV(potentialTarget, fovToSearch) == true
                        && IsTargetFactionHostile(potentialTarget) == true)
                    {
                        // Check if target line of sight isn't blocked
                        if (TargetInLineOfSight(potentialTarget) == true)
                            return potentialTarget.gameObject;
                    }
                }
            }
            return null;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (drawDebugLines.Value == false) return;

            DrawConeAOE(this.transform);
        }

        private void DrawConeAOE(Transform transform)
        {
            if (transform == null) return;

            // Setup matrix and get root transform
            using (new UnityEditor.Handles.DrawingScope(Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one)))
            {
                // -------- Draw Front FOV

                // Calculate FOV angles
                float halfFOV = fieldOfViewAngle.Value / 2;
                Vector3 rightAngle = DirectionFromAngle(halfFOV);
                Vector3 leftAngle = DirectionFromAngle(-halfFOV);

                // Draw WireArc. WireArc draws left to right, so we use the left angle as the 'from' parameter
                UnityEditor.Handles.color = Color.cyan;
                UnityEditor.Handles.DrawWireArc(Vector3.zero, Vector3.up, leftAngle, fieldOfViewAngle.Value, viewDistance.Value);

                // Draw FOV angles
                UnityEditor.Handles.color = Color.green;
                UnityEditor.Handles.DrawLine(Vector3.zero, Vector3.zero + rightAngle * viewDistance.Value);
                UnityEditor.Handles.DrawLine(Vector3.zero, Vector3.zero + leftAngle * viewDistance.Value);

                // -------- Draw Inner Circle

                // Calculate FOV angles
                float inHalfFOV = _innerCircleFOV / 2;
                Vector3 inRightAngle = DirectionFromAngle(inHalfFOV);
                Vector3 inLeftAngle = DirectionFromAngle(-inHalfFOV);

                // Draw WireArc. WireArc draws left to right, so we use the left angle as the 'from' parameter
                UnityEditor.Handles.color = Color.green;
                UnityEditor.Handles.DrawWireArc(Vector3.zero, Vector3.up, inLeftAngle, _innerCircleFOV, innerCircleViewDistance.Value);

                // Draw FOV angles
                UnityEditor.Handles.color = Color.cyan;
                UnityEditor.Handles.DrawLine(Vector3.zero, Vector3.zero + inRightAngle * innerCircleViewDistance.Value);
                UnityEditor.Handles.DrawLine(Vector3.zero, Vector3.zero + inLeftAngle * innerCircleViewDistance.Value);
            }
        }

        private Vector3 DirectionFromAngle(float angle)
        {
            return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        }

        /// <summary>
        /// Checks if target is in Raycasts to target to deter
        /// </summary>
        /// <param name="target"></param>
        /// <returns>whether target is in view of character</returns>
        private bool IsTargetInFOV(Transform target, float fieldOfView)
        {
            Vector3 dir = target.position - this.transform.position;
            float angle = Vector3.Angle(dir, this.transform.forward);
            if (angle < (fieldOfView / 2))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks line of sight to target for whether it is blocked by some object
        /// </summary>
        /// <param name="target"></param>
        /// <returns>First hit in target search LayerMask</returns>
        private bool TargetInLineOfSight(Transform target)
        {
            Vector3 dir = (target.position - this.transform.position).normalized;
            LayerMask hittableLayers = viewBlockingLayers.Value.Combine(targetSearchLayerMask.Value);
            Ray ray = new Ray(_raycastStartPos + this.transform.up, dir);
            int hits = Physics.RaycastNonAlloc(ray, _lineOfSightHits, viewDistance.Value + 5, hittableLayers);
            if (hits > 0)
            {
                _lineOfSightHits.SortByDistance(hits);
                for (int i = 0; i < hits; i++)
                {
                    if (_lineOfSightHits[i].transform == target)
                        return true;
                    else if (HighestParent(target).transform == _lineOfSightHits[i].transform)
                        return true;
                    else if (viewBlockingLayers.Value.Contains(_lineOfSightHits[i].collider.gameObject.layer) == true)
                        return false;
                }
            }

            return false;
        }

        private GameObject HighestParent(Transform target)
        {
            if (target.parent != null)
                return HighestParent(target.parent);
            return target.gameObject;
        }

        /// <summary>
        /// Check if target's faction is hostile
        /// </summary>
        /// <param name="target"></param>
        /// <returns>whether target's faction is hostile</returns>
        private bool IsTargetFactionHostile(Transform target)
        {
            ITargetExt targetChar = target.GetComponent<ITargetExt>();

            if (InterfaceHelper.IsNull(targetChar))
            {
                if (target.parent != null)
                    return IsTargetFactionHostile(target.parent);
                else
                    return false;
            }

            //check If Target is friendly..
            //if (_character.Faction.CheckFriendlyStatus(targetChar.Faction.FactionID))
            //    return false;

            return true;
        }
    }
}