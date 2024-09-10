﻿#region

using System;
using System.Collections.Generic;
using Feature.Component;
using Feature.Interface;
using UniRx;
using UnityEngine;

#endregion

namespace Feature.View
{
    public class PlayerView : MonoBehaviour, IDamaged, IPlayerView
    {
        public bool isDrawSwapRange;

        [SerializeField] private List<GameObject> slashingEffect;
        [SerializeField] private GameObject dagger;
        public bool Right = true;
        public float hx;
        public float vy;

        private readonly IReactiveProperty<bool> isGrounded = new ReactiveProperty<bool>(false); // 地面に接触しているかどうかのフラグ

        private readonly IReactiveProperty<Vector3> position = new ReactiveProperty<Vector3>();

        private AnimationWrapper animator;
        private float comboAngleOffset; // 連続攻撃時の角度変化
        private int comboCount;
        private float comboTimeWindow; // 〇秒以内の連続攻撃を許可

        private float lastAttackTime;
        private float lastDegree;
        private int maxComboCount; // 連続攻撃の最大回数
        private float monochrome;
        private Vector3 previousPosition;
        private Rigidbody rb;
        private float speed;
        [NonSerialized] public float SwapRange;
        private URP urp;

        private VFXView vfxView;

        private float vignetteChange; //赤くなるまでの時間
        private float yDegree; //y座標の回転

        public float SwapRange { get; set; }
        
        public bool IsDrawSwapRange { get; set; }

        private void Awake()
        {
            rb = GetComponentInChildren<Rigidbody>();
            animator = this.Create(GetComponentInChildren<Animator>());
            vfxView = GetComponent<VFXView>();
            isGrounded
                .Subscribe(x => {
                    animator.SetIsFalling(!x);        
                });
        }

        private void Update()
        {
            position.Value = transform.position;
        }
        
        public IReadOnlyReactiveProperty<Vector3> GetPositionRef() => position;
        
        public GameObject GetGameObject() => gameObject;

        private void FixedUpdate()
        {
            speed = (rb.position - previousPosition).magnitude / Time.deltaTime;
            previousPosition = rb.position;
            animator.SetSpeed(speed);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground")) isGrounded.Value = true;
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
            }
        }

        private void OnDrawGizmos()
        {
            if (SwapRange != 0 && IsDrawSwapRange) DrawWireDisk(transform.position, SwapRange, Color.magenta);
        }
        
        public void SetPosition(Vector3 p)
        {
            transform.position = p;
        }
        
        public Transform GetTransform() => transform;

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            var imp = (transform.position - attacker.position).normalized;
            imp.y += 1f;
            rb.AddForce(imp * 5f, ForceMode.Impulse);
            OnDamageEvent?.Invoke(damage);
            animator.OnTakeDamage();
        }

        public void SetParam(float ComboTimeWindow, float ComboAngleOffset, int MaxComboCount, float VignetteChange,
            URP _urp, float Monochrome)
        {
            comboTimeWindow = ComboTimeWindow;
            comboAngleOffset = ComboAngleOffset;
            maxComboCount = MaxComboCount;
            vignetteChange = VignetteChange;
            urp = _urp;
            monochrome = Monochrome;
        }

        public event Action<uint> OnDamageEvent;

        private static void DrawWireDisk(Vector3 position, float radius, Color color)
        {
            var oldColor = Gizmos.color;
            Gizmos.color = color;
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, 1, 1));
            Gizmos.DrawWireSphere(Vector3.zero, radius);
            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;
        }

        public void Move(float direction, float jumpMove)
        {
            //向き
            if (direction > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                Right = true;
            }
            else if (direction < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                direction = direction * -1;
                Right = false;
            }

            if (isGrounded.Value)
            {
                var movement = transform.right * (direction * Time.deltaTime);
                rb.MovePosition(rb.position + movement);
            }
            else
            {
                var movement = transform.right * (direction * Time.deltaTime) / jumpMove;
                rb.MovePosition(rb.position + movement);
            }
        }

        public void Jump(float jumpForce)
        {
            if (isGrounded.Value)
            {
                animator.OnJump();
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded.Value = false; // ジャンプ中になるので接地フラグをfalseにする
            }
        }

        public void Dagger(float degree, float h, float v)
        {
            GameObject instantiateDagger;
            if (degree == 0 && Right == false)
                instantiateDagger = Instantiate(this.dagger, transform.position, Quaternion.Euler(0, 0, -180));
            else
                instantiateDagger = Instantiate(this.dagger, transform.position, Quaternion.Euler(0, 0, degree));

            if (h == 0 && v == 0)
            {
                if (Right)
                    h = 1;
                else
                    h = -1;
            }

            var dagger = instantiateDagger.GetComponentInChildren<Dagger>();
            dagger.HorizontalVertical(h, v);
            animator.OnDagger();
        }

        public void Attack(float degree, uint damage)
        {
            var currentTime = Time.time;
            if (currentTime - lastAttackTime <= comboTimeWindow)
            {
                if (comboCount < maxComboCount)
                {
                    comboCount++;
                    yDegree += lastDegree + comboAngleOffset;
                }
                else
                {
                    // 最大連続攻撃回数に達した場合、リセット
                    comboCount = 0;
                    yDegree = 0; // 角度をリセット（必要に応じて初期角度に変更）
                }
            }
            else
            {
                yDegree = 0;
            }

            animator.SetAttackComboCount(comboCount);

            var effectIndex = Mathf.Clamp(comboCount, 0, slashingEffect.Count - 1);

            if (degree == 0 && Right == false) degree = -180f;
            var obj = Instantiate(slashingEffect[effectIndex], transform.position + new Vector3(0f, 1f, 0),
                Quaternion.Euler(yDegree, 0, degree));

            var slash = obj.GetComponent<Slash>();
            slash.SetDamage(damage);
            Destroy(obj, 0.5f);
            animator.OnAttack(0.5f);

            // 最後の攻撃情報を更新
            lastAttackTime = currentTime;
            lastDegree = degree;
        }

        public bool IsGrounded()
        {
            return isGrounded.Value;
        }

        public void SwapTimeStartURP()
        {
            urp.SwapStartURP(vignetteChange, monochrome);
        }

        public void SwapTimeFinishURP()
        {
            urp.SwapFinishURP();
        }
    }
}