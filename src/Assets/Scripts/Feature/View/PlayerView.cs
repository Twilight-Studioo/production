﻿#region

using System;
using System.Collections;
using System.Collections.Generic;
using Core.Utilities;
using Feature.Component;
using Feature.Interface;
using UniRx;
using UnityEngine;
using UnityEngine.VFX;

#endregion

namespace Feature.View
{
    public class PlayerView : MonoBehaviour, IDamaged, IPlayerView
    {
        [SerializeField] private List<GameObject> normalSlash;
        [SerializeField] private List<GameObject> slashingEffect;
        [SerializeField] private GameObject dagger;
        [SerializeField] private GameObject katana;
        [SerializeField] private GameObject sheath;
        [SerializeField] private GameObject swappingEffect;
        [SerializeField] private VisualEffect visualEffect;
        public Transform daggerSpawn;

        [SerializeField, Tooltip("刀の初期回転角度（一段目の攻撃時）"),]
        private float yDegree = 105f;

        [SerializeField, Tooltip("刀の二段目攻撃時の回転角度"),]
        private float newYDegree = 284f;

        private readonly IReactiveProperty<bool> isGrounded = new ReactiveProperty<bool>(false); // 地面に接触しているかどうかのフラグ

        private readonly IReactiveProperty<Vector3> position = new ReactiveProperty<Vector3>();
        private readonly IReactiveProperty<float> speed = new ReactiveProperty<float>(0f);

        private AnimationWrapper animator;
        private float attackComboCount;
        private float attackCoolTime;
        private float comboAngleOffset; // 連続攻撃時の角度変化
        private int comboCount = -1;
        private float comboTimeWindow; // 〇秒以内の連続攻撃を許可
        private bool isGravityDisabled;
        private bool isMovementDisabled;

        private float lastAttackTime;
        private float lastDegree;
        private float maxComboCoolTime;
        private float maxComboCount; // 連続攻撃の最大回数
        private float monochrome;
        private Vector3 previousPosition;
        private Rigidbody rb;
        private bool right = true;

        private float vignetteChange; //赤くなるまでの時間

        private void Awake()
        {
            rb = GetComponentInChildren<Rigidbody>();
            animator = this.Create(GetComponentInChildren<Animator>());
            GetComponent<VFXView>();
            isGrounded
                .Subscribe(x => { animator.SetIsFalling(!x); });
            Speed = speed.ToReadOnlyReactiveProperty();
        }

        private void Update()
        {
            var pos = transform.position;
            if(Mathf.Abs(pos.z) > 0.3f)
            {
                pos.z = 0f;
                transform.position = pos;
            }
            
            position.Value = transform.position;
        }

        private void FixedUpdate()
        {
            var pos = rb.position;
            pos.y = 0;
            speed.Value = (pos - previousPosition).magnitude / Time.fixedDeltaTime;
            previousPosition = pos;
            animator.SetSpeed(speed.Value);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground") && 1f > transform.GetGroundDistance(10f))
            {
                isGrounded.Value = true;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
            }
        }

        private void OnDrawGizmos()
        {
            if (SwapRange != 0 && IsDrawSwapRange)
            {
                DrawWireDisk(transform.position, SwapRange, Color.magenta);
            }
        }

        public DamageResult OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            var imp = (transform.position - hitPoint).normalized;
            imp.y += 1f;
            this.UniqueStartCoroutine(Knockback(imp, 5f, 0.4f));
            // rb.AddForce(imp * 5f, ForceMode.Impulse);
            var result = OnDamageEvent?.Invoke(damage, attacker.position);
            animator.OnTakeDamage();
            return result;
        }

        public event Action<DamageResult> OnHitHandler;
        public IReadOnlyReactiveProperty<float> Speed { get; set; }

        public float SwapRange { get; set; }

        public bool IsDrawSwapRange { get; set; }

        public IReadOnlyReactiveProperty<Vector3> GetPositionRef() => position;

        public GameObject GetGameObject() => gameObject;

        public void SetParam(float comboTimeWindow, float comboAngleOffset, float maxComboCount, float attackCoolTime,
            float maxComboCoolTime)
        {
            this.comboTimeWindow = comboTimeWindow;
            this.comboAngleOffset = comboAngleOffset;
            this.maxComboCount = maxComboCount;
            this.attackCoolTime = attackCoolTime;
            this.maxComboCoolTime = maxComboCoolTime;
        }

        public void SetPosition(Vector3 p)
        {
            // スワップ前の位置を取得
            var previousPosition = transform.position;

            transform.position = p;

            if (swappingEffect != null)
            {
                // 生成
                var instantiateEffect = ObjectFactory.Instance.CreateObject(
                    swappingEffect,
                    transform.position,
                    Quaternion.identity
                );

                // VisualEffect コンポーネントを取得
                var visualEffect = instantiateEffect.GetComponent<VisualEffect>();

                //Target_position更新
                if (visualEffect != null)
                {
                    visualEffect.SetVector3("Target_position_position", new(previousPosition.x, previousPosition.y, 0));
                }
            }
        }

        public Transform GetTransform() => transform;

        public event DamageHandler<uint, Vector3> OnDamageEvent;

        public void Move(Vector3 direction, float power)
        {
            if (isMovementDisabled)
            {
                return;
            }

            //向き
            if (direction.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                right = true;
            }
            else if (direction.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                right = false;
            }

            if (isGrounded.Value)
            {
                var movement = direction * (Time.deltaTime * power);
                rb.MovePosition(rb.position + movement);
            }
            else
            {
                var movement = direction * (0.5f * (Time.deltaTime * power));
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
            if (degree == 0 && right == false)
            {
                instantiateDagger =
                    ObjectFactory.Instance.CreateObject(dagger, daggerSpawn.position, Quaternion.Euler(0, 0, -180));
            }
            else
            {
                instantiateDagger =
                    ObjectFactory.Instance.CreateObject(dagger, daggerSpawn.position, Quaternion.Euler(0, 0, degree));
            }

            if (h == 0 && v == 0)
            {
                h = right ? 1 : -1;
            }

            var component = instantiateDagger.GetComponentInChildren<Dagger>();
            component.HorizontalVertical(h, v);
            animator.OnDagger();
        }

        private IEnumerator Knockback(Vector3 direction, float strength, float duration)
        {
            isMovementDisabled = true;
            yield return transform.Knockback(direction, strength, duration);
            yield return new WaitForSeconds(0.9f);
            isMovementDisabled = false;
        }

        private static void DrawWireDisk(Vector3 position, float radius, Color color)
        {
            var oldColor = Gizmos.color;
            Gizmos.color = color;
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new(1, 1, 1));
            Gizmos.DrawWireSphere(Vector3.zero, radius);
            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;
        } // ReSharper disable Unity.PerformanceAnalysis

        public bool CanAttack()
        {
            var currentTime = Time.time;
            return currentTime - lastAttackTime >= (comboCount == 2 ? maxComboCoolTime : attackCoolTime);
        }

        public void Attack(float degree, uint damage, bool voltage)
        {
            var currentTime = Time.time;
            if (currentTime - lastAttackTime < attackCoolTime)
            {
                return;
            }

            if (currentTime - lastAttackTime <= comboTimeWindow)
            {
                if (comboCount >= maxComboCount)
                {
                    comboCount = -1;
                    yDegree = 0;
                }

                comboCount++;
            }
            else
            {
                comboCount = 0;
            }

            var effectIndex = Mathf.Clamp(comboCount, 0, slashingEffect.Count - 1);

            // 最後の攻撃情報を更新
            lastAttackTime = currentTime;

            if (degree == 0 && right == false)
            {
                degree = -180f;
            }

            switch (comboCount)
            {
                case 0:
                    katana.gameObject.transform.Rotate(0, newYDegree * Time.deltaTime, 0);
                    break;
                case 1:
                    katana.gameObject.transform.Rotate(0, yDegree * Time.deltaTime, 0);
                    break;
            }

            var effectPrefab = voltage ? slashingEffect[effectIndex] : normalSlash[effectIndex];

            var obj = Instantiate(effectPrefab, transform.position + new Vector3(0f, 1f, 0),
                Quaternion.Euler(effectPrefab.transform.localEulerAngles.x, effectPrefab.transform.localEulerAngles.y,
                    degree));

            var slash = obj.GetComponent<Slash>();
            slash.OnHitEvent += OnHitHandler;
            slash.SetDamage(damage);

            Destroy(obj, 0.5f);
            animator.SetAttackComboCount(comboCount);
            animator.OnAttack(0);


            // 攻撃方向に少し飛ばす
            // degreeをラジアンに変換
            var radian = degree * Mathf.Deg2Rad;

            // 力の方向を計算
            var forceDirection = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian));
            const float force = 6f;
            const float snapStopTime = 0.1f;
            const float gravityDisableTime = 0.06f;
            rb.AddForce(forceDirection.normalized * force, ForceMode.Impulse);
            this.UniqueStartCoroutine(this.DelayMethod(snapStopTime, () => rb.velocity = Vector3.zero),
                "AttackedSnap");
            // if (snapCanceledToken != null) StopCoroutine(snapCanceledToken);
            //snapCanceledToken = StartCoroutine(this.DelayMethod(snapStopTime, () => rb.velocity = Vector3.zero));
            if (!isGravityDisabled)
            {
                StartCoroutine(DisableGravityTemporarily(gravityDisableTime));
            }
        }

        private IEnumerator DisableGravityTemporarily(float duration)
        {
            isGravityDisabled = true;
            rb.useGravity = false; // 重力を無効化
            yield return new WaitForSeconds(duration); // 指定時間待機
            rb.useGravity = true; // 重力を再有効化
            isGravityDisabled = false;
        }

        public void AddForce(Vector3 force)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(force, ForceMode.VelocityChange);
        }

        public void VoltageEffect(int voltageValue, int useVoltageAttackValue, int votageTwoAttackValue, int maxVoltage)
        {
            if (voltageValue >= maxVoltage)
            {
                visualEffect.SetUInt("Voltage", 3);
            }
            else if (voltageValue >= votageTwoAttackValue)
            {
                visualEffect.SetUInt("Voltage", 2);
            }
            else if (voltageValue >= useVoltageAttackValue)
            {
                visualEffect.SetUInt("Voltage", 1);
            }
            else
            {
                visualEffect.SetUInt("Voltage", 0);
            }
        }

        public bool IsGrounded() => isGrounded.Value;

        public Vector3 GetForward() => right ? Vector3.right : Vector3.left;
    }
}