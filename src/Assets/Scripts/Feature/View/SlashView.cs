using Feature.Interface;
using UnityEngine;

namespace Feature.View
{
    public class SlashView: MonoBehaviour
    {
        private uint damage;
        public void SetDamage(uint dmg)
        {
            damage = dmg;
        }
        private void OnTriggerEnter(Collider other)
        {
            var enemy = other.gameObject.GetComponent<IEnemy>()
                ?? other.gameObject.GetComponentInParent<IEnemy>();
            enemy?.OnDamage(damage, other.transform.position, transform);
        }
    }
}