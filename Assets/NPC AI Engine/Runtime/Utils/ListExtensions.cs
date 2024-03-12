using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Aikom.AIEngine.Utils
{
    /// <summary>
    /// List utility extensions
    /// </summary>
    /// <remarks>
    /// https://stackoverflow.com/a/1262619
    /// </remarks>
    public static class ListExtensions
    {
        public static void Suffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

}
