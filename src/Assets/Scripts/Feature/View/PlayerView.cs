#region

using Feature.Common;
using System.Collections;
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
        private bool isGrounded;
        private Rigidbody rb;
        private GameObject sword;
        private Coroutine damageCoroutine;

        public CharacterParams characterParams;
        public EnemyParams enemyParams;

        public IReactiveProperty<int> Health { get; } = new ReactiveProperty<int>();

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            sword = transform.Find("Sword").gameObject;
            animator = sword.GetComponent<Animator>();
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
            Position.Value = transform.position;

            if (!animator.isActiveAndEnabled)
            {
                return;
            }

            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("SwordUp") && stateInfo.normalizedTime >= 1.0f)
            {
                StopAnimation();
            }

            if (stateInfo.IsName("SwordUpR") && stateInfo.normalizedTime >= 1.0f)
            {
                StopAnimation();
            }

            if (stateInfo.IsName("SwordRight") && stateInfo.normalizedTime >= 1.0f)
            {
                StopAnimation();
            }

            if (stateInfo.IsName("SwordDownR") && stateInfo.normalizedTime >= 1.0f)
            {
                StopAnimation();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
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
                isGrounded = false;
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
            if (isGrounded)
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
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
        }

        public void Attack(Vector2 direction)
        {
            sword.SetActive(true);

            if (direction == Vector2.zero)
            {
                direction = Vector2.right;
            }

            if ((direction.y >= 0.2f && direction.x >= 0.2f) || (direction.y >= 0.2f && direction.x <= -0.2f))
            {
                animator.SetBool("UpRight", true);
            }
            else if (direction.y >= 0.2f)
            {
                animator.SetBool("Up", true);
            }
            else if (direction.y <= -0.2f)
            {
                animator.SetBool("DownRight", true);
            }
            else if (direction.x >= 0.5f || direction.x <= -0.5f)
            {
                animator.SetBool("Right", true);
            }
        }

        private void StopAnimation()
        {
            animator.SetBool("Up", false);
            animator.SetBool("UpRight", false);
            animator.SetBool("Right", false);
            animator.SetBool("DownRight", false);
            sword.SetActive(false);
        }

        public bool IsGrounded() => isGrounded;

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