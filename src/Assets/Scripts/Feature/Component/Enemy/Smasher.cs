using System;
using System.Collections.Generic;
using Core.Utilities;
using Feature.Common.Constants;
using Feature.Common.Parameter;
using Feature.Interface;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Feature.Component.Enemy
{
    public class Smasher : MonoBehaviour, IEnemyAgent
    {
        [SerializeField] private SmasherPrams bossPrams;
        public EnemyType EnemyType => EnemyType.Smasher;

        private bool onPlayer = false;

        private float coolTime;
        private bool canAttack = false;
        private Rigidbody rb;
        private int rnd;

        private bool UpperAttack = false;

        private void Start()
        {
            coolTime = 0;
            rb = this.gameObject.GetComponent<Rigidbody>();
        }

        public void FlowCancel()
        {
        }

        public void FlowExecute()
        {
        }

        public Action RequireDestroy { set; get; }

        public GetHealth OnGetHealth { get; set; }

        public void SetParams(EnemyParams @params)
        {
            if (@params is SmasherPrams smasherPrams)
            {
                bossPrams = smasherPrams;
            }
        }

        public void SetPatrolPoints(List<Vector3> pts)
        {
        }

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            var imp = (transform.position - attacker.position).normalized;
            imp.y += 10f;
            StartCoroutine(transform.Knockback(imp, 10f, 0.5f));
        }

        public event Action OnTakeDamageEvent;

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
            rb.AddForce(-Vector3.right*bossPrams.chargeDistance,ForceMode.Impulse);
            coolTime = bossPrams.chargeIntervalSec;
            canAttack = false;
        }

        private void Upper()
        {
            rb.AddForce(Vector3.up*bossPrams.upperHeight,ForceMode.Impulse);
            coolTime = bossPrams.upperIntervalSec;
            canAttack = false;
            UpperAttack = true;
        }
        
    }
}