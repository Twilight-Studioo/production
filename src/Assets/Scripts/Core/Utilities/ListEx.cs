using System.Collections.Generic;

namespace Core.Utilities
{
    public static class ListEx
    {
        public static T RandomElement<T>(this T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        public static T RandomElement<T>(this List<T> list)
        {
            if (list.Count == 0)
            {
                return default;
            }
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}