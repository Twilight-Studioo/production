#region

using System;
using UnityEngine;

#endregion

namespace Main.Factory
{
    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField] private GameObject enemyRef;

        private void Start()
        {
            var enemy = Instantiate(enemyRef.gameObject, new(0, 2, 0), Quaternion.identity);

            OnAddField?.Invoke(enemy);
        }

        public event Action<GameObject> OnAddField;
    }
}