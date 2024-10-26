#region

using System;
using Core.Utilities;
using Feature.Common.Parameter;
using Feature.Interface;
using UniRx;
using UnityEngine;

#endregion

namespace Feature.Component
{
    public class Dagger : MonoBehaviour, ISwappable
    {
        public DaggerPrams daggerPrams;
        private readonly IReactiveProperty<Vector2> position = new ReactiveProperty<Vector2>();
        private Collider capsuleCollider;
        private uint damage;

        private float h;

        private float lifeTime;
        private Rigidbody rb;
        private float speed;
        private float v;

        private void Start()
        {
            PramSet();

            rb = GetComponent<Rigidbody>();
            this.UniqueStartCoroutine(this.DelayMethod(lifeTime, Delete), "onLifetime delete dagger");
            capsuleCollider = gameObject.GetComponent<Collider>();

            var initialVelocity = new Vector3(h, v, 0).normalized * speed;
            rb.velocity = initialVelocity;
        }

        private void Update()
        {
            position.Value = transform.position;
        }

        private void FixedUpdate()
        {
            var currentVelocity = rb.velocity;
            var constantVelocity = currentVelocity.normalized * speed;
            rb.velocity = constantVelocity;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                var enemy = collision.gameObject.GetComponent<IEnemy>()
                            ?? collision.gameObject.GetComponentInParent<IEnemy>();
                enemy?.OnDamage(damage, collision.transform.position, transform);
                capsuleCollider.isTrigger = false;
                if (rb != null)
                {
                    rb.isKinematic = true; // ナイフが動かないようにする
                    h *= -0.5f;
                    v = 0.5f;
                    if (h <= 0)
                        RotateToTarget(200, 0.2f);
                    else
                        RotateToTarget(-200, 0.2f);

                    rb.isKinematic = false;
                }

                this.UniqueStartCoroutine(this.DelayMethod(0.5f, Delete), "onAttacked delete dagger");
            }
            else if (collision.gameObject.CompareTag("Wall"))
            {
                var component = GetComponent<Rigidbody>();
                if (component != null) component.isKinematic = true;
                position.Value = transform.position;
            }
            else if (collision.gameObject.CompareTag("Ground"))
            {
                var component = GetComponent<Rigidbody>();
                if (component != null) component.isKinematic = true;
                position.Value = transform.position;
            }
        }

        public void OnSelected()
        {
        }

        public void OnDeselected()
        {
        }

        public IReadOnlyReactiveProperty<Vector2> GetPositionRef()
        {
            return position.ToReadOnlyReactiveProperty();
        }

        public Vector2 GetPosition()
        {
            return position.Value;
        }

        public void OnSwap(Vector2 p)
        {
            transform.position = p;
        }

        public event Action OnDestroyEvent;

        public void OnInSelectRange()
        {
        }

        public void OnOutSelectRange()
        {
        }

        public void Delete()
        {
            OnDestroyEvent?.Invoke();
            Destroy(gameObject);
        }

        private void PramSet()
        {
            lifeTime = daggerPrams.lifeTime;
            speed = daggerPrams.speed;
            damage = daggerPrams.damage;
        }

        public void HorizontalVertical(float hx, float vy)
        {
            h = hx;
            v = vy;
        }

        private void RotateToTarget(float angle, float duration)
        {
            var startAngle = transform.eulerAngles.z;

            Observable.EveryUpdate()
                .Select(_ => Time.deltaTime / duration)
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(duration)))
                .Scan((progress, delta) => progress + delta)
                .Subscribe(progress =>
                {
                    // 進行に基づいて新しい角度を計算
                    var currentAngle = Mathf.LerpAngle(startAngle, angle, progress);
                    var angles = transform.eulerAngles;
                    angles.z = currentAngle;
                    transform.eulerAngles = angles;
                });
        }
    }
}