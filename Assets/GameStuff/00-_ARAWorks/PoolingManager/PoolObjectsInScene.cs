using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Pooling
{
    public class PoolObjectsInScene : MonoBehaviour
    {
        [SerializeField] private List<PoolingManager.ObjectToPool> pooledObjects;

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            foreach (PoolingManager.ObjectToPool poolObj in pooledObjects)
            {
                if (poolObj.pooledObject != null && poolObj.poolTag != "" && poolObj.amountToPool != 0)
                    PoolingManager.Instance.CreateNewPool(poolObj.poolTag, poolObj.pooledObject, poolObj.amountToPool, poolObj.canExpandPool, transform, false);
            }
        }
    }
}