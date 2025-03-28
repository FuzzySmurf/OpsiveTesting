using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Spawner
{
    public struct V3LineSeg
    {
        public Vector3 Point0;
        public Vector3 Point1;
        public Vector3 Center;
        public Vector3 Direction;
        public float Extent;

        public V3LineSeg(Vector3 point0, Vector3 point1)
        {
            Point0 = point0;
            Point1 = point1;
            Center = Direction = Vector3.zero;
            Extent = 0f;
            CalcDir();
        }

        public void CalcDir()
        {
            Center = 0.5f * (Point0 + Point1);
            Direction = Point1 - Point0;
            var directionLength = Direction.magnitude;
            var invDirectionLength = 1f / directionLength;
            Direction *= invDirectionLength;
            Extent = 0.5f * directionLength;
        }

        public float DistanceTo(Vector3 point)
        {
            return Mathf.Sqrt(SqrPoint3Segment3(ref point, ref this));
        }

        public static float SqrPoint3Segment3(ref Vector3 point, ref V3LineSeg segment)
        {
            var diff = point - segment.Center;
            var param = Vector3.Dot(segment.Direction, diff);
            Vector3 closestPoint;
            if (-segment.Extent < param)
            {
                if (param < segment.Extent)
                {
                    closestPoint = segment.Center + param * segment.Direction;
                }
                else
                {
                    closestPoint = segment.Point1;
                }
            }
            else
            {
                closestPoint = segment.Point0;
            }
            diff = closestPoint - point;
            return diff.sqrMagnitude;
        }
    }
}