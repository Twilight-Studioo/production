using Feature.Interface;
using UnityEngine;

namespace Feature.Component
{
    public class DamagedTrigger: MonoBehaviour
    {
        private uint damage = 0;
        
        private Vector3 direction = Vector3.zero;
        
        private float speed = 1.0f;
        
        private bool canHitEnemy = false;
        private bool canHitPlayer = false;
        
        public void SetHitObject(bool hitEnemy, bool hitPlayer)
        {
            canHitEnemy = hitEnemy;
            canHitPlayer = hitPlayer;
        }
        
        public void Execute(
            Vector3 dir,
            float s,
            uint d
        )
        {
            direction = dir;
            speed = s;
            damage = d;
        }

        private void FixedUpdate()
        {
            transform.position += direction * (speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((canHitPlayer && other.gameObject.CompareTag("Player")) || (canHitEnemy && other.gameObject.CompareTag("Enemy")))
            {
                other.gameObject.GetComponent<IDamaged>().OnDamage(damage, transform.position, transform);
                Destroy(gameObject);
            }
        }
    }
}