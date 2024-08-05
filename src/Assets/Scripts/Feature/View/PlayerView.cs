#region

using System;
using Feature.Common;
using System.Collections;
using Core.Input.Generated;
using Feature.Presenter;
using UniRx;
using UnityEngine;

#endregion

namespace Feature.View
{
    public class PlayerView : MonoBehaviour
    {
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
        
        private Coroutine damageCoroutine;

        public CharacterParams characterParams;
        public EnemyParams enemyParams;

        public IReactiveProperty<int> Health { get; } = new ReactiveProperty<int>();
        [SerializeField] private GameObject slashingEffect;

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

        private void Start()
        {
            Health.Value = characterParams.health;

            Health
                .Where(hp => hp <= 0)
                .Subscribe(_ => OnPlayerDeath())
                .AddTo(this);
        }

        private void Update()
        {
            
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

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (damageCoroutine == null)
                {
                    damageCoroutine = StartCoroutine(TakeDamageOverTime());
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (damageCoroutine != null)
                {
                    StopCoroutine(damageCoroutine);
                    damageCoroutine = null;
                }
            }

            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded.Value = false;
            }
        }

        private IEnumerator TakeDamageOverTime()
        {
            while (true)
            {
                Health.Value = Mathf.Max(Health.Value - enemyParams.damage, 0);
                yield return new WaitForSeconds(characterParams.takeDamageOverTime);
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
            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, 1, 1));
            Gizmos.DrawWireSphere(Vector3.zero, radius);
            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;
        }

        public void Move(float direction, float jumpMove)
        {
            if (isGrounded.Value)
            {
                Vector3 movement = transform.right * (direction * Time.deltaTime);
                rb.MovePosition(rb.position + movement);
        
            }
            else
            {
                Vector3 movement = transform.right * (direction * Time.deltaTime)/jumpMove;
                rb.MovePosition(rb.position + movement);
            }
            //向き
            if (direction > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (direction < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                direction = direction * -1;
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

        public void Attack(float degree)
        {
            Instantiate(slashingEffect, this.transform.position, Quaternion.Euler(0,0,degree),this.transform);
        }

        public bool IsGrounded() => isGrounded.Value;

        private void OnPlayerDeath()
        {
            Debug.Log("Player has died. Stopping game.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}