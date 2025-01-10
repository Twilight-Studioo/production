using UnityEngine;

namespace Feature.Component
{
    public class TitlePlayerAnimation : MonoBehaviour
    {
        [SerializeField] private GameObject sword;
        [SerializeField] private GameObject saya;
        [SerializeField] private GameObject swordReverse;
        private Animator animator;
        private int random;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            PlayRandomAnimation();
        }

        public void SheatingSword()
        {
            sword.SetActive(false);
            swordReverse.SetActive(true);
        }

        public void OnClickStage()
        {
            //animator.Play("StageSelectA");
            animator.SetBool("Selecting", true);
        }

        public void ClickBacktoTitle()
        {
            animator.SetBool("Selecting", false);
        }

        public void StageSelect()
        {
            sword.SetActive(true);
            swordReverse.SetActive(false);
            animator.Play("StageSelectB");
        }
        private void PlayRandomAnimation()
        {
            sword.SetActive(true);
            swordReverse.SetActive(false);
            // int randomChoice = Random.Range(0, 2);
            // var animationStateName = randomChoice == 0 ? "Player_katana" : "Player_swap";
            var animationStateName = "Player_katana";
            
            animator.Play(animationStateName, 0, 0f);
        }
    }
}