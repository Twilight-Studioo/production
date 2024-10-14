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
        private Collider capsuleCollider;
        private uint damage;

        private float h;
        
        private float lifeTime;
        private Rigidbody rb;
        private float speed;
        private float v;
        private readonly IReactiveProperty<Vector2> position = new ReactiveProperty<Vector2>();

        private void Start()
        {
            PramSet();

            rb = GetComponent<Rigidbody>();
            this.UniqueStartCoroutine(this.DelayMethod(lifeTime, Delete), "onLifetime delete dagger");
            //transform.parent.parent = null;
            capsuleCollider = gameObject.GetComponent<Collider>();
        }

        private void Update()
        {
            position.Value = transform.position;
        }

        private void FixedUpdate()
        {
            var force = new Vector3(h, v, 0);
            rb.AddForce(force * speed, ForceMode.Force);
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // 敵に当たった場合の処理
                // 敵にダメージを与える
                // ナイフがはじかれる

                var enemy = collision.gameObject.GetComponent<IEnemy>()
                            ?? collision.gameObject.GetComponentInParent<IEnemy>();
                enemy?.OnDamage(damage, collision.transform.position, transform);
                capsuleCollider.isTrigger = false;
                if (rb != null)
                {
                    rb.isKinematic = true; // ナイフが動かないようにする
                    h = h * -0.5f;
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
                // 壁に当たった場合の処理
                // ナイフが壁に刺さる
                var rb = GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true; // ナイフが動かないようにする
            }
            else if (collision.gameObject.CompareTag("Ground"))
            {
                // 壁に当たった場合の処理
                // ナイフが壁に刺さる
                var rb = GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true; // ナイフが動かないようにする
            }
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

        //クナイ回転処理
        private void RotateToTarget(float angle, float duration)
        {
            // 現在の角度を取得
            var startAngle = transform.eulerAngles.z;

            // 回転の進行
            Observable.EveryUpdate()
                .Select(_ => Time.deltaTime / duration)
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(duration)))
                .Scan((progress, delta) => progress + delta)
                .Subscribe(progress =>
                {
                    // 進行に基づいて新しい角度を計算
                    var currentAngle = Mathf.LerpAngle(startAngle, angle, progress);
                    transform.eulerAngles = new Vector3(0, 0, currentAngle);
                });
        }

        public void OnSelected()
        {
        }

        public void OnDeselected()
        {
        }

        public IReadOnlyReactiveProperty<Vector2> GetPositionRef() => position.ToReadOnlyReactiveProperty();

        public Vector2 GetPosition() => position.Value;

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
    }
}