#region

using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.View
{
    public class SlashView : MonoBehaviour
    {
        private uint damage;

        private void OnTriggerEnter(Collider other)
        {
            var enemy = other.gameObject.GetComponent<IEnemy>()
                        ?? other.gameObject.GetComponentInParent<IEnemy>();
            enemy?.OnDamage(damage, other.transform.position, transform);
        }

        public void SetDamage(uint dmg)
        {
            damage = dmg;
        }
    }
}