using System.Collections.Generic;
using Core.Utilities;
using UnityEngine;

namespace Feature.Component.Enemy.SmasherEx
{
    [RequireComponent(typeof(Collider))]
    public class DebrisSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject debrisPrefab;
        private readonly List<DebrisObject> debrisObjects = new();
        private readonly int maxDebrisCount = 40;
        private Collider spawnerCollider;

        private void Awake()
        {
            spawnerCollider = GetComponent<Collider>();
        }

        /// <summary>
        ///     per%のobjectを破壊する
        /// </summary>
        public void RandomLifeTimeDestroy(float per)
        {
            var count = (int)(debrisObjects.Count * per);
            for (var i = 0; i < count; i++)
            {
                var index = Random.Range(0, debrisObjects.Count);
                var debris = debrisObjects[index];
                debrisObjects.RemoveAt(index);
                debris.Delete();
            }
        }

        public void RandomSpawnDebris(int count)
        {
            if (spawnerCollider == null)
            {
                spawnerCollider = GetComponent<Collider>();
            }

            for (var i = 0; i < count; i++)
            {
                var randomPosition = new Vector3(
                    Random.Range(spawnerCollider.bounds.min.x, spawnerCollider.bounds.max.x),
                    transform.position.y,
                    transform.position.z
                );
                SpawnDebris(randomPosition);
            }
        }

        private void SpawnDebris(Vector3 position)
        {
            if (debrisObjects.Count >= maxDebrisCount)
            {
                return;
            }

            var item = ObjectFactory.Instance.CreateObject(debrisPrefab, position, Quaternion.identity);
            var debrisObject = item.GetComponent<DebrisObject>().CheckNull();
            debrisObjects.Add(debrisObject);
        }
    }
}