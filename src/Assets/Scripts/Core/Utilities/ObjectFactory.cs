#region

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Core.Utilities
{
    /// <summary>
    ///     キャラクターやエネミーは必ずこのファクトリを使って生成する
    /// </summary>
    public static class ObjectFactory
    {
        private static readonly List<GameObject> objects = new();
        public static event Action<GameObject> OnObjectCreated;

        public static GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var newObj = Object.Instantiate(prefab, position, rotation);
            OnObjectCreated?.Invoke(newObj);
            objects.Add(newObj);
            return newObj;
        }
        
        public static GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var newObj = Object.Instantiate(prefab, position, rotation, parent);
            OnObjectCreated?.Invoke(newObj);
            objects.Add(newObj);
            return newObj;
        }


        public static GameObject FindObject(string name)
        {
            return objects.Find(obj => obj.name == name);
        }

        public static GameObject FindPlayer()
        {
            return objects.Find(obj => obj.CompareTag("Player"));
        }
    }
}