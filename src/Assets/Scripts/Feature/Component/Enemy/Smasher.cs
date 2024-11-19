using System;
using System.Collections;
using Core.Utilities;
using Feature.Common.Constants;
using Feature.Common.Parameter;
using Feature.Interface;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;
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
        private NavMeshAgent Agent;

        private float playerDistance = 0;
        private Vector3 positionAtAttack;

        private bool UpperAttack = false;

        [SerializeField] private GameObject Debris;
        

        private void Start()
        {
            bossRb = GetComponent<Rigidbody>();
            Agent = GetComponent<NavMeshAgent>();
            StartCoroutine(Attack());
        }
        
        private void Update()
        {
            playerTransform = ObjectFactory.Instance.FindPlayer()?.transform;
            xDistance = Mathf.Abs(transform.position.x - positionAtAttack.x);
        }

        private IEnumerator Attack()
        {
            rnd = Random.Range(1, 2);
            switch (rnd)
            {
                case 1:
                    yield return ChargeAttack();
                    yield return new WaitForSeconds(bossPrams.chargeIntervalSec); 
                    /*Upper();
                    yield return null;
                    FallAttack();
                    yield return new WaitUntil(() => onGround == true);
                    yield return null;
                    DebrisAttack();
                    yield return new WaitForSeconds(bossPrams.debrisAttackIntervalSec);*/
                    
                    break;
            }

            StartCoroutine(Attack());
        }

        private IEnumerator ChargeAttack()
        {
            yield return new WaitForSeconds(bossPrams.chargeTime);
            CurrentDistance();
            if (playerDistance < 0)
            {
                //StartCoroutine(MoveTowardsTarget(bossPrams.chargeSpeed, Vector3.right * bossPrams.chargeDistance));
                bossRb.AddForce(bossPrams.chargeSpeed*Vector3.right);
            }
            else
            {
                //StartCoroutine(MoveTowardsTarget(bossPrams.chargeSpeed, -Vector3.right * bossPrams.chargeDistance));
                bossRb.AddForce(bossPrams.chargeSpeed*Vector3.left);
            }

            yield return new WaitForSeconds(1);
            bossRb.velocity = Vector3.zero;
        }

        private void Upper()
        {
            bossRb.AddForce(0,bossPrams.upperHeight*100,0);
        }

        private void Jump()
        {
            bossRb.AddForce(0,bossPrams.upperHeight*100,0);
        }

        private void FallAttack()
        {
            CurrentDistance();
            var absoluteDistance = ConvertAbsolute(playerDistance);
            Debug.Log(playerDistance);
            if (absoluteDistance <= bossPrams.fallAttackDistance)
            {
                Debug.Log("playerDistance <= bossPrams.fallAttackDistance");
                StartCoroutine(MoveTowardsTarget(bossPrams.fallSpeed,positionAtAttack));
                bossRb.velocity = Vector3.zero;
            }
            else
            {
                Debug.Log("else");
                if (playerDistance < 0)
                {
                    transform.Translate(bossPrams.fallAttackDistance,0,0);
                }
                else
                {
                    transform.Translate(-bossPrams.fallAttackDistance,0,0);
                }
            }
        }

        private IEnumerator MoveTowardsTarget(float speed,Vector3 target)
        {
            while (xDistance > 0)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    speed * Time.deltaTime
                );
                yield return null;
            }
            bossRb.velocity = Vector3.zero;
        }

        private void DebrisAttack()
        {
            Instantiate(Debris,transform.position,Quaternion.identity);
            var debrisRb = Debris.GetComponent<Rigidbody>();
            debrisRb.AddForce(-bossPrams.debrisSpeed,0,0);
        }

        private void Slap()
        {
            
        }

        private float ConvertAbsolute(float distance)
        {
            var absolute = Mathf.Abs(distance);
            return absolute;
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