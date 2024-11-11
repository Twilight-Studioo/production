using UnityEngine;

namespace Feature.Component
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject katana;
        [SerializeField] private GameObject sheath;
        [SerializeField] private GameObject katanaReverse;
        [SerializeField] private float hideDelay = 0.5f;
        private Animator animator;
        private bool isAttacking;


        private void Awake()
        {
            animator = GetComponent<Animator>();
            FinishAnimation();
            if (katana == null || sheath == null || katanaReverse == null)
            {
                Debug.LogError($"必要なGameObject参照が設定されていません: {gameObject.name}");
                enabled = false;
            }
        }

        public void StartAnimation()
        {
            if (isAttacking) return;
            isAttacking = true;
            katana.SetActive(true);
            sheath.SetActive(true);
        }

        public void FinishAnimation()
        {
            Invoke("HideWeapon", hideDelay);
        }

        private void HideWeapon()
        {
            if (!IsPlayingAttackAnimation())
            {
                isAttacking = false;
                katana.SetActive(false);
                sheath.SetActive(false);
                katanaReverse.SetActive(false);
            }
        }

        public void ThirdAttack()
        {
            isAttacking = true;
            katana.SetActive(false);
            sheath.SetActive(false);
            katanaReverse.SetActive(true);
        }

        private bool IsPlayingAttackAnimation()
        {
            var currentAnimatorState = animator.GetCurrentAnimatorStateInfo(0);
            return isAttacking &&
                   (currentAnimatorState.IsName("SJK_attackA") ||
                    currentAnimatorState.IsName("SJK_attackB") ||
                    currentAnimatorState.IsName("SJK_attackC"));
        }
    }
}