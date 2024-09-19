using System;
using Feature.Interface;
using UniRx;
using UnityEngine;

namespace Feature.Component
{
    public class DamagedTrigger: MonoBehaviour, ISwappable
    {
        private uint damage = 0;
        
        private Vector3 direction = Vector3.zero;
        
        private float speed = 1.0f;
        
        private bool canHitEnemy = false;
        private bool canHitPlayer = false;
        
        private readonly IReactiveProperty<Vector2> position = new ReactiveProperty<Vector2>();
        
        public event Action OnHitEvent;
        
        public event Action OnDestroyEvent;

        private void Update()
        {
            position.Value = transform.position;
        }

        public void SetHitObject(bool hitEnemy, bool hitPlayer)
        {
            canHitEnemy = hitEnemy;
            canHitPlayer = hitPlayer;
        }
        
        public void Execute(
            Vector3 dir,
            float s,
            uint d,
            float lifeTime
        )
        {
            direction = dir;
            speed = s;
            damage = d;
            Invoke(nameof(TryDestroy), lifeTime);
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
                OnHitEvent?.Invoke();
                TryDestroy();
            }
        }
        
        private void TryDestroy()
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
        }
    }
}