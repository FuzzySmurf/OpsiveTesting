using ARAWorks.Base.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Spawner
{
    public class SpawnerRandomPointPicker
    {
        private SpawnerMask _spawnerMask;

        public SpawnerRandomPointPicker(SpawnerMask spawnerMask)
        {
            _spawnerMask = spawnerMask;
        }

        /// <summary>
        /// Calculate position within the spawner to spawn
        /// </summary>
        /// <returns>The position to spawn</returns>
        public Vector3 GetSpawnPosition()
        {
            Triangle randomTri = PickRandomTriangle();
            Vector2 point = RandomPointInTriangle(randomTri);
            return new Vector3(point.x, randomTri.a.y + 1, point.y);
        }

        public Vector3 GetRandomPositionInCircle(Vector3 center, float radius)
        {
            Vector2 circleCenter = new Vector2(center.x, center.z);
            Vector2 randomPoint = circleCenter + (Random.insideUnitCircle * radius);
            return new Vector3(randomPoint.x, center.y, randomPoint.y);
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

        /// <summary>
        /// Find a random point within a triangle in the spawner
        /// </summary>
        /// <param name="tri"> The triangle to find point within</param>
        /// <returns>Returns random point within given triangle</returns>
        private Vector2 RandomPointInTriangle(Triangle tri)
        {
            var r1 = Mathf.Sqrt(Random.Range(0f, 1f));
            var r2 = Random.Range(0f, 1f);
            var m1 = 1 - r1;
            var m2 = r1 * (1 - r2);
            var m3 = r2 * r1;

            Vector2 p1 = new Vector2(tri.a.x, tri.a.z);
            Vector2 p2 = new Vector2(tri.b.x, tri.b.z);
            Vector2 p3 = new Vector2(tri.c.x, tri.c.z);
            return (m1 * p1) + (m2 * p2) + (m3 * p3);
        }

        /// <summary>
        /// Find a random triangle within spawner area
        /// </summary>
        /// <returns>Return a random triangle from spawner area</returns>
        private Triangle PickRandomTriangle()
        {
            float rng = Random.Range(0f, _spawnerMask.TotalArea);
            for (int i = 0; i < _spawnerMask.Triangles.Count; ++i)
            {
                if (rng < _spawnerMask.Triangles[i].area)
                {
                    return _spawnerMask.Triangles[i];
                }
                rng -= _spawnerMask.Triangles[i].area;
            }
            // Should normally not get here
            return _spawnerMask.Triangles[0];
        }
    }
}