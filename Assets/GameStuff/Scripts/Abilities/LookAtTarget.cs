using Opsive.UltimateCharacterController.Character.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARAWorks;
using System;
using Opsive.UltimateCharacterController.Character;

namespace PolyGame.Abilities
{
    public class LookAtTarget : LookAt
    {
        public bool willRotateToTarget = false;
        protected CharacterIK _characterIK = null;
        public override void Awake()
        {
            base.Awake();
            if ((m_ObjectDetection & ObjectDetectionMode.Customcast) != 0) {
                m_DetectedTriggerObjects = new GameObject[m_MaxTriggerObjectCount];
                Debug.LogWarning($"Warning! We're using 'LookAtTarget' without CustomCast. Is this what you want?");
            }
            _characterIK = m_GameObject.GetComponentInChildren<CharacterIK>();
        }

        public override void Update()
        {
            //we're assuming the CharacterIK exists, because it works. if it fails, 
            try
            {
                if (_characterIK != null) base.Update();
            } catch {
                //If this fails, then the Characters Avatar might be set to Generic.
                //this component needs access to the 'head' component to trigger 'Look At' correctly.
                Debug.LogError($"LookAt is failing. Check if the CharacterIK for {m_GameObject.name} is set correctly. otherwise Remove CharacterIK from this character.");
            }
            

            if (willRotateToTarget) RotateToLookAtTarget();
        }

        private void RotateToLookAtTarget()
        {
            // Determine the direction that the character should be facing.
            var transformRotation = m_Transform.rotation;
            var lookDirection = Vector3.ProjectOnPlane(m_LookSource.LookDirection(m_LookSource.LookPosition(true), true, m_CharacterLayerManager.IgnoreInvisibleCharacterLayers, false, false), m_CharacterLocomotion.Up);
            var rotation = transformRotation * m_CharacterLocomotion.DesiredRotation;
            var targetRotation = Quaternion.LookRotation(lookDirection, rotation * Vector3.up);
            m_CharacterLocomotion.DesiredRotation = (Quaternion.Inverse(transformRotation) * targetRotation);
        }

        public void AddObjectDetected(GameObject gObj)
        {
            //we only want to use this if its using our CUSTOM object Detection.
            if((m_ObjectDetection & ObjectDetectionMode.Customcast) == 0){
                return;
            }

            // Ensure the detected object isn't duplicated within the list.
            for (int i = 0; i < m_DetectedTriggerObjectsCount; ++i) {
                if (m_DetectedTriggerObjects[i] == gObj) {
                    return;
                }
            }

            if (ValidateObject(gObj, null))
            {
                if (m_DetectedTriggerObjects.Length == m_DetectedTriggerObjectsCount)
                {
                    Debug.LogError($"Error: The maximum number of trigger objects should be increased on the {GetType().Name} ability.", m_GameObject);
                    return;
                }
                m_DetectedTriggerObjects[m_DetectedTriggerObjectsCount] = gObj;
                m_DetectedTriggerObjectsCount++;
            }
        }

        public bool RemoveObjectDetected(GameObject gObj)
        {
            for (int i = 0; i < m_DetectedTriggerObjectsCount; ++i)
            {
                if (gObj == m_DetectedTriggerObjects[i] || m_DetectedTriggerObjects[i] == null)
                {
                    m_DetectedTriggerObjects[i] = null;
                    // Ensure there is not a gap in the trigger object elements.
                    for (int j = i; j < m_DetectedTriggerObjectsCount - 1; ++j)
                    {
                        m_DetectedTriggerObjects[j] = m_DetectedTriggerObjects[j + 1];
                    }
                    m_DetectedTriggerObjectsCount--;
                    // The detected object should be assigned to the oldest trigger object. This value may be null.
                    DetectedObject = m_DetectedTriggerObjects[0];
                    return true;
                }
            }
            return false;
        }
    }
}