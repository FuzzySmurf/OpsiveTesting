using ARAWorks.Base.Extensions;
using ARAWorks.Contracts;
using ARAWorks.LevelManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Pooling
{
    public class PoolingManager : MonoBehaviour
    {
        public static PoolingManager Instance { get; private set; }

        [SerializeField] public StringObjDictionary pooledGroups = StringObjDictionary.New<StringObjDictionary>();

        [System.Serializable]
        public class PoolGroup
        {
            public List<ObjectToPool> pooledObjs;
        }

        [System.Serializable]
        public struct ObjectToPool
        {
            public string poolTag;
            public GameObject pooledObject;
            public int amountToPool;
            public bool canExpandPool;
        }

        // This section is only used for the Categories section which is helpful for the editor, but changes nothing on the backend
#if UNITY_EDITOR || !UNITY_2021_2_OR_NEWER
        [SerializeField] private string[] categories = new string[1];
        public string[] Categories => categories;
        public int curEditorTab = 0;
        public List<ObjectToPool> currentPoolGroup;
        public int curCategoryTab = 0;
        public StringObjDictionary editorPooledGroups = StringObjDictionary.New<StringObjDictionary>();
#endif

        private class PooledObjectData
        {
            public GameObject obj;
            public bool canExpandPool;
            public Transform parent;
            public int defaultAmount;
            public bool persistsBetweenScenes;

            public PooledObjectData(GameObject obj, bool canExpandPool, Transform parent, int defaultAmount, bool persistsBetweenScenes)
            {
                this.obj = obj;
                this.canExpandPool = canExpandPool;
                this.parent = parent;
                this.defaultAmount = defaultAmount;
                this.persistsBetweenScenes = persistsBetweenScenes;
            }
        }

        private class PooledObjects
        {
            public PooledObjectData pooledObjectData { get; private set; }
            public Queue<GameObject> queue { get; private set; }

            public PooledObjects(PooledObjectData pooledObjectData, Queue<GameObject> queue)
            {
                this.pooledObjectData = pooledObjectData;
                this.queue = queue;
            }
        }

        public bool IsTagInPool(string poolTag) => _pool.ContainsKey(poolTag);
        public bool CanTagExpand(string poolTag) => _pool.ContainsKey(poolTag) && _pool[poolTag].pooledObjectData.canExpandPool;
        public int GetAmountRemainingInPool(string poolTag) => _pool.ContainsKey(poolTag) ? _pool[poolTag].queue.Count : 0;


        private Dictionary<string, PooledObjects> _pool = new Dictionary<string, PooledObjects>();
        private Vector3 _defaultPosition = new Vector3(1000, 1000, 1000);


        protected virtual void OnEnable()
        {
            var newInstance = UnityExtensions.CreatePersistentInstance(Instance, this, gameObject);
            if (newInstance != null)
                Instance = newInstance;
        }

        void Start()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            // Add pool groups to the main pool
            if (pooledGroups.dictionary.Count > 0)
            {
                foreach (KeyValuePair<string, PoolGroup> poolGroup in pooledGroups.dictionary)
                {
                    foreach (ObjectToPool poolObj in poolGroup.Value.pooledObjs)
                    {
                        CreateNewPool(poolObj.poolTag, poolObj.pooledObject, poolObj.amountToPool, poolObj.canExpandPool, transform, true);
                    }
                }
            }
        }

        public void CreateNewPool(string poolTag, GameObject obj, int amount, bool canExpandPool, Transform parent, bool persistsBetweenScenes)
        {
            CreateNewPool(poolTag, obj, amount, canExpandPool, parent, persistsBetweenScenes, _defaultPosition);
        }

        public void CreateNewPool(string poolTag, GameObject obj, int amount, bool canExpandPool, Transform parent, bool persistsBetweenScenes, Vector3 position)
        {
            if (string.IsNullOrEmpty(poolTag))
            {
                Debug.LogError("Pool Tag is empty.");
                return;
            }
            else if (obj == null)
            {
                Debug.LogError("GameObject you're trying to pool is null.");
                return;
            }
            else if (amount <= 0)
            {
                Debug.LogError("The amount you're trying to pool is 0 or less.");
                return;
            }

            if (_pool.ContainsKey(poolTag) == false)
                _pool.Add(poolTag, new PooledObjects(new PooledObjectData(obj, canExpandPool, parent, amount, persistsBetweenScenes), new Queue<GameObject>()));

            SpawnObjects(poolTag, obj, amount, parent, position);
        }

        public void ReturnObjToPool(string poolTag, GameObject obj)
        {
            if (obj == null)
            {
                Debug.LogError($"Object is null, cannot be returned to pool.");
                return;
            }
            else if (_pool.ContainsKey(poolTag) == false)
            {
                Debug.LogError($"Object: {obj.name} cannot be returned to pool. Tag: {poolTag} does not exist.");
                return;
            }

            obj.transform.SetParent(_pool[poolTag].pooledObjectData.parent, true);
            _pool[poolTag].queue.Enqueue(obj);
            obj.SetActive(false);
        }

        public void ReturnObjToPool(string poolTag, GameObject obj, float timer)
        {
            StartCoroutine(ReturnAfterTimer(poolTag, obj, timer));
        }

        private IEnumerator ReturnAfterTimer(string poolTag, GameObject obj, float timer)
        {
            yield return new WaitForSeconds(timer);
            ReturnObjToPool(poolTag, obj);
            yield break;
        }

        public GameObject GetPooledObject(string poolTag)
        {
            if (_pool.ContainsKey(poolTag) == false)
                return null;

            if (_pool[poolTag].queue.Count > 0)
            {
                GameObject obj = _pool[poolTag].queue.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else if (_pool[poolTag].pooledObjectData.canExpandPool == true)
            {
                GameObject expandObj = ExpandPool(poolTag);
                expandObj.SetActive(true);
                return expandObj;
            }
            else
            {
                return null;
            }
        }

        private GameObject ExpandPool(string poolTag)
        {
            if (_pool.ContainsKey(poolTag) == false)
                return null;

            GameObject newObj = SpawnAnObject(_pool[poolTag].pooledObjectData.obj, _pool[poolTag].pooledObjectData.parent, _defaultPosition);
            _pool[poolTag].queue.Enqueue(newObj);
            return _pool[poolTag].queue.Dequeue();
        }

        private void SpawnObjects(string poolTag, GameObject objToSpawn, int amount, Transform parent, Vector3 position)
        {
            for (int i = 0; i < amount; i++)
            {
                GameObject newObj = SpawnAnObject(objToSpawn, parent, position);
                _pool[poolTag].queue.Enqueue(newObj);
            }
        }

        private GameObject SpawnAnObject(GameObject objToSpawn, Transform parent, Vector3 position)
        {
            GameObject obj;
            if (parent != null)
                obj = Instantiate(objToSpawn, position, Quaternion.identity, parent);
            else
                obj = Instantiate(objToSpawn, position, Quaternion.identity);

            obj.SetActive(false);

            return obj;
        }

        protected void OnSceneUnloaded(ContractLevel level)
        {
            if (level.levelType != ELevelType.GameLevel)
                return;

            Dictionary<string, PooledObjects> newPool = new Dictionary<string, PooledObjects>();
            foreach (KeyValuePair<string, PooledObjects> pool in _pool)
            {
                PooledObjectData pooledObjectData = pool.Value.pooledObjectData;

                if (pooledObjectData.persistsBetweenScenes == true && pooledObjectData.obj != null)
                {
                    Queue<GameObject> newQueue = new Queue<GameObject>();
                    // Remove null objects from queues
                    foreach (GameObject obj in pool.Value.queue)
                    {
                        if (obj != null)
                        {
                            newQueue.Enqueue(obj);
                            obj.SetActive(false);
                        }
                    }

                    int poolDefaultAmount = pooledObjectData.defaultAmount;
                    if (newQueue.Count < poolDefaultAmount)
                    {
                        for (int i = 0; i < poolDefaultAmount - newQueue.Count; i++)
                        {
                            GameObject newItem = SpawnAnObject(pooledObjectData.obj, pooledObjectData.parent, _defaultPosition);
                            newQueue.Enqueue(newItem);
                        }
                    }

                    newPool.Add(pool.Key, new PooledObjects(pooledObjectData, newQueue));
                }
            }

            // Cleanup any objects that may be persisting after their pool has already expired
            foreach (KeyValuePair<string, PooledObjects> pool in _pool)
            {
                if (newPool.ContainsKey(pool.Key)) continue;

                foreach (GameObject obj in pool.Value.queue)
                {
                    if (obj != null)
                        Destroy(obj);
                }
            }

            _pool = newPool;
        }

    }
}