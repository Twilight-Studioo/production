using System.Collections;
using Feature.Common.Constants;
using Feature.Common.Parameter;
using Feature.Interface;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using ObjectFactory = Core.Utilities.ObjectFactory;
using Random = UnityEngine.Random;

namespace Feature.Component.Enemy
{
    public class Smasher : MonoBehaviour,IDamaged
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
        private uint health;
        private float lastDamageTime = 0f;
        private bool kick = false;
        [SerializeField] private Slider bossHealthBar;
        [SerializeField] private Animator animator;

        private float playerDistance = 0;
        private Vector3 positionAtAttack;
        [SerializeField] private Collider kickCollider;
        
        private readonly IReactiveProperty<float> speed = new ReactiveProperty<float>(0f);
        private Vector3 previousPosition;
        

        private bool UpperAttack = false;

        [SerializeField] private GameObject debrisPrefab;
        [SerializeField] private GameObject minePrefab;
        
        private void Start()
        {
            bossRb = GetComponent<Rigidbody>();
            health = bossPrams.health;
            UpdateHealth();
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
        

        private void UpdateHealth()
        {
            bossHealthBar.value = (float)health / bossPrams.health;
        }
        
        private IEnumerator Attack()
        {
            rnd = Random.Range(6, 7);
            switch (rnd)
            {
                case 1:
                    Debug.Log("111");
                    yield return StartCoroutine(ChargeAttack());
                    yield return StartCoroutine(Upper());
                    yield return StartCoroutine(FallAttack());
                    break;
                    
                case 2 :
                    Debug.Log("222");
                    yield return StartCoroutine(Jump());
                    yield return StartCoroutine(FallAttack());
                    break;
                
                case 3 :
                    Debug.Log("333");
                    yield return StartCoroutine(StrikeMine());
                    yield return StartCoroutine(ChargeAttack());
                    break;
                
                case 4:
                    Debug.Log("444");
                    yield return StartCoroutine(ChargeAttack());
                    animator.SetTrigger("ReturnStandby");
                    yield return StartCoroutine(ChargeAttack());
                    break;
                
                case 5:
                    Debug.Log("555");
                    yield return StartCoroutine(StrikeMine());
                    yield return StartCoroutine(Jump());
                    yield return StartCoroutine(FallAttack());
                    break;
                
                case 6 :
                    yield return StartCoroutine(StrikeMine());
                    break;
                    
            }
            animator.SetTrigger("ReturnStandby");

            StartCoroutine(Attack());
        }

        private IEnumerator ChargeAttack()
        {
            Debug.Log("突進");
            animator.SetTrigger("OnForwardattack");
            chargeAttack = true;
            yield return new WaitForSeconds(bossPrams.chargeTime);
            CurrentDistance();
            Debug.Log(playerDistance);
            bossRb.AddRelativeForce(bossPrams.chargeSpeed*Vector3.forward);
            yield return new WaitForSeconds(bossPrams.chargeAttackTime);
            bossRb.velocity = Vector3.zero;
            chargeAttack = false;
            yield return new WaitForSeconds(bossPrams.chargeIntervalSec); 
        }

        private IEnumerator Upper()
        {
            Debug.Log("アッパー");
            yield return new WaitForSeconds(bossPrams.upperOccurrenceTime);
            animator.SetTrigger("OnUpper");
            bossRb.AddForce(0,bossPrams.upperHeight,0);
            yield return new WaitForSeconds(bossPrams.upperIntervalSec);
        }

        private IEnumerator Jump()
        {
            Debug.Log("ジャンプ");
            yield return new WaitForSeconds(bossPrams.jumpOccurrenceTime);
            animator.SetTrigger("OnJump");
            bossRb.AddForce(0,bossPrams.upperHeight,0);
            yield return new WaitForSeconds(bossPrams.jumpIntervalSec);
        }

        private IEnumerator FallAttack()
        {
            Debug.Log("落下攻撃");
            yield return new WaitForSeconds(bossPrams.fallAttackOccurrenceTime);
            animator.SetTrigger("Fall");
            fallAttack = true;
            CurrentDistance();
            if (playerDistance <= bossPrams.fallAttackDistance)
            {
                StartCoroutine(MoveTowardsTarget(bossPrams.fallSpeed,positionAtAttack));
            }
            else
            {
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
            yield return StartCoroutine(DebrisAttack());
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
                yield return new WaitForFixedUpdate();
                if (onGround)
                {
                    break;
                }
            }
            bossRb.velocity = Vector3.zero;
        }

        private IEnumerator DebrisAttack()
        {
            Debug.Log("瓦礫攻撃");
            yield return new WaitForSeconds(bossPrams.debrisAttackOccurrenceTime);
            animator.SetTrigger("DebriAttack");
            Instantiate(debrisPrefab,transform.position,Quaternion.identity);
            yield return new WaitForSeconds(bossPrams.debrisAttackIntervalSec);
        }

        private IEnumerator StrikeMine()
        {
            Debug.Log("地雷発射");
            yield return new WaitForSeconds(bossPrams.mineOccurrenceTime);
            animator.SetTrigger("OnMine");
            mine = ObjectFactory.Instance.CreateObject(minePrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(bossPrams.mineIntervalSec);
        }
        

        private void Slap()
        {
            
        }

        private void Kick()
        {
            kickCollider.OnTriggerEnterAsObservable().Where(_ => kick == true)
                .Subscribe(other =>
                {
                    var damaged = other.gameObject.GetComponent<IDamaged>();
                    if (damaged != null)
                    {
                        damaged.OnDamage(bossPrams.kickDamage, transform.position, transform);  
                        kick = false;
                    }
                    else
                    {
                        Debug.LogWarning($"IDamaged が {other.gameObject.name} に存在しません。");
                    }
                })
                .AddTo(this);
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
        
        public DamageResult OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            var currentTime = Time.time;
            health -= damage;
            animator.SetTrigger("OnDamage");
            UpdateHealth();
            if (currentTime - lastDamageTime <= bossPrams.kickTriggerTime)
            {
                animator.SetTrigger("Kick");
                kick = true;
                Kick();
            }
            lastDamageTime = currentTime;
            return new DamageResult.Damaged(transform);
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
                    //other.gameObject.GetComponent<IDamaged>().OnDamage(0,transform.position,transform);
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