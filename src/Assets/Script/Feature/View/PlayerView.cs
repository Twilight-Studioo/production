using UnityEngine;
using System;
using UniRx;

namespace Script.Feature.View
{
    public class PlayerView: MonoBehaviour
    { 

        private Rigidbody rb;
        private float attack;
        private bool isGrounded; // 地面に接触しているかどうかのフラグ
        private Animator animator;
        private GameObject Sword;
        private string animationName = "SwordAnimation";

        private void Update()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // 現在のアニメーションが指定したアニメーションであり、かつそのアニメーションが終了したかどうかを確認
            if (stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1.0f)
            {
                StopAnimation();
            }

        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            Sword = transform.Find("Sword").gameObject;
            animator = Sword.GetComponent<Animator>();
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

        public void Attack(float modelAttack)
        {
            attack = modelAttack;
            Sword.SetActive(true);
            animator.SetBool("OnAttack",true);
        }

        public void AttackDamege(GameObject enemy)
        {
            //enemy.GetComponent<EnemyView>(attack);
            Destroy(enemy);
        }
        private void StopAnimation()
        {
            animator.SetBool("OnAttack",false);
            Sword.SetActive(false);
        }
        public bool IsGrounded()
        {
            return isGrounded;
        }
    }
}