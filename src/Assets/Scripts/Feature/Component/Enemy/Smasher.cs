﻿using System.Collections;
using Feature.Common.Constants;
using Feature.Common.Parameter;
using Feature.Interface;
using UnityEngine;
using ObjectFactory = Core.Utilities.ObjectFactory;
using Random = UnityEngine.Random;

namespace Feature.Component.Enemy
{
    public class Smasher : MonoBehaviour
    {
        [SerializeField] private SmasherPrams bossPrams;
        public EnemyType EnemyType => EnemyType.Smasher;

        private bool onPlayer = false;
        public bool onGround = true;
        private float xDistance = 0;
        private float yDistance = 0;
        private bool canAttack = false;
        private bool hit = false;
        private int rnd;
        private Transform playerTransform;
        private Rigidbody bossRb;
        private Vector3 playerPosition;
        private bool playerRightSide = false;
        private bool chargeAttack = false;
        private bool fallAttack = false;
        private GameObject mine;

        private float playerDistance = 0;
        private Vector3 positionAtAttack;

        private bool UpperAttack = false;

        [SerializeField] private GameObject debrisPrefab;
        [SerializeField] private GameObject minePrefab;
        
        private void Start()
        {
            bossRb = GetComponent<Rigidbody>(); 
            StartCoroutine(Attack());
        }

        private void Update()
        {
            playerTransform = ObjectFactory.Instance.FindPlayer()?.transform;
            xDistance = Mathf.Abs(transform.position.x - positionAtAttack.x);
            var distance = transform.position.x - playerTransform.position.x;
            if (distance < 0)
            {
                playerRightSide = true;
            }
            playerPosition = playerTransform.position;
            var direction = playerPosition - transform.position;
            direction.y = 0;
            var lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.1f);
        }

        private IEnumerator Attack()
        {
            rnd = Random.Range(1, 4);
            switch (rnd)
            {
                case 1:
                    yield return ChargeAttack();
                    yield return new WaitForSeconds(bossPrams.chargeIntervalSec); 
                    Upper();
                    yield return new WaitForSeconds(bossPrams.upperIntervalSec);
                    StartCoroutine(FallAttack());
                    yield return new WaitForSeconds(bossPrams.debrisAttackIntervalSec);
                    break;
                    
                case 2 :
                    Jump();
                    yield return new WaitForSeconds(bossPrams.upperIntervalSec);
                    StartCoroutine(FallAttack());
                    yield return new WaitForSeconds(bossPrams.debrisAttackIntervalSec);
                    break;
                
                case 3 :
                    StrikeMine();
                    yield return new WaitForSeconds(bossPrams.mineIntervalSec);
                    yield return ChargeAttack();
                    yield return new WaitForSeconds(bossPrams.chargeIntervalSec); 
                    break;
            }

            StartCoroutine(Attack());
        }

        private IEnumerator ChargeAttack()
        {
            chargeAttack = true;
            yield return new WaitForSeconds(bossPrams.chargeTime);
            CurrentDistance();
            Debug.Log(playerDistance);
            bossRb.AddRelativeForce(bossPrams.chargeSpeed*Vector3.forward);
            yield return new WaitForSeconds(bossPrams.chargeAttackTime);
            bossRb.velocity = Vector3.zero;
            chargeAttack = false;
        }

        private void Upper()
        {
            bossRb.AddForce(0,bossPrams.upperHeight,0);
        }

        private void Jump()
        {
            bossRb.AddForce(0,bossPrams.upperHeight,0);
        }

        private IEnumerator FallAttack()
        {
            fallAttack = true;
            CurrentDistance();
            Debug.Log(playerDistance);
            if (playerDistance <= bossPrams.fallAttackDistance)
            {
                Debug.Log("playerDistance <= bossPrams.fallAttackDistance");
                StartCoroutine(MoveTowardsTarget(bossPrams.fallSpeed,positionAtAttack));
            }
            else
            {
                Debug.Log("fallAttack" + playerDistance);
                if (playerRightSide)
                {
                    StartCoroutine(MoveTowardsTarget(bossPrams.fallSpeed,new Vector3(transform.
                        position.x + bossPrams.fallAttackDistance,positionAtAttack.y,positionAtAttack.z)));
                }
                else
                {
                    StartCoroutine(MoveTowardsTarget(bossPrams.fallSpeed,new Vector3(transform.
                        position.x - bossPrams.fallAttackDistance,positionAtAttack.y,positionAtAttack.z)));

                }
            }
            yield return new WaitUntil(() => onGround == true);
            bossRb.velocity = Vector3.zero;
            yield return new WaitForSeconds(bossPrams.fallAttackIntervalSec);
            fallAttack = false;
            DebrisAttack();
        }

        private IEnumerator MoveTowardsTarget(float speed,Vector3 target)
        {
            while (xDistance > 0)
            {
                transform.localPosition = Vector3.MoveTowards(
                    transform.position,
                    target,
                    speed * Time.deltaTime
                );
                yield return null;
                if (onGround)
                {
                    break;
                }
            }
            bossRb.velocity = Vector3.zero;
        }

        private void DebrisAttack()
        {
            Instantiate(debrisPrefab,transform.position,Quaternion.identity);
        }

        private void StrikeMine()
        {
            mine = ObjectFactory.Instance.CreateObject(minePrefab, transform.position, Quaternion.identity);
        }

        private void Slap()
        {
            
        }

        private void CurrentDistance()
        {
            playerDistance = DistancePlayer();
            positionAtAttack = playerTransform.position;
        }

        private float DistancePlayer()
        {
            return  Vector3.Distance(playerTransform.position, transform.position);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (chargeAttack)
                {
                    other.gameObject.GetComponent<IDamaged>().OnDamage(bossPrams.chargeAttackDamage,transform.position,transform);
                }
                else if (fallAttack)
                {
                    other.gameObject.GetComponent<IDamaged>().OnDamage(bossPrams.fallAttackDamage,transform.position,transform);
                }
                else
                {
                    other.gameObject.GetComponent<IDamaged>().OnDamage(0,transform.position,transform);
                }
            }

            if (other.gameObject.CompareTag("Ground"))
            {
                onGround = true;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                hit = false;
            }
            
            if (other.gameObject.CompareTag("Ground"))
            {
                onGround = false;
            }
        }
    }
}