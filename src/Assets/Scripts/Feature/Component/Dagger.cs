#region

using System;
using Feature.Common.Parameter;
using Feature.Interface;
using UniRx;
using UnityEngine;

#endregion

namespace Feature.Component
{
    public class Dagger : MonoBehaviour
    {
        public DaggerPrams daggerPrams;
        private CapsuleCollider capsuleCollider;
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
            Destroy(gameObject, lifeTime);
            Destroy(transform.parent.gameObject, lifeTime);
            //transform.parent.parent = null;
            capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
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

                Destroy(gameObject, 0.5f); // ナイフを破壊
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
    }
}