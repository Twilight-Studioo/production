﻿using System;
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
            PlayRandomAnimation();
        }

        public void SheatingSword()
        {
            sword.SetActive(false);
            swordReverse.SetActive(true);
        }

        public void OnClickStage()
        {
            animator.Play("StageSelectA");
        }

        public void StageSelect()
        {
            animator.Play("StageSelectB");
        }
        private void PlayRandomAnimation()
        {
            sword.SetActive(true);
            swordReverse.SetActive(false);
            int randomChoice = Random.Range(0, 2);
            var animationStateName = randomChoice == 0 ? "Player_katana" : "Player_swap";
            
            animator.Play(animationStateName, 0, 0f);
        }
    }
}