using ARAWorks.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ARAWorks.Spawner
{
    [System.Serializable]
    public class Mobs
    {
        [Range(0, 100)] public int level = 1;
        public bool FollowWaypoint = false;
        public uint WaypointID;
        public string WaypointTag;
        public GameObject prefab;
        [MinMaxSlider(0, 100)] public Vector2Int spawnRange;
    }
}