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

        private void Update()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

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

        public void Attack(float modelAttack,Vector2 direction)
        {
            attack = modelAttack;
            Sword.SetActive(true);
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

        public void AttackDamege(GameObject enemy)
        {
            //enemy.GetComponent<EnemyView>(attack);
            Destroy(enemy);
        }
        private void StopAnimation()
        {
            animator.SetBool("Up",false);
            animator.SetBool("UpRight",false);
            animator.SetBool("Right",false);
            animator.SetBool("DownRight",false);
            Sword.SetActive(false);
        }
        public bool IsGrounded()
        {
            return isGrounded;
        }
    }
}