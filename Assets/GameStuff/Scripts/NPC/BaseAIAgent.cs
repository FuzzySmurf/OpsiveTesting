using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PolyGame.Character
{
    public class BaseAIAgent : MonoBehaviour
    {
        private bool _hasError = false;
        protected NavMeshAgent _backingAgent;

        // <summary>
        /// The current velocity of the NavMeshAgent component.
        /// </summary>
        public Vector3 velocity { get { return _agent.velocity; } }

        /// <summary>
        /// Gets the destination of the agent in world-space units.
        /// </summary>
        public Vector3 destination { get { return _agent.destination; } }

        [ShowInInspector, ReadOnly] public Vector3 currentVelocity { get; private set; }
        [ShowInInspector, ReadOnly] public bool IsMoving => currentVelocity.magnitude > 0;

        protected NavMeshAgent _agent
        {
            get
            {
                //Backing field used to handle errors regarding the navmesh agent
                AgentErrorCheck();
                return _backingAgent;
            }
        }

        protected virtual void Awake() { }

        // Start is called before the first frame update
        protected virtual void Start() { }


        private bool AgentErrorCheck()
        {
            //  NOTE:   If this still produces errors, then consider adding a boolean that will not notify this error system that
            //          an agent can be spawned without a navmesh or backing agent. This approach would be considered unsafe due to allowing 
            //          Unity to throw errors.


            //  Last erorr is used for checking if the issues has been resolved during runtime.
            //  If the issues has been logged once, it will not be logged again unless the issue was fixed and re-introduced.

            bool _lastError = _hasError;
            _hasError = false;


            if (_backingAgent == null)
            {
                _backingAgent = GetComponent<NavMeshAgent>();
                if (_backingAgent == null)
                {
#if UNITY_EDITOR
                    if (_lastError == false) Debug.LogError("AIAgent -- Unable to get NavMeshAgent from " + gameObject.name, gameObject);
#endif
                    _hasError = true;
                }
            }

            if (_backingAgent.enabled == true && _backingAgent?.isOnNavMesh == false)
            {
                //If this error is raised, more then likely the nav mesh was not generated for the provided agent type.
#if UNITY_EDITOR
                if (_lastError == false) Debug.Log("AIAgent -- Agent is not on a NavMesh.\nAgent Name: " + name + "\nAgent Type: " + NavMesh.GetSettingsNameFromID(_backingAgent.agentTypeID), gameObject);
#endif
                _hasError = true;
            }

            //Deactivate agent if errors were detected. Stops unforseen issues
            if (_hasError == true)
            {
                gameObject.SetActive(false);
                return true;
            }

            //Reactivates agent if errors were fixed
            if (_lastError == true)
            {
                gameObject.SetActive(true);
            }

            return false;
        }
    }
}