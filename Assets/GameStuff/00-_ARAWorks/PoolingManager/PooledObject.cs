using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Pooling
{
    public class PooledObject : MonoBehaviour
    {
        [Tooltip("The tag used to reference a pool.")]
        [SerializeField] private string poolTag = "";
        [Tooltip("The object will return to pool automatically after set duration.")]
        [SerializeField] private bool destroyAfterDuration = false;
        [Tooltip("The duration the object stays alive before returning to the pool.")]
        [SerializeField] private float duration = 0;

        private bool isInitialSpawn = true;
        public float Duration { get { return duration; } set { duration = value; } }

        private void OnEnable()
        {
            if (!isInitialSpawn)
            {
                if (PoolingManager.Instance != null && destroyAfterDuration)
                    PoolingManager.Instance.ReturnObjToPool(poolTag, gameObject, duration);
            }
            else
            {
                isInitialSpawn = false;
            }
        }

        private void OnDisable()
        {
            if (!isInitialSpawn)
            {
                if (PoolingManager.Instance != null && !destroyAfterDuration)
                    PoolingManager.Instance.ReturnObjToPool(poolTag, gameObject);
            }
            else
            {
                isInitialSpawn = false;
            }
        }
    }
}