using System;
using UnityEngine;
using Random = UnityEngine.Random;

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
        }

        public void SheatingSword()
        {
            sword.SetActive(false);
            swordReverse.SetActive(true);
        }

        public void OnClickStage()
        {
            animator.Play("A");
            PlayRandomAnimation();
        }

        public void OnClickContinue()
        {
            PlayRandomAnimation();
        }
        private void PlayRandomAnimation()
        {
            sword.SetActive(true);
            swordReverse.SetActive(false);
           // if (animator == null) return;
           // Debug.Log("aiueo");
            int randomChoice = Random.Range(0, 2); // 0 or 1
            var animationStateName = randomChoice == 0 ? "A" : "B";
            
            animator.Play(animationStateName, 0, 0f);
        }
    }
}