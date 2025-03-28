using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ARAWorks.Pooling;

#if MORE_EFFICIENT_COROUTINES
using MEC;
#endif

namespace ARAWorks.Spawner
{
    public class EnemySpawner : SpawnerMask
    {
        #region Serialized Properties
        /// <summary>
        /// Only used when same mobs every wave
        /// </summary>
        [SerializeField] private List<Mobs> mobs = new List<Mobs>();

        /// <summary>
        /// Only used with spawn next wave timer method
        /// </summary>
        [SerializeField] private float spawnTimer = 0;
        [SerializeField] private bool sameMobsEveryWave = true;
        [SerializeField][Range(1, 50)] private int numberOfWaves = 1;
        [SerializeField] private bool sameTimerEveryWave = true;
        [SerializeField] private SpawnNextWave spawnNextWave;
        /// <summary>
        /// Only used with spawn next wave continous kills method
        /// </summary>
        [SerializeField][Range(1, 100)] private int spawnContinueKillAmount = 1;
        /// <summary>
        /// Only used with spawn next wave continous kills method
        /// </summary>
        [SerializeField][Range(1, 100)] private int continuousAmountOutAtOnce = 1;
        /// <summary>
        /// Only used with spawn next wave continous kills method
        /// </summary>
        [SerializeField] private float continuousDelayWaves = 0;
        /// <summary>
        /// Only used with spawn next wave After Wave Killed method
        /// </summary>
        [SerializeField] private float delayBetweenWaves = 0;
        [SerializeField] private float delayBetweenSpawns = 0;
        [SerializeField] private SpawnerEventsHandler spawnerEvents;
        [SerializeField] private PlayerDetectionData playerDetectionData;
        #endregion

        #region Public Properties/Structs
        public IntObjDictionary Waves = IntObjDictionary.New<IntObjDictionary>();
#if UNITY_EDITOR
        public List<Mobs> currentWave;
        public int CurWaveTab = 0;
        public IntObjDictionary editorWaves = IntObjDictionary.New<IntObjDictionary>();
#endif

        public enum SpawnNextWave { Timer, After_Wave_Killed, Continuous_Kills };

        [System.Serializable]
        public class Wave
        {
            public List<Mobs> mobs;
            public float timer;
        }

        public string PlayerTag { get { return playerDetectionData.playerTag; } set { playerDetectionData.playerTag = value; } }
        internal List<SpawnedMob> SpawnedMobs = new List<SpawnedMob>();
        internal int CurrentSpawnedCount => SpawnedMobs.Count;
        #endregion

        #region Private Properties/Structs
        private bool canSpawn = true;
        private int wavesSpawned = 0;
        private PoolingManager poolingManager;
        private Quaternion startRot;
        private const string poolTagPrefix = "VoVSpawner";
        private List<ContinousSpawn> mobsForContinuous = new List<ContinousSpawn>();
        private bool isNewWave = false;
        private string coroutineTag = "EnemySpawner";
        private bool allEnemiesDead = false;
        private SpawnerRandomPointPicker randomPointPicker;
        private SpawnerObstacleAvoidanceHandler obstacleAvoidanceHandler;
        private SpawnerPlayerDetectionHandler playerDetectionHandler;
        private SpawnerNavMeshMovementHandler navMeshMovementHandler;


        private class ContinousSpawn
        {
            public Mobs mob;
            public int mobAmount;

            public ContinousSpawn(Mobs mob, int mobAmount)
            {
                this.mob = mob;
                this.mobAmount = mobAmount;
            }
        }
        #endregion

        #region Unity Callbacks
        private void Awake()
        {

            if (spawnMethod == SpawnMethod.Pooling)
            {
                if (PoolingManager.Instance != null)
                {
                    poolingManager = PoolingManager.Instance;
                    AddMobsToPool();
                }
                else
                {
                    spawnMethod = SpawnMethod.Instantiate;
#if UNITY_EDITOR
                    Debug.LogError("No pooling manager found. Spawn Method switched to instantiate.");
#endif
                }
            }

            randomPointPicker = new SpawnerRandomPointPicker(this);
            obstacleAvoidanceHandler = new SpawnerObstacleAvoidanceHandler(obstacleAvoidanceData, randomPointPicker, GroundLayerMask, name);
            playerDetectionHandler = new SpawnerPlayerDetectionHandler(playerDetectionData, poolingManager, this);
            navMeshMovementHandler = new SpawnerNavMeshMovementHandler(navMeshMoveData, randomPointPicker, obstacleAvoidanceHandler, this);
        }

        protected override void Start()
        {
            base.Start();

            startRot = Quaternion.Euler(transform.rotation.eulerAngles + StartRotation);
        }

        protected void OnDisable()
        {
            RestartSpawner();
        }

        public void Update()
        {
            if ((playerDetectionData.spawnOnLoad == false && playerDetectionHandler.PlayerNearby && canSpawn) || (playerDetectionData.spawnOnLoad && canSpawn))
            {
                if (spawnNextWave != SpawnNextWave.Continuous_Kills)
                {
                    if (wavesSpawned < numberOfWaves)
#if MORE_EFFICIENT_COROUTINES
                        Timing.RunCoroutine(StartSpawnCoroutine(), coroutineTag);
#else
                        StartCoroutine(StartSpawnCoroutine());
#endif
                }
                else
                {
                    if (wavesSpawned <= numberOfWaves)
                    {
#if MORE_EFFICIENT_COROUTINES
                        Timing.RunCoroutine(StartSpawnCoroutine(), coroutineTag);
#else
                        StartCoroutine(StartSpawnCoroutine());
#endif
                    }
                }
            }

            // If there are still mobs alive wait to finish enemies defeated
            if (wavesSpawned > numberOfWaves && CurrentSpawnedCount <= 0 && !allEnemiesDead)
            {
                spawnerEvents.Invoke_AllEnemiesDefeated();
                allEnemiesDead = true;
            }
        }

        private void FixedUpdate()
        {
            if (!playerDetectionData.spawnOnLoad)
            {
                if (spawnNextWave != EnemySpawner.SpawnNextWave.Continuous_Kills)
                {
                    if (wavesSpawned < numberOfWaves)
                        playerDetectionHandler.DetectPlayerNearby();
                }
                else
                {
                    if (wavesSpawned <= numberOfWaves)
                        playerDetectionHandler.DetectPlayerNearby();
                }
            }

            playerDetectionHandler.OnFixedUpdate();
        }
        #endregion

        #region Initialize
        private void AddMobsToPool()
        {
            // Get position to pool
            Vector3 poolPos = transform.position;

            if (useCustomInitialPoolPos == true)
                poolPos = customPoolPos.position + Vector3.up;

            if (Physics.Raycast(poolPos, Vector3.down, out RaycastHit hit, 1000, GroundLayerMask))
                poolPos = hit.point;

            // Pool all mobs
            if (sameMobsEveryWave)
            {
                foreach (Mobs mob in mobs)
                {

                    if (mob.prefab != null)
                        poolingManager.CreateNewPool(MakeSpawnerPoolTag(mob.prefab), mob.prefab, (mob.spawnRange.y * numberOfWaves), true, null, false, poolPos);
                }
            }
            else
            {
                for (int i = 0; i < numberOfWaves; i++)
                {
                    foreach (Mobs mob in Waves.dictionary[i].mobs)
                    {

                        if (mob.prefab != null)
                            poolingManager.CreateNewPool(MakeSpawnerPoolTag(mob.prefab), mob.prefab, mob.spawnRange.y, true, null, false, poolPos);
                    }
                }
            }
        }
        #endregion

        #region Spawn Enemies
#if MORE_EFFICIENT_COROUTINES
        private IEnumerator<float> StartSpawnCoroutine()
        {
            canSpawn = false;

            if (spawnNextWave != SpawnNextWave.Continuous_Kills)
            {
                // If wave has no mobs to spawn, continue to next wave
                if (sameMobsEveryWave || Waves.dictionary.ContainsKey(wavesSpawned) && Waves.dictionary[wavesSpawned].mobs.Count != 0)
                {
                    yield return Timing.WaitUntilDone(Timing.RunCoroutine(SpawnEnemyWave(), coroutineTag));
                    // Start waiting
                    yield return Timing.WaitUntilDone(Timing.RunCoroutine(SpawnEnemies(), coroutineTag));
                }
                else
                {
                    wavesSpawned++;
                    yield break;
                }
            }
            else
            {
                if (CheckContinuousAdvanceWaves())
                    AdvanceWavesInContinuous();

                if (!isNewWave && wavesSpawned <= numberOfWaves)
                    yield return Timing.WaitUntilDone(Timing.RunCoroutine(SpawnEnemiesContinuously(), coroutineTag));

                // Start waiting
                yield return Timing.WaitUntilDone(Timing.RunCoroutine(SpawnEnemies(), coroutineTag));
            }

            canSpawn = true;
            yield break;
        }
#else
        private IEnumerator StartSpawnCoroutine()
        {
            canSpawn = false;

            if (spawnNextWave != SpawnNextWave.Continuous_Kills)
            {
                // If wave has no mobs to spawn, continue to next wave
                if (sameMobsEveryWave || Waves.dictionary.ContainsKey(wavesSpawned) && Waves.dictionary[wavesSpawned].mobs.Count != 0)
                {
                    yield return StartCoroutine(SpawnEnemyWave());
                    // Start waiting
                    yield return StartCoroutine(SpawnEnemies());
                }
                else
                {
                    wavesSpawned++;
                    yield break;
                }
            }
            else
            {
                if (CheckContinuousAdvanceWaves())
                    AdvanceWavesInContinuous();

                if (!isNewWave && wavesSpawned <= numberOfWaves)
                    yield return StartCoroutine(SpawnEnemiesContinuously());

                // Start waiting
                yield return StartCoroutine(SpawnEnemies());
            }

            canSpawn = true;
            yield break;
        }
#endif

        /// <summary>
        /// Spawns all enemies
        /// </summary>
        /// <returns></returns>
#if MORE_EFFICIENT_COROUTINES
        private IEnumerator<float> SpawnEnemies()
        {
            // Check how to wait on spawning
            switch (spawnNextWave)
            {
                case SpawnNextWave.Timer:
                    if (sameTimerEveryWave || sameMobsEveryWave)
                        yield return Timing.WaitForSeconds(spawnTimer);
                    else
                        yield return Timing.WaitForSeconds(Waves.dictionary[wavesSpawned].timer);
                    spawnerEvents.Invoke_WaveEndEvent(wavesSpawned);
                    if (wavesSpawned >= numberOfWaves)
                    {
                        spawnerEvents.Invoke_AllWavesDoneEvent();
                        wavesSpawned++;
                    }
                    break;
                case SpawnNextWave.After_Wave_Killed:
                    yield return Timing.WaitUntilDone(Timing.RunCoroutine(CheckAllSpawnedDead()));
                    spawnerEvents.Invoke_WaveEndEvent(wavesSpawned);
                    if (wavesSpawned >= numberOfWaves)
                    {
                        spawnerEvents.Invoke_AllWavesDoneEvent();
                        wavesSpawned++;
                    }
                    if (delayBetweenWaves > 0)
                        yield return Timing.WaitForSeconds(delayBetweenWaves);
                    break;
                case SpawnNextWave.Continuous_Kills:
                    if (wavesSpawned <= numberOfWaves)
                        yield return Timing.WaitUntilDone(Timing.RunCoroutine(CheckCanContinueSpawning()));
                    if (isNewWave && CurrentSpawnedCount <= 0)
                    {
                        spawnerEvents.Invoke_WaveEndEvent(wavesSpawned - 1);
                        if (wavesSpawned >= numberOfWaves)
                        {
                            spawnerEvents.Invoke_WaveEndEvent(wavesSpawned);
                            spawnerEvents.Invoke_AllWavesDoneEvent();
                        }
                        yield return Timing.WaitForSeconds(continuousDelayWaves);
                        isNewWave = false;
                    }
                    break;
            }
            yield break;
        }

        private IEnumerator<float> CheckAllSpawnedDead()
        {
            while (CurrentSpawnedCount > 0)
                yield return Timing.WaitForSeconds(0.1f);
            yield break;
        }

        private IEnumerator<float> CheckCanContinueSpawning()
        {
            while (((continuousAmountOutAtOnce - CurrentSpawnedCount) < spawnContinueKillAmount)
                || (isNewWave && CurrentSpawnedCount > 0 && wavesSpawned <= numberOfWaves))
            {
                yield return Timing.WaitForSeconds(0.1f);
            }

            yield break;
        }
#else
        private IEnumerator SpawnEnemies()
        {
            // Check how to wait on spawning
            switch (spawnNextWave)
            {
                case SpawnNextWave.Timer:
                    if (sameTimerEveryWave || sameMobsEveryWave)
                        yield return new WaitForSeconds(spawnTimer);
                    else
                        yield return new WaitForSeconds(Waves.dictionary[wavesSpawned].timer);
                    spawnerEvents.Invoke_WaveEndEvent(wavesSpawned);
                    if (wavesSpawned >= numberOfWaves)
                    {
                        spawnerEvents.Invoke_AllWavesDoneEvent();
                        wavesSpawned++;
                    }
                    break;
                case SpawnNextWave.After_Wave_Killed:
                    yield return new WaitWhile(() => CurrentSpawnedCount > 0);
                    spawnerEvents.Invoke_WaveEndEvent(wavesSpawned);
                    if (wavesSpawned >= numberOfWaves)
                    {
                        spawnerEvents.Invoke_AllWavesDoneEvent();
                        wavesSpawned++;
                    }
                    if (delayBetweenWaves > 0)
                        yield return new WaitForSeconds(delayBetweenWaves);
                    break;
                case SpawnNextWave.Continuous_Kills:
                    if (wavesSpawned <= numberOfWaves)
                        yield return new WaitWhile(() => ((continuousAmountOutAtOnce - CurrentSpawnedCount) < spawnContinueKillAmount) || (isNewWave && CurrentSpawnedCount > 0 && wavesSpawned <= numberOfWaves));
                    if (isNewWave && CurrentSpawnedCount <= 0)
                    {
                        spawnerEvents.Invoke_WaveEndEvent(wavesSpawned - 1);
                        if (wavesSpawned >= numberOfWaves)
                        {
                            spawnerEvents.Invoke_WaveEndEvent(wavesSpawned);
                            spawnerEvents.Invoke_AllWavesDoneEvent();
                        }
                        yield return new WaitForSeconds(continuousDelayWaves);
                        isNewWave = false;
                    }
                    break;
            }
            yield break;
        }
#endif

        /// <summary>
        /// Spawns a wave of enemies
        /// </summary>
#if MORE_EFFICIENT_COROUTINES
        private IEnumerator<float> SpawnEnemyWave()
        {
            List<Mobs> mobWave;
            if (sameMobsEveryWave)
                mobWave = mobs;
            else
                mobWave = Waves.dictionary[wavesSpawned].mobs;

            if (mobWave.Count > 0)
            {
                spawnerEvents.Invoke_WaveStartEvent(wavesSpawned + 1);
                foreach (Mobs mob in mobWave)
                {
                    if (mob.prefab != null)
                    {
                        int amount = Random.Range(mob.spawnRange.x, mob.spawnRange.y + 1);
                        for (int i = 0; i < amount; i++)
                        {
                            bool mobSpawned = SpawnWaveMob(mob);
                            if (mobSpawned)
                                yield return Timing.WaitForSeconds(delayBetweenSpawns);
                        }
                    }
                }
            }
            wavesSpawned++;
            yield break;
        }
#else
        private IEnumerator SpawnEnemyWave()
        {
            List<Mobs> mobWave;
            if (sameMobsEveryWave)
                mobWave = mobs;
            else
                mobWave = Waves.dictionary[wavesSpawned].mobs;

            if (mobWave.Count > 0)
            {
                spawnerEvents.Invoke_WaveStartEvent(wavesSpawned + 1);
                foreach (Mobs mob in mobWave)
                {
                    int amount = Random.Range(mob.spawnRange.x, mob.spawnRange.y + 1);
                    for (int i = 0; i < amount; i++)
                    {
                        bool mobSpawned = SpawnWaveMob(mob);
                        if (mobSpawned)
                            yield return new WaitForSeconds(delayBetweenSpawns);
                    }
                }
            }
            wavesSpawned++;
            yield break;
        }
#endif

        /// <summary>
        /// Spawns a mob when using After wave Killed or Timer method for spawn next wave
        /// </summary>
        /// <param name="mob">the mob to spawn</param>
        /// <returns>Returns TRUE if a mob is spawned</returns>
        private bool SpawnWaveMob(Mobs mob)
        {
            Vector3 spawnPosition = GetSpawnPoint();

            if (spawnPosition != Vector3.zero)
            {
                SpawnMob(MakeSpawnerPoolTag(mob.prefab), mob, spawnPosition, startRot);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Spawns as many enemies as specified in the continuous spawning (Only for Continuous Kills Spawn Type)
        /// </summary>
#if MORE_EFFICIENT_COROUTINES
        private IEnumerator<float> SpawnEnemiesContinuously()
        {
            for (int i = CurrentSpawnedCount; i < continuousAmountOutAtOnce; i++)
            {
                bool mobSpawned = SpawnMobInContinuous();
                if (mobSpawned)
                {
                    // Causes a delay before the next mob can spawn
                    yield return Timing.WaitForSeconds(delayBetweenSpawns);
                }
                else
                {
                    // If not last wave revert i to spawn previous mob
                    if (wavesSpawned != numberOfWaves)
                        i--;
                    else
                        break;

                    // If a mob isn't spawned advance to next wave
                    AdvanceWavesInContinuous();

                    if (continuousDelayWaves > 0)
                        break;
                }
            }
            yield break;
        }
#else
        private IEnumerator SpawnEnemiesContinuously()
        {
            for (int i = CurrentSpawnedCount; i < continuousAmountOutAtOnce; i++)
            {
                bool mobSpawned = SpawnMobInContinuous();
                if (mobSpawned)
                {
                    // Causes a delay before the next mob can spawn
                    yield return new WaitForSeconds(delayBetweenSpawns);
                }
                else
                {
                    // If not last wave revert i to spawn previous mob
                    if (wavesSpawned != numberOfWaves)
                        i--;
                    else
                        break;

                    // If a mob isn't spawned advance to next wave
                    AdvanceWavesInContinuous();

                    if (continuousDelayWaves > 0)
                        break;
                }
            }
            yield break;
        }
#endif

        /// <summary>
        /// Spawn a mob when using Continuous Kills as spawn next wave method 
        /// </summary>
        /// <returns>Returns TRUE if a mob was spawned</returns>
        private bool SpawnMobInContinuous()
        {
            Vector3 spawnPosition = GetSpawnPoint();
            bool mobSpawned = false;
            if (spawnPosition != Vector3.zero)
            {
                for (int j = 0; j < mobsForContinuous.Count; j++)
                {
                    if (mobsForContinuous[j].mobAmount > 0)
                    {
                        SpawnMob(MakeSpawnerPoolTag(mobsForContinuous[j].mob.prefab), mobsForContinuous[j].mob, spawnPosition, startRot);
                        ContinousSpawn spawn = mobsForContinuous[j];
                        spawn.mobAmount--;
                        mobsForContinuous[j] = spawn;
                        mobSpawned = true;
                        break;
                    }
                }
            }
            return mobSpawned;
        }

        /// <summary>
        /// Spawn an enemy of given prefab
        /// </summary>
        /// <param name="poolTag">The tag to reference a specific pool in pooling manager</param>
        /// <param name="prefab">The prefab used to spawn an object</param>
        /// <param name="spawnPosition">The position to place the spawned object</param>
        /// <param name="rotation">The rotation of the spawned object</param>
        internal void SpawnMob(string poolTag, Mobs mob, Vector3 spawnPosition, Quaternion rotation)
        {
            if (UseRandomStartRotation)
                rotation = CalculateRandomSpawnRotation();

            GameObject spawn = null;

            switch (spawnMethod)
            {
                case SpawnMethod.Instantiate:
                    spawn = Instantiate(mob.prefab, spawnPosition, rotation);
                    break;

                case SpawnMethod.Pooling:
                    spawn = poolingManager.GetPooledObject(poolTag);
                    spawn.transform.position = spawnPosition;
                    spawn.transform.rotation = rotation;
                    break;
            }

            WarpAgentToSpawnPosition(spawn, spawnPosition);

            SpawnerCommunicator communicator = spawn.GetComponent<SpawnerCommunicator>();
            if (communicator != null)
                communicator.InitializeData(this, mob.level, mob.FollowWaypoint, mob.WaypointID, mob.WaypointTag);

            SpawnedMobs.Add(new SpawnedMob(poolTag, spawn, mob));

            bool startOutsideMainNavMesh = obstacleAvoidanceHandler.NavMeshSpawning && navMeshMoveData.startOutsideMainNavArea;
            if (startOutsideMainNavMesh)
                navMeshMovementHandler.AddAreaMask(spawn);




            if (startOutsideMainNavMesh)
                navMeshMovementHandler.MoveToEndPoint(spawn, communicator);
        }

        protected virtual void WarpAgentToSpawnPosition(GameObject spawn, Vector3 spawnPosition)
        {
            NavMeshAgent agent = spawn.GetComponent<NavMeshAgent>();
            if (agent != null)
                agent.Warp(spawnPosition);
        }
        #endregion

        private Vector3 GetSpawnPoint()
        {
            if (spawnType == SpawnType.Point)
                return transform.position;
            else
                return obstacleAvoidanceHandler.RaycastPosObstacleAvoid();
        }

        /// <summary>
        /// Calculate a random Y rotation between 0-360
        /// </summary>
        /// <returns>Returns a Quaternion with Eulers of (0, randomY, 0)</returns>
        private Quaternion CalculateRandomSpawnRotation()
        {
            float randomYRot = Random.Range(0, 361);
            Vector3 randomRot = new Vector3(0, randomYRot, 0);
            return Quaternion.Euler(randomRot);
        }

        #region Checking Functions
        /// <summary>
        /// Check if continuous kills spawning needs to advance waves
        /// </summary>
        /// <returns></returns>
        private bool CheckContinuousAdvanceWaves()
        {
            if (mobsForContinuous.Count > 0)
            {
                foreach (ContinousSpawn mob in mobsForContinuous)
                {
                    if (mob.mobAmount > 0)
                        return false;
                }
                return true;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region More Functions
        public void EnemyKilledRemoveFromSpawned(GameObject enemy)
        {
            playerDetectionHandler.EnemyKilledRemoveFromSpawned(enemy);
        }

        /// <summary>
        /// Resets Spawner (sets groups spawned to 0)
        /// </summary>
        public void RestartSpawner()
        {
            foreach (SpawnedMob mob in SpawnedMobs)
            {
                switch (SPAWNMETHOD)
                {
                    case SpawnMethod.Pooling:
                        if (mob.obj != null)
                            mob.obj.SetActive(false);
                        break;

                    case SpawnMethod.Instantiate:
                        if (mob.obj != null)
                            Destroy(mob.obj);
                        break;
                }
            }
            mobsForContinuous.Clear();
            playerDetectionHandler.ClearDisabledMobs();

#if MORE_EFFICIENT_COROUTINES
            Timing.KillCoroutines(coroutineTag);
#else
            StopCoroutine(StartSpawnCoroutine());
            if (spawnNextWave != SpawnNextWave.Continuous_Kills)
                StopCoroutine(SpawnEnemyWave());
            else
                StopCoroutine(SpawnEnemiesContinuously());
            StopCoroutine(SpawnEnemies());
#endif
            wavesSpawned = 0;
            allEnemiesDead = false;
            canSpawn = true;
        }

        public void DestroyObject(GameObject gameObject)
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Only used to advance to next wave in continuous kills spawning mode
        /// </summary>
        private void AdvanceWavesInContinuous()
        {
            mobsForContinuous.Clear();
            if (wavesSpawned != numberOfWaves)
            {
                spawnerEvents.Invoke_WaveStartEvent(wavesSpawned + 1);
                if (sameMobsEveryWave)
                {
                    foreach (Mobs mob in mobs)
                    {
                        if (mob.prefab != null)
                        {
                            int amount = Random.Range(mob.spawnRange.x, mob.spawnRange.y + 1);
                            mobsForContinuous.Add(new ContinousSpawn(mob, amount));
                        }
                    }
                }
                else
                {
                    foreach (Mobs mob in Waves.dictionary[wavesSpawned].mobs)
                    {
                        if (mob.prefab != null)
                        {
                            int amount = Random.Range(mob.spawnRange.x, mob.spawnRange.y + 1);
                            mobsForContinuous.Add(new ContinousSpawn(mob, amount));
                        }
                    }
                }
            }

            if (wavesSpawned != 0)
                isNewWave = true;

            wavesSpawned++;
        }

        /// <summary>
        /// Makes a unique pool tag for spawner mobs
        /// </summary>
        /// <param name="name">Mob's Name</param>
        /// <returns>Returns pool tag to spawn with</returns>
        private string MakeSpawnerPoolTag(GameObject prefab)
        {
            return string.Concat(poolTagPrefix, prefab.GetInstanceID().ToString());
        }

        /// <summary>
        /// If enemy is removed from spawnedMobs because it didn't disable/destroy immediate, then this is called when it disables/destroys
        /// </summary>
        /// <param name="enemy"></param>
        /// <param name="poolTag"></param>
        /*public void ReturnEnemyToPool(GameObject enemy, string poolTag)
        {
            foreach (SpawnedMob mob in spawnedMobs)
            {
                if (mob.obj != null && mob.obj == enemy)
                    return;
            }

            if (enemy.activeSelf == false)
                poolingManager.ReturnObjToPool(poolTag, enemy);
        }*/
        #endregion

        #region Gizmos
#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            if (ShowBoundsAlways && playerDetectionData.showRadiusOnlyWhenSelected == false)
                DrawGizmos();
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            if (ShowBoundsAlways == false || playerDetectionData.showRadiusOnlyWhenSelected)
                DrawGizmos();
        }

        private void DrawGizmos()
        {
            if (playerDetectionHandler == null)
                playerDetectionHandler = new SpawnerPlayerDetectionHandler(playerDetectionData, poolingManager, this);

            playerDetectionHandler.DrawGizmos();
        }

#endif
        #endregion
    }
}