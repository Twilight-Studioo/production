#region

using Core.Input.Generated;
using Feature.Presenter;
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
        private bool isGrounded; // 地面に接触しているかどうかのフラグ
        private Rigidbody rb;
        private float z;
        private float zRight = -20;
        private float zLeft = 200;
        [SerializeField] private GameObject slashingEffect;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
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
            //向き
            if (direction > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                z = zRight;
            }
            else if (direction < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                direction = direction * -1;
                z = zLeft;
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
                isGrounded = false; // ジャンプ中になるので接地フラグをfalseにする
            }
        }

        public void Attack(Vector2 direction)
        {
            // 攻撃方向に応じたアニメーションを再生
            if (direction == Vector2.zero)
            {
                direction = Vector2.right;
            }

            if ((direction.y >= 0.2f && direction.x >= 0.2f) || (direction.y >= 0.2f && direction.x <= -0.2f))
            {
                //斜め上
                if (z == zRight)
                { 
                    Instantiate(slashingEffect, this.transform.position, Quaternion.Euler(0,0,40),this.transform);
                }
                else
                {
                    Instantiate(slashingEffect, this.transform.position, Quaternion.Euler(0,180,40),this.transform);
                }
            }
            else if (direction.y >= 0.2f)
            {
                //上
                Instantiate(slashingEffect, this.transform.position, Quaternion.Euler(0,0,90),this.transform);
            }
            else if ((direction.y <= -0.2f&&direction.x>=0.2f)||(direction.y <= -0.2f&&direction.x>=-0.2f))
            {
                //斜め下
                if (z == zRight)
                { 
                    Instantiate(slashingEffect, this.transform.position, Quaternion.Euler(0,0,-40),this.transform);
                }
                else
                {
                    Instantiate(slashingEffect, this.transform.position, Quaternion.Euler(0,0,90),this.transform);
                }
            }
            else if(direction.y<=-0.2f)
            {                                                                                                    
                //した
                Instantiate(slashingEffect, this.transform.position, Quaternion.Euler(0,180,-40),this.transform);
            }
            else if (direction.x >= 0.5f || direction.x <= -0.5f)
            {
                //横
                if (z == zRight)
                {
                    Instantiate(slashingEffect, this.transform.position, Quaternion.Euler(0, 0, 0), this.transform);
                }
                else
                {
                    Instantiate(slashingEffect, this.transform.position, Quaternion.Euler(0, 180, 0), this.transform);
                }
                
            }
        }
        


        public bool IsGrounded() => isGrounded;
    }
}