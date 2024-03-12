using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aikom.AIEngine
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Gets a random point from this vector that is within specified distance
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Vector3 RandomWithinDistance(this Vector3 vec, float distance)
        {
            var rand = RandomVec3(-distance, distance);
            return rand + vec;
        }

        public static Vector3 Hadamar(this Vector3 vec, Vector3 other)
        {
            for(int i = 0; i < 2; i++)
            {
                vec[i] *= other[i];
            }
            return vec;
        }

        public static Vector3 RandomVec3(float min, float max)
        {
            return new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
        }
    }

}
