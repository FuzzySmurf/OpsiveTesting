using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpawnMethod = ARAWorks.Spawner.SpawnerMask.SpawnMethod;

namespace ARAWorks.Spawner
{
    [System.Serializable]
    internal class PlayerDetectionData
    {
        public bool restartSpawnerOnPlayerDistance = false;
        public bool spawnOnLoad = false;
        public bool disableOnPlayerDistance = false;
        public string playerTag = "";
        public bool alsoCheckForTag;
        public LayerMask playerLayer = new LayerMask();
        public bool dontResetMobOnDisable = false;
        [Range(1, 1000)] public float playerCheckRadius = 20;
        public bool detectFromSeperateObject = false;
        [Tooltip("The tag used to find players")]
        [Range(1, 5000)] public float playerDistance = 500;
        /// <summary>
        /// Only used when detecting from seperate object
        /// </summary>
        public Transform objectToDetectFrom;

#if UNITY_EDITOR
        public bool showSearchRadius = true;
        public bool showRadiusOnlyWhenSelected = true;
#endif
    }

    internal class SpawnedMob
    {
        public string poolTag;
        public GameObject obj;
        public Mobs mob;

        public SpawnedMob(string poolTag, GameObject obj, Mobs mob)
        {
            this.poolTag = poolTag;
            this.obj = obj;
            this.mob = mob;
        }
    }

    internal class DisabledMob
    {
        public string poolTag;
        public Vector3 position;
        public Quaternion rotation;
        public Mobs mob;
        public GameObject obj;

        public DisabledMob(string poolTag, Vector3 position, Quaternion rotation, Mobs mob, GameObject obj)
        {
            this.poolTag = poolTag;
            this.position = position;
            this.rotation = rotation;
            this.mob = mob;
            this.obj = obj;
        }
    }


    public class SpawnerPlayerDetectionHandler
    {
        public bool PlayerNearby { get; private set; } = false;

        private readonly PlayerDetectionData _playerDetectionData;
        private readonly object _poolingManager;
        private readonly EnemySpawner _spawner;
        private readonly List<DisabledMob> _mobsDisabled = new List<DisabledMob>();
        private readonly List<SpawnedMob> _mobsLateDisableKills = new List<SpawnedMob>();
        private readonly Collider[] hitColliders = new Collider[10];
        private int playersDetected = 0;

        // Disable on player distance variables
        private readonly GameObject[] _players;


        internal SpawnerPlayerDetectionHandler(PlayerDetectionData playerDetectionData, object poolingManager, EnemySpawner spawner)
        {
            _playerDetectionData = playerDetectionData;
            _poolingManager = poolingManager;
            _spawner = spawner;

            if (_playerDetectionData.disableOnPlayerDistance && !string.IsNullOrEmpty(_playerDetectionData.playerTag))
                _players = GameObject.FindGameObjectsWithTag(_playerDetectionData.playerTag);
            else if (_playerDetectionData.disableOnPlayerDistance && string.IsNullOrEmpty(_playerDetectionData.playerTag))
            {
                _playerDetectionData.disableOnPlayerDistance = false;
#if UNITY_EDITOR
                Debug.LogError("No Player Tag set for spawner \"" + _spawner.name + "\" disable on player distance now disabled.");
#endif
            }
        }

        public void OnFixedUpdate()
        {
            if (_playerDetectionData.restartSpawnerOnPlayerDistance)
                RestartOnPlayerDistance();

            if (_spawner.SpawnedMobs.Count > 0)
                HandleSpawnedMobsDisabling();

            // If player is close enough to enemy's last position then respawn the enemy
            if (_playerDetectionData.disableOnPlayerDistance && _mobsDisabled.Count > 0)
            {
                foreach (DisabledMob mob in _mobsDisabled)
                {
                    bool mobRespawned = RespawnOnPlayerDistance(mob);
                    if (mobRespawned)
                        break;
                }
            }

            if (_mobsLateDisableKills.Count > 0 && _spawner.SPAWNMETHOD == SpawnMethod.Pooling)
            {
                foreach (SpawnedMob mob in _mobsLateDisableKills)
                    ReturnKilledMobToPool(mob);
            }
        }

        /// <summary>
        /// If player is nearby set playerNearby bool to true;
        /// </summary>
        public void DetectPlayerNearby()
        {
            Vector3 detectionPos = _playerDetectionData.detectFromSeperateObject ? _playerDetectionData.objectToDetectFrom.position : _spawner.transform.position;
            playersDetected = Physics.OverlapSphereNonAlloc(detectionPos, _playerDetectionData.playerCheckRadius, hitColliders, _playerDetectionData.playerLayer);

            if (playersDetected > 0)
            {
                if (_playerDetectionData.alsoCheckForTag)
                {
                    foreach (Collider collider in hitColliders)
                    {
                        if (collider != null
                            && !string.IsNullOrEmpty(_playerDetectionData.playerTag)
                            && !string.IsNullOrEmpty(collider.tag)
                            && collider.CompareTag(_playerDetectionData.playerTag))
                        {
                            PlayerNearby = true;
                            return;
                        }
                    }
                    PlayerNearby = false;
                }
                else
                    PlayerNearby = true;
            }
            else
                PlayerNearby = false;
        }

        internal void ClearDisabledMobs()
        {
            _mobsDisabled.Clear();
        }

        private void RestartOnPlayerDistance()
        {
            bool playersAreTooFar = CheckIfAllPlayersTooFar(_spawner.transform.position);

            if (playersAreTooFar)
                _spawner.RestartSpawner();
        }

        /// <summary>
        /// Handle if a spawned mob should disable with player distance and what happens when it disables
        /// </summary>
        private void HandleSpawnedMobsDisabling()
        {
            switch (_spawner.SPAWNMETHOD)
            {
                case SpawnMethod.Pooling:
                    // If pooling check for mobs disabled to return to pool and also check for disable on distance if they're active
                    foreach (SpawnedMob mob in _spawner.SpawnedMobs)
                    {
                        bool mobReturnedToPool = ReturnKilledMobToPool(mob);
                        if (mobReturnedToPool)
                        {
                            _spawner.SpawnedMobs.Remove(mob);
                            HandleSpawnedMobsDisabling();
                            break;
                        }

                        if (_playerDetectionData.disableOnPlayerDistance)
                        {
                            if (_playerDetectionData.restartSpawnerOnPlayerDistance == false)
                            {
                                bool mobDisabled = DisableOnPlayerDistance(mob);
                                if (mobDisabled)
                                {
                                    //_poolingManager.ReturnObjToPool(mob.poolTag, mob.obj);
                                    HandleSpawnedMobsDisabling();
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case SpawnMethod.Instantiate:
                    foreach (SpawnedMob mob in _spawner.SpawnedMobs)
                    {
                        if (mob.obj == null)
                        {
                            _spawner.SpawnedMobs.Remove(mob);
                            HandleSpawnedMobsDisabling();
                            break;
                        }

                        // If not pooling, but still disabling on distance then destroy mob on distance
                        if (_playerDetectionData.disableOnPlayerDistance)
                        {
                            if (_playerDetectionData.restartSpawnerOnPlayerDistance == false)
                            {
                                bool mobDisabled = DisableOnPlayerDistance(mob);
                                if (mobDisabled)
                                {
                                    if (_playerDetectionData.dontResetMobOnDisable)
                                        mob.obj.SetActive(false);
                                    else
                                        _spawner.DestroyObject(mob.obj);
                                    HandleSpawnedMobsDisabling();
                                    break;
                                }
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// If enemy is killed and not immediately disabling/destroying this can be called
        /// </summary>
        /// <param name="enemy"></param>
        public void EnemyKilledRemoveFromSpawned(GameObject enemy)
        {
            foreach (SpawnedMob mob in _spawner.SpawnedMobs)
            {
                if (mob.obj != null && mob.obj.activeSelf && mob.obj == enemy)
                {
                    _spawner.SpawnedMobs.Remove(mob);

                    if (_spawner.SPAWNMETHOD == SpawnMethod.Pooling)
                        _mobsLateDisableKills.Add(mob);
                    break;
                }
            }
        }

        /// <summary>
        /// Return mob to pool if it's disabled (and also remove it from spawned mobs)
        /// </summary>
        private bool ReturnKilledMobToPool(SpawnedMob mob)
        {
            if (mob.obj.activeSelf == false)
            {
                //_poolingManager.ReturnObjToPool(mob.poolTag, mob.obj);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Disable a mob if all players are too far away from it
        /// </summary>
        /// <param name="mob"></param>
        /// <returns>Returns TRUE if the mob was disabled</returns>
        private bool DisableOnPlayerDistance(SpawnedMob mob)
        {
            bool playersAreTooFar = CheckIfAllPlayersTooFar(mob.obj.transform.position);

            if (playersAreTooFar)
            {
                DisabledMob disMob = new DisabledMob(mob.poolTag, mob.obj.transform.position, mob.obj.transform.rotation, mob.mob, mob.obj);
                _mobsDisabled.Add(disMob);
                _spawner.SpawnedMobs.Remove(mob);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Respawn the disabled mob if a player is close enough
        /// </summary>
        /// <param name="mob"></param>
        /// <returns>Returns TRUE if mob is respawned</returns>
        private bool RespawnOnPlayerDistance(DisabledMob mob)
        {
            bool aPlayerIsNear = !CheckIfAllPlayersTooFar(mob.position);
            // If a player is close then respawn enemy
            if (aPlayerIsNear)
            {
                if (_playerDetectionData.dontResetMobOnDisable)
                {
                    mob.obj.SetActive(true);
                    _spawner.SpawnedMobs.Add(new SpawnedMob(mob.poolTag, mob.obj, mob.mob));
                    _mobsDisabled.Remove(mob);
                }
                else
                {
                    _spawner.SpawnMob(mob.poolTag, mob.mob, mob.position, mob.rotation);
                    _mobsDisabled.Remove(mob);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns TRUE if all players are further than set distance from mobPos
        /// </summary>
        /// <param name="mobPos"> The position of the enemy</param>
        /// <returns></returns>
        private bool CheckIfAllPlayersTooFar(Vector3 mobPos)
        {
            if (_players.Length > 0)
            {
                foreach (GameObject player in _players)
                {
                    Vector3 offset = player.transform.position - mobPos;
                    float dis = offset.sqrMagnitude;

                    if (dis < _playerDetectionData.playerDistance * _playerDetectionData.playerDistance)
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

#if UNITY_EDITOR
        public void DrawGizmos()
        {
            if (_playerDetectionData.showSearchRadius)
            {
                Gizmos.color = Color.green;
                if (_playerDetectionData.detectFromSeperateObject == false)
                    Gizmos.DrawWireSphere(_spawner.transform.position, _playerDetectionData.playerCheckRadius);
                else if (_playerDetectionData.objectToDetectFrom != null)
                    Gizmos.DrawWireSphere(_playerDetectionData.objectToDetectFrom.position, _playerDetectionData.playerCheckRadius);
            }
        }
#endif
    }
}
