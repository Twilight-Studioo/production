#region

using UniRx;
using UnityEngine;

#endregion

namespace Feature.View
{
    public class PlayerView : MonoBehaviour
    {
        public readonly IReactiveProperty<Vector3> Position = new ReactiveProperty<Vector3>();
        private bool isGrounded; // 地面に接触しているかどうかのフラグ
        private Rigidbody rb;
        private GameObject sword;
        private Animator animator;
        

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            sword = transform.Find("Sword").gameObject;
            animator = sword.GetComponent<Animator>();
        }

        private void Update()
        {
            Position.Value = transform.position;

            // TODO: Updateは辞めて、delegateで受け取る
            if (!animator.isActiveAndEnabled)
            {
                return;
            }
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // 現在のアニメーションが指定したアニメーションであり、かつそのアニメーションが終了したかどうかを確認
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

        // ----TODO Draft----
        
        public bool isDrawSwapRange;

        public float swapRange;

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
                isGrounded = false; // ジャンプ中になるので接地フラグをfalseにする
            }
        }
        
        public void Attack(Vector2 direction)
        {
            sword.SetActive(true);
            // 攻撃方向に応じたアニメーションを再生
            if (direction == Vector2.zero)
            {
                direction = Vector2.right;
            }
            
            if (direction.y>=0.2f&&direction.x>=0.2f||direction.y>=0.2f&&direction.x<=-0.2f)
            {
                animator.SetBool("UpRight",true);
                
            }
            else if (direction.y>=0.2f)
            {
                animator.SetBool("Up",true);
            }
            else if (direction.y<=-0.2f) 
            {
                animator.SetBool("DownRight",true);
            }
            else if (direction.x>=0.5f||direction.x<=-0.5f)
            {
                animator.SetBool("Right",true);
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
    }
}