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
        [SerializeField] private GameObject slashingEffect;
        [SerializeField] private GameObject dagger;
        public bool Right=true;
        public float hx;
        public float vy;

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
                Right = true;
            }
            else if (direction < 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                direction = direction * -1;
                Right = false;
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

        public void Attack(float degree)
        { 
            if (degree == 0&&Right==false)//degree == 0&&transform.rotation.y==-180f
            {
                Instantiate(slashingEffect, this.transform.position, Quaternion.Euler(0,0,180),this.transform);
            }
            else 
                Instantiate(slashingEffect, this.transform.position, Quaternion.Euler(0,0,degree),this.transform);
        }

        public void Dagger(float degree,float h,float v)
        {
            GameObject instantiateDagger;
            if (degree == 0&&Right==false)
            {
               instantiateDagger = Instantiate(dagger, this.transform.position, Quaternion.Euler(0, 0, -180), this.transform);
            }
            else
               instantiateDagger = Instantiate(dagger, this.transform.position, Quaternion.Euler(0, 0, degree), this.transform);

            if (h == 0 && v == 0)
            {
                if (Right) 
                {
                    h = 1;
                }
                else 
                    h = -1;
            }
            DaggerView daggerView = instantiateDagger.GetComponentInChildren<DaggerView>();
            daggerView.HorizontalVertical(h,v);
        }
        public bool IsGrounded() => isGrounded;
    }
}