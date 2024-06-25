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

        public void Move(float direction)
        {
            Vector3 movement = transform.right * direction * Time.deltaTime;
            rb.MovePosition(rb.position + movement);
        }

        public void Jump(float jumpForce)
        {
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false; // ジャンプ中になるので接地フラグをfalseにする
            }
        }

        public void AttackAnimation()
        {
            // 攻撃アニメーションやエフェクトの実装
            Debug.Log("Attack!");
        }

        public void OnJumpButtonClicked()
        {
            OnJump?.Invoke(); // Presenterにジャンプを通知
        }

        public void OnAttackButtonClicked()
        {
            OnAttack?.Invoke(); // Presenterに攻撃を通知
        }

        public bool IsGrounded()
        {
            return isGrounded;
        }
    }
}