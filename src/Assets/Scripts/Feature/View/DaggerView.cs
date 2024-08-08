﻿#region

using System;
using UnityEngine;
using VContainer;
using Quaternion = System.Numerics.Quaternion;
using UniRx;

#endregion

namespace Feature.View
{
    public class DaggerView : MonoBehaviour
    {
        //後にScriptable化するヨ
        [SerializeField]private float lifeTime = 3f;
        private Rigidbody rb;
        [SerializeField]private float speed = 20;
        [SerializeField] private float damage = 1;
        private float h;
        private float v;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            Destroy(this.gameObject,lifeTime);
            Destroy(transform.parent.gameObject,lifeTime);
            transform.parent.parent = null;

        }

        public void HorizontalVertical(float hx,float vy)
        {
            h = hx;
            v = vy;
        }
        
        void FixedUpdate()
        { 
            Vector3 force = new Vector3(h, v, 0);
             rb.AddForce(force*speed, ForceMode.Force);
        }
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // 敵に当たった場合の処理
                // 敵にダメージを与える
                // ナイフがはじかれる
                
                //ダメージ体力出来次第実装
                //collision.gameObject.GetComponent<Enemy>(damage);
                Destroy(collision.gameObject);
                if (rb != null)
                { 
                    rb.isKinematic = true; // ナイフが動かないようにする
                    h = h * (-0.5f);
                    v = 0.5f;
                    if (h <= 0)
                    {
                        RotateToTarget(200, 0.2f);
                    }
                    else
                        RotateToTarget(-200, 0.2f);
                    rb.isKinematic = false;
                }
                Destroy(gameObject,0.5f); // ナイフを破壊
            }
            else if (collision.gameObject.CompareTag("Wall"))
            {
                // 壁に当たった場合の処理
                // ナイフが壁に刺さる
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true; // ナイフが動かないようにする
                }
            }
            else if (collision.gameObject.CompareTag("Ground"))
            {
                // 壁に当たった場合の処理
                // ナイフが壁に刺さる
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true; // ナイフが動かないようにする
                }
            }
            
        }

        //クナイ回転処理
        void RotateToTarget(float angle, float duration)
        {
            // 現在の角度を取得
            float startAngle = transform.eulerAngles.z;

            // 回転の進行
            Observable.EveryUpdate()
                .Select(_ => Time.deltaTime / duration)
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(duration)))
                .Scan((progress, delta) => progress + delta)
                .Subscribe(progress =>
                {
                    // 進行に基づいて新しい角度を計算
                    float currentAngle = Mathf.LerpAngle(startAngle, angle, progress);
                    transform.eulerAngles = new Vector3(0, 0, currentAngle);
                });
        }
    }
}