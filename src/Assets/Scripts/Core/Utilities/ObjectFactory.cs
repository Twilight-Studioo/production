#region

using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Core.Utilities
{
    /// <summary>
    ///     キャラクターやエネミーは必ずこのファクトリを使って生成する
    /// </summary>
    public class ObjectFactory
    {
        private static GameObject superObject;

        private static ObjectFactory instance;
        private readonly List<GameObject> objects;

        private ObjectFactory()
        {
            objects = new();
            Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Subscribe(Update)
                .AddTo(SuperObject);
        }

        public static GameObject SuperObject
        {
            get
            {
                if (!superObject)
                {
                    lock (typeof(ObjectFactory))
                    {
                        if (!superObject)
                        {
                            superObject = new GameObject("SuperObject");
                            Object.DontDestroyOnLoad(superObject);
                        }
                    }
                }

                return superObject;
            }
        }
        public static ObjectFactory Instance => instance ??= new();
        public event Action<GameObject> OnObjectCreated;

        private void Update(long x)
        {
            objects.RemoveAll(obj => obj is null);
        }

        public GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var newObj = Object.Instantiate(prefab, position, rotation);
            OnObjectCreated?.Invoke(newObj);
            objects.Add(newObj);
            return newObj;
        }

        public GameObject CreateObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var newObj = Object.Instantiate(prefab, position, rotation, parent);
            OnObjectCreated?.Invoke(newObj);
            objects.Add(newObj);
            return newObj;
        }


        public GameObject FindObject(string name)
        {
            return objects.Find(obj => obj.name == name);
        }

        public GameObject FindPlayer()
        {
            return objects.Find(obj => obj != null && obj.activeSelf && obj.CompareTag("Player"));
        }
    }
}