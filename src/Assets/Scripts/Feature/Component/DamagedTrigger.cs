#region

using System;
using Feature.Interface;
using UniRx;
using UnityEngine;

#endregion

namespace Feature.Component
{
    public class DamagedTrigger : MonoBehaviour, ISwappable
    {
        private readonly IReactiveProperty<Vector2> position = new ReactiveProperty<Vector2>();

        private bool canHitEnemy;
        private bool canHitField;
        private bool canHitPlayer;
        private uint damage;

        private Vector3 direction = Vector3.zero;

        private float lastUpdateDirectionTime;

        private float speed = 1.0f;

        private bool swapped;

        private Transform target;

        private void Update()
        {
            position.Value = transform.position;
        }

        private void FixedUpdate()
        {
            if (target != null && Time.time - lastUpdateDirectionTime > 0.2f)
            {
                var newDir = (target.position - transform.position).normalized;
                direction = Vector3.Lerp(direction, newDir, 0.1f);
                lastUpdateDirectionTime = Time.time;
            }

            transform.position += direction * (speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (
                (canHitPlayer && other.gameObject.CompareTag("Player"))
                || (canHitEnemy && other.gameObject.CompareTag("Enemy"))
                || (canHitField && other.gameObject.CompareTag("Ground")))
            {
                var damaged = other.gameObject.GetComponent<IDamaged>();
                damaged?.OnDamage(damage, transform.position, transform);
                OnHitEvent?.Invoke();
                TryDestroy();
            }
        }

        public event Action OnDestroyEvent;

        public void Delete()
        {
            OnDestroyEvent?.Invoke();
            Destroy(gameObject);
        }

        public void OnSelected()
        {
        }

        public void OnDeselected()
        {
        }

        public void OnInSelectRange()
        {
        }

        public void OnOutSelectRange()
        {
        }

        public IReadOnlyReactiveProperty<Vector2> GetPositionRef() => position;


        public Vector2 GetPosition() => transform.position;

        public void OnSwap(Vector2 p)
        {
            transform.position = p;
            swapped = true;
        }

        public event Action OnHitEvent;

        public void SetHitObject(bool hitEnemy, bool hitPlayer, bool field)
        {
            canHitEnemy = hitEnemy;
            canHitPlayer = hitPlayer;
            canHitField = field;
        }

        public void ExecuteWithFollow(
            Transform follow,
            float s,
            uint d,
            float lifeTime
        )
        {
            target = follow;
            speed = s;
            damage = d;
            Invoke(nameof(TryDestroy), lifeTime);
        }

        public void Execute(
            Vector3 dir,
            float s,
            uint d,
            float lifeTime
        )
        {
            target = null;
            direction = dir;
            speed = s;
            damage = d;
            Invoke(nameof(TryDestroy), lifeTime);
        }

        private void TryDestroy()
        {
            OnDestroyEvent?.Invoke();
            Destroy(gameObject);
        }

        public bool IsSwapped() => swapped;
    }
}