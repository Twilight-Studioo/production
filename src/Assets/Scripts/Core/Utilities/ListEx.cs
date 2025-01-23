using System.Collections.Generic;
using UnityEngine;

namespace Core.Utilities
{
    public static class ListEx
    {
        public static T RandomElement<T>(this T[] array) => array[Random.Range(0, array.Length)];

        public static T RandomElement<T>(this List<T> list)
        {
            if (list.Count == 0)
            {
                return default;
            }

            return list[Random.Range(0, list.Count)];
        }
    }
}