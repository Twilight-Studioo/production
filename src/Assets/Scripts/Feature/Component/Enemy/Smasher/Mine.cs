using System;
using System.Linq;
using Core.Utilities;
using Feature.Interface;
using UnityEngine;

namespace Feature.Component.Enemy.Smasher
{
    /// <summary>
    /// Smasherが投げる地雷
    /// <warning>Smasherでのみ使用</warning>
    /// </summary>
    public class Mine: MonoBehaviour
    {
        private Collider mineCollider;
        public event Action OnDestroyed;
        private bool isReady;
        
        private Transform smasher;
        private Transform player;
        private void Awake()
        {
            isReady = false;
            mineCollider = GetComponent<Collider>();
        }

        public void Resume(Transform smasher)
        {
            player = ObjectFactory.Instance.FindPlayer()?.transform;
            this.smasher = smasher;
            isReady = true;
        }

        private void FixedUpdate()
        {
            if (!isReady)
            {
                return;
            }
            var hits = transform.GetBoxCastAll(mineCollider.bounds.size * 1.2f, Vector3.up, 0.1f, 10);
            var onHit = hits.Any(x => x.transform == smasher || x.transform == player);
            if (onHit)
            {
                LaunchExplosion();
            }
        }
        
        /// <summary>
        /// smasher自身が接近した時
        /// 確定でsmasherにダメージを与える
        internal void LaunchExplosionForSelf()
        {
            if (!isReady)
            {
                return;
            }

            if (smasher)
            {
                if (smasher.TryGetComponent(out IDamaged damageable))
                {
                    damageable.OnDamage(10, smasher.position, transform);
                }
            }
            LaunchExplosion();
        }

        internal void LaunchExplosion()
        {
            if (!isReady)
            {
                return;
            }
            Debug.Log("地雷の爆発");
            var hits = transform.GetBoxCastAll(mineCollider.bounds.size * 1.2f, Vector3.up, 0.1f, 10);
            foreach (var hit in hits)
            {
                if (hit.transform && hit.transform.TryGetComponent(out IDamaged damageable))
                {
                    damageable.OnDamage(10, hit.point, transform);
                }
            }
            Destroy(gameObject);
            OnDestroyed?.Invoke();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!isActiveAndEnabled || !isReady)
            {
                return;
            }
            if (other.gameObject != smasher.gameObject || other.gameObject != player.gameObject || other.gameObject == gameObject)
            {
                return;
            }
            if (other.gameObject.TryGetComponent(out IDamaged damageable))
            {
                damageable.OnDamage(10, other.contacts[0].point, transform);
            }
            Destroy(gameObject);
            OnDestroyed?.Invoke();
        }
    }
}