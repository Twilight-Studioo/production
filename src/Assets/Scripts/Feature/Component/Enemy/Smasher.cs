using System;
using Feature.Common.Constants;
using Feature.Common.Parameter;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Feature.Component.Enemy
{
    public class Smasher : MonoBehaviour
    {
        [SerializeField] private SmasherPrams bossPrams;
        [SerializeField] private GameObject Player;
        public EnemyType EnemyType => EnemyType.Smasher;

        private bool onPlayer = false;

        private float coolTime;
        private bool canAttack = false;
        private bool hit = false;
        private Rigidbody rb;
        private int rnd;
        private Rigidbody playerRb;

        private float playerDistance = 0;

        private bool UpperAttack = false;

        private void Start()
        {
            coolTime = 0;
            playerRb = Player.GetComponent<Rigidbody>();
            rb = this.gameObject.GetComponent<Rigidbody>();
        }
        
        private void Update()
        {
            ManageCooldown();
            if (canAttack)
            {
                Attack();
            }
        }

        private void Attack()
        {
            CurrentDistance();
            rnd = Random.Range(1, 3);
            switch (rnd)
            {
                case 1:
                    ChargeAttack();
                    break;
                case 2:
                    Upper();
                    break;
            }
        }

        private void ManageCooldown()
        {
            if (coolTime > 0)
            { 
                coolTime -= Time.deltaTime;
            }
            else if(coolTime <= 0)
            { 
                canAttack = true;
            }
        }

        private void ChargeAttack()
        {
            rb.transform.Translate(-Vector3.right*bossPrams.chargeDistance);
            coolTime = bossPrams.chargeIntervalSec;
            canAttack = false;
        }

        private void Upper()
        {
            rb.transform.Translate(Vector3.up*bossPrams.upperHeight);
            
            coolTime = bossPrams.upperIntervalSec;
            canAttack = false;
            UpperAttack = true;
        }

        private void Slap()
        {
            
        }

        private void CurrentDistance()
        {
            playerDistance = DistancePlayer();
        }

        private float DistancePlayer()
        {
            return  Vector3.Distance(playerRb.position, rb.position);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                hit = true;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                hit = false;
            }
        }
    }
}