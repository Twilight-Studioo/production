using UnityEngine;
using System;

namespace Script.Feature.View
{
    public class PlayerView: MonoBehaviour
    { 
        public event Action OnJump;
        public event Action OnAttack;

        private Rigidbody rb;
        private bool isGrounded; // 地面に接触しているかどうかのフラグ

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }

        public void Move(float direction,float jumpMove)
        {
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
        public void SwapMode()
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0.2f;
            }
            else
                Time.timeScale = 1;
        }
        public bool IsGrounded()
        {
            return isGrounded;
        }
    }
}