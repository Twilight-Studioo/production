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
        
        public bool isDrawSwapRange;

        [NonSerialized]
        public float SwapRange;
        public readonly IReactiveProperty<Vector3> Position = new ReactiveProperty<Vector3>();
        private bool isGrounded; // 地面に接触しているかどうかのフラグ
        private Rigidbody rb;
        private VFXView vfxView;
        
        [SerializeField] 
        private GameObject slashingEffect;
        
        public event Action<uint> OnDamageEvent;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            vfxView = GetComponent<VFXView>();
        }

        private void Update()
        {
            Position.Value = transform.position;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
                Debug.Log("Grounded");
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

        public void Attack(float degree, uint damage)
        {
            var obj = Instantiate(slashingEffect, this.transform.position, Quaternion.Euler(0,0,degree),this.transform);
            var slash = obj.GetComponent<SlashView>();
            slash.SetDamage(damage);
            Destroy(obj, 0.5f);
        }
        
        public void PlayVFX()
        {
            vfxView.PlayVFX();
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

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            var imp = (transform.position - attacker.position).normalized;
            imp.y += 1f;
            rb.AddForce(imp * 5f, ForceMode.Impulse);
            OnDamageEvent?.Invoke(damage);
        }
    }
}