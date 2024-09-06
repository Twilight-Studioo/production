#region

using System;
using Feature.Component;
using Feature.Interface;
using UniRx;
using UnityEngine;

#endregion

namespace Feature.View
{
    [RequireComponent(typeof(VFXView))]
    public class PlayerView : MonoBehaviour, IDamaged
    {

        public bool isDrawSwapRange;

        [SerializeField] private GameObject slashingEffect;
        [SerializeField] private GameObject dagger;
        public bool Right = true;
        public float hx;
        public float vy;
        public float comboTimeWindow = 1f; // 〇秒以内の連続攻撃を許可
        public float comboAngleOffset = 60f; // 連続攻撃時の角度変化
        public int maxComboCount = 3; // 連続攻撃の最大回数
        private readonly IReactiveProperty<bool> isGrounded = new ReactiveProperty<bool>(false); // 地面に接触しているかどうかのフラグ

        public readonly IReactiveProperty<Vector3> Position = new ReactiveProperty<Vector3>();

        private AnimationWrapper animator;
        private int comboCount;

        private float lastAttackTime;
        private float lastDegree;
        private Vector3 previousPosition;
        private Rigidbody rb;
        private float speed;
        [NonSerialized] public float SwapRange;

        private VFXView vfxView;
        private float yDegree; //y座標の回転

        private void Awake()
        {
            rb = GetComponentInChildren<Rigidbody>();
            animator = new (GetComponentInChildren<Animator>());
            vfxView = GetComponent<VFXView>();
            isGrounded
                .Where(x => !x)
                .Subscribe(animator.SetIsFalling);
        }

        private void Update()
        {
            Position.Value = transform.position;
        }

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
            if (SwapRange != 0 && isDrawSwapRange) DrawWireDisk(transform.position, SwapRange, Color.magenta);
        }

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            var imp = (transform.position - attacker.position).normalized;
            imp.y += 1f;
            rb.AddForce(imp * 5f, ForceMode.Impulse);
            OnDamageEvent?.Invoke(damage);
            animator.OnTakeDamage();
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

            if (degree == 0 && Right == false) degree = -180f;
            var obj = Instantiate(slashingEffect, transform.position + new Vector3(0f, 1f, 0),
                Quaternion.Euler(yDegree, 0, degree));
            var slash = obj.GetComponent<Slash>();
            slash.SetDamage(damage);
            Destroy(obj, 0.5f);
            animator.OnAttack();

            // 最後の攻撃情報を更新
            lastAttackTime = currentTime;
            lastDegree = degree;
        }

        public void PlayVFX()
        {
            vfxView.PlayVFX();
        }

        public bool IsGrounded()
        {
            return isGrounded.Value;
        }
    }
}