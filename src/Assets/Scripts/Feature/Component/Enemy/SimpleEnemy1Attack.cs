#region

using UnityEngine;

#endregion

namespace Feature.Component.Enemy
{
    public class SimpleEnemy1Attack : MonoBehaviour
    {
        private SimpleEnemy1Agent enemy;

        private void Awake()
        {
            enemy = GetComponentInParent<SimpleEnemy1Agent>();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                enemy.TakeDamage();
            }
        }
    }
}