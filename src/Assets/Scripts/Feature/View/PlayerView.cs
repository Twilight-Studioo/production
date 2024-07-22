#region

using System;
using UniRx;
using UnityEngine;

#endregion

namespace Feature.View
{
    public class PlayerView : MonoBehaviour
    {
        // ----TODO Draft----

        public bool isDrawSwapRange;

        public float swapRange;
        public readonly IReactiveProperty<Vector3> Position = new ReactiveProperty<Vector3>();
        private Animator animator;
        private IReactiveProperty<bool> isGrounded = new ReactiveProperty<bool>(false); // 地面に接触しているかどうかのフラグ
        private Rigidbody rb;
        private Vector3 previousPosition;
        private float speed;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int OnJump = Animator.StringToHash("OnJump");
        private static readonly int IsFalling = Animator.StringToHash("IsFalling");


        private void Awake()
        {
            rb = GetComponentInChildren<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
            isGrounded
                .Subscribe(x =>
                {
                    animator.SetBool(IsFalling, !x);
                });
        }

        private void Update()
        {
            Position.Value = transform.position;

            // TODO: Updateは辞めて、delegateで受け取る
            if (animator == null || !animator.isActiveAndEnabled)
            {
                return;
            }
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

        private void OnDrawGizmos()
        {
            if (swapRange != 0 && isDrawSwapRange)
            {
                DrawWireDisk(transform.position, swapRange, Color.magenta);
            }
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
        }

        // ----TODO Draft----

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

        public void Attack(Vector2 direction)
        {
            // 攻撃方向に応じたアニメーションを再生
            if (direction == Vector2.zero)
            {
                direction = Vector2.right;
            }
        }

        private void StopAnimation()
        {
            
        }


        public bool IsGrounded() => isGrounded.Value;
    }
}