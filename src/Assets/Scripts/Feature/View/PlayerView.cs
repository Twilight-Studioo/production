#region

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
        private Coroutine damageCoroutine;
        private bool isGrounded; // 地面に接触しているかどうかのフラグ
        private Rigidbody rb;
        
        [SerializeField] 
        private GameObject slashingEffect;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
        }

        private void Update()
        {

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
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
            }

            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = false;
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
            if (isGrounded)
            {
                Vector3 movement = transform.right * (direction * Time.deltaTime);
                rb.MovePosition(rb.position + movement);
        
            }
            else
            {
                Vector3 movement = transform.right * (direction * Time.deltaTime)/jumpMove;
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

        public void Attack(float degree)
        {
            Instantiate(slashingEffect, this.transform.position, Quaternion.Euler(0,0,degree),this.transform);
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