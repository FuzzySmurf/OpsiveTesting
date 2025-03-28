using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARAWorks.Base.Extensions
{
    public class Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
        public float area;

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            area = GetArea(Vector3.Distance(a, b), Vector3.Distance(a, c), Vector3.Distance(b, c));
        }

        /// <summary>
        /// Uses the cross product formula to calculate the triangle's area. 
        /// </summary>
        public static float GetArea(float a, float b, float c)
        {
            // Length of sides must be positive and sum of any two sides must be smaller than third side. 
            if (a < 0 || b < 0 || c < 0 || (a + b <= c) || a + c <= b || b + c <= a)
            {
                Debug.Log("Not a valid triangle");
            }
            float s = (a + b + c) / 2;
            return (float)System.Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }
    }

}