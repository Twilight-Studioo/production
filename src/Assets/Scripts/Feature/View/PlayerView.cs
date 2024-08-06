#region

using System;
using Feature.Interface;
using UniRx;
using UnityEngine;

#endregion

namespace Feature.View
{
    [RequireComponent(typeof(VFXView))]
    public class PlayerView : MonoBehaviour, IDamaged
    {
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int OnJump = Animator.StringToHash("OnJump");
        private static readonly int IsFalling = Animator.StringToHash("IsFalling");

        public bool isDrawSwapRange;

        [SerializeField] private GameObject slashingEffect;

        public readonly IReactiveProperty<Vector3> Position = new ReactiveProperty<Vector3>();

        private Animator animator;
        private readonly IReactiveProperty<bool> isGrounded = new ReactiveProperty<bool>(false); // 地面に接触しているかどうかのフラグ
        private Vector3 previousPosition;
        private Rigidbody rb;
        private float speed;

        [NonSerialized] public float SwapRange;

        private VFXView vfxView;

        public IReactiveProperty<int> Health { get; } = new ReactiveProperty<int>();

        private void Awake()
        {
            rb = GetComponentInChildren<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
            vfxView = GetComponent<VFXView>();
            isGrounded
                .Subscribe(x => { animator.SetBool(IsFalling, !x); });
        }

        private void Update()
        {
            Position.Value = transform.position;
        }

        private void FixedUpdate()
        {
            speed = (rb.position - previousPosition).magnitude / Time.deltaTime;
            previousPosition = rb.position;
            animator.SetFloat(Speed, speed);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
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
            if (SwapRange != 0 && isDrawSwapRange)
            {
                DrawWireDisk(transform.position, SwapRange, Color.magenta);
            }
        }

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            var imp = (transform.position - attacker.position).normalized;
            imp.y += 1f;
            rb.AddForce(imp * 5f, ForceMode.Impulse);
            OnDamageEvent?.Invoke(damage);
        }

        public event Action<uint> OnDamageEvent;

        private static void DrawWireDisk(Vector3 position, float radius, Color color)
        {
            var oldColor = Gizmos.color;
            Gizmos.color = color;
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new(1, 1, 1));
            Gizmos.DrawWireSphere(Vector3.zero, radius);
            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;
        }

        public void Move(float direction, float jumpMove)
        {
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
            // //向き
            // if (direction > 0)
            // {
            //     transform.rotation = Quaternion.Euler(0, 0, 0);
            // }
            // else if (direction < 0)
            // {
            //     transform.rotation = Quaternion.Euler(0, 180, 0);
            //     direction = direction * -1;
            // }
        }

        public void Jump(float jumpForce)
        {
            if (isGrounded.Value)
            {
                animator.SetTrigger(OnJump);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded.Value = false; // ジャンプ中になるので接地フラグをfalseにする
            }
        }

        public void Attack(float degree, uint damage)
        {
            var obj = Instantiate(slashingEffect, transform.position + new Vector3(0f, 1f, 0f),
                Quaternion.Euler(0, 0, degree));
            var slash = obj.GetComponent<SlashView>();
            slash.SetDamage(damage);
            Destroy(obj, 0.5f);
        }

        public void PlayVFX()
        {
            vfxView.PlayVFX();
        }

        public bool IsGrounded() => isGrounded.Value;
    }
}