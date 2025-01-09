using System.Collections;
using System.Collections.Generic;
using Core.Utilities;
using Feature.Common.Constants;
using Feature.Common.Parameter;
using Feature.Interface;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ObjectFactory = Core.Utilities.ObjectFactory;
using Random = UnityEngine.Random;

namespace Feature.Component.Enemy
{
    public class Smasher : MonoBehaviour, IDamaged
    {
        [SerializeField] private SmasherPrams bossPrams;
        public EnemyType EnemyType => EnemyType.Smasher;

        public bool onGround = true;
        private float xDistance = 0;
        private bool hit = false;
        private int rnd;
        private Transform playerTransform;
        private Rigidbody bossRb;
        private Vector3 playerPosition;
        private bool playerRightSide = false;
        private bool chargeAttack = false;
        private bool fallAttack = false;
        private bool upper = false;
        private GameObject mine;
        private GameObject debris;
        private GameObject debris2;
        private int health;
        private float lastDamageTime = 0f;
        private bool kick = false;
        private bool slap = false;
        [SerializeField] private Slider bossHealthBar;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform strikePoint;

        private float playerDistance = 0;
        private Vector3 positionAtAttack;
        [SerializeField] private Collider kickCollider;
        [SerializeField] private Collider slapCollider;
        [SerializeField] private GameObject spawnPoint;

        private readonly IReactiveProperty<float> speed = new ReactiveProperty<float>(0f);
        private Vector3 previousPosition;

        [SerializeField] private GameObject debrisPrefab;
        [SerializeField] private List<GameObject> swapItemPrefab;
        [SerializeField] private GameObject minePrefab;

        private bool alive = true;

        private void Start()
        {
            bossRb = GetComponent<Rigidbody>();
            health = (int)bossPrams.health;
            UpdateHealth();
            StartCoroutine(Attack());
        }

        private void Update()
        {
            playerTransform = ObjectFactory.Instance.FindPlayer()?.transform;
            if (playerTransform == null)
            {
                return;
            }
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
            if (health <= 0 && alive)
            {
                StartCoroutine(Dead());
            }
        }

        private IEnumerator Attack()
        {
            rnd = Random.Range(1, 8);
            switch (rnd)
            {
                case 1:
                    yield return StartCoroutine(ChargeAttack());
                    yield return StartCoroutine(Upper());
                    if (hit)
                    {
                        yield return StartCoroutine(Slap());
                        hit = false;
                    }
                    else
                    {
                        yield return StartCoroutine(FallAttack());
                    }
                    break;

                case 2:
                    yield return StartCoroutine(Jump());
                    yield return StartCoroutine(FallAttack());
                    break;

                case 3:
                    yield return StartCoroutine(Upper());
                    if (hit)
                    {
                        yield return StartCoroutine(Slap());
                        hit = false;
                    }
                    else
                    {
                        yield return StartCoroutine(FallAttack());
                    }

                    break;

                case 4:
                    yield return StartCoroutine(ChargeAttack());
                    animator.SetTrigger("ReturnStandby");
                    yield return StartCoroutine(ChargeAttack());
                    break;

                case 5:
                    yield return StartCoroutine(StrikeMine());
                    yield return StartCoroutine(Jump());
                    yield return StartCoroutine(FallAttack());
                    break;

                case 6:
                    // yield return StartCoroutine(StrikeMine());
                    break;

                case 7:
                    yield return StartCoroutine(StrikeMine());
                    yield return StartCoroutine(ChargeAttack());
                    break;
            }

            animator.SetTrigger("ReturnStandby");

            if (alive)
            {
                StartCoroutine(Attack());
            }
        }

        private IEnumerator ChargeAttack()
        {
            Debug.Log("突進");
            chargeAttack = true;
            yield return new WaitForSeconds(bossPrams.chargeTime);
            animator.SetTrigger("OnForwardattack");
            CurrentDistance();
            Debug.Log(playerDistance);
            bossRb.AddRelativeForce(bossPrams.chargeSpeed * Vector3.forward);
            yield return new WaitForSeconds(bossPrams.chargeAttackTime);
            bossRb.velocity = Vector3.zero;
            chargeAttack = false;
            yield return new WaitForSeconds(bossPrams.chargeIntervalSec);
        }

        private IEnumerator Upper()
        {
            Debug.Log("アッパー");
            yield return new WaitForSeconds(bossPrams.upperOccurrenceTime);
            upper = true;
            kickCollider.OnTriggerEnterAsObservable().Where(_ => upper == true)
                .Subscribe(other =>
                {
                    var damaged = other.gameObject.GetComponent<IDamaged>();
                    if (damaged != null)
                    {
                        var hitPoint = new Vector3(other.gameObject.transform.position.x,
                            other.gameObject.transform.position.y - 5f,
                            other.gameObject.transform.position.z);
                        damaged.OnDamage(bossPrams.upperDamage, hitPoint, transform);
                        other.gameObject.GetComponent<Rigidbody>().AddForce(0, bossPrams.upperHeight, 0);
                        hit = true;
                        upper = false;
                        Debug.Log("Upper Hit!!");
                    }
                })
                .AddTo(this);
            animator.SetTrigger("OnUpper");
            bossRb.AddForce(0, bossPrams.upperHeight, 0);
            yield return new WaitForSeconds(bossPrams.upperIntervalSec);
            upper = false;
        }

        private IEnumerator Jump()
        {
            Debug.Log("ジャンプ");
            animator.SetTrigger("OnJump");
            yield return new WaitForSeconds(bossPrams.jumpOccurrenceTime);
            bossRb.AddForce(0, bossPrams.upperHeight, 0);
            yield return new WaitForSeconds(bossPrams.jumpIntervalSec);
        }

        private IEnumerator FallAttack()
        {
            Debug.Log("落下攻撃");
            CurrentDistance();
            yield return new WaitForSeconds(bossPrams.fallAttackOccurrenceTime);
            animator.SetTrigger("Fall");
            fallAttack = true;
            if (playerDistance <= bossPrams.fallAttackDistance)
            {
                StartCoroutine(MoveTowardsTarget(bossPrams.fallSpeed, positionAtAttack));
            }
            else
            {
                if (playerRightSide)
                {
                    StartCoroutine(MoveTowardsTarget(bossPrams.fallSpeed,
                        new Vector3(transform.position.x + bossPrams.fallAttackDistance, positionAtAttack.y,
                            positionAtAttack.z)));
                }
                else
                {
                    StartCoroutine(MoveTowardsTarget(bossPrams.fallSpeed,
                        new Vector3(transform.position.x - bossPrams.fallAttackDistance, positionAtAttack.y,
                            positionAtAttack.z)));
                }
            }

            yield return new WaitUntil(() => onGround == true);
            bossRb.velocity = Vector3.zero;
            yield return new WaitForSeconds(bossPrams.fallAttackIntervalSec);
            fallAttack = false;
            yield return StartCoroutine(DebrisAttack());
        }

        private IEnumerator MoveTowardsTarget(float speed, Vector3 target)
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
            yield return new WaitForSeconds(0.2f);
            debris2 = ObjectFactory.Instance.CreateObject(debrisPrefab, spawnPoint.transform.position,
                 Quaternion.identity);
            //var swapitem = swapItemPrefab.RandomElement();
                    //debris2 = ObjectFactory.Instance.CreateObject(swapitem, spawnPoint.transform.position, spawnPoint.transform.rotation);
            //debris2.GetComponent<Rigidbody>().AddForce((playerRightSide == true) ? 2 : -2, 10, 0);
            debris = ObjectFactory.Instance.CreateObject(debrisPrefab, spawnPoint.transform.position,
                Quaternion.identity);
            debris.GetComponent<Debris>().Kick();
            yield return new WaitForSeconds(bossPrams.debrisAttackIntervalSec);
        }

        private IEnumerator StrikeMine()
        {
            Debug.Log("地雷発射");
            yield return new WaitForSeconds(bossPrams.mineOccurrenceTime);
            animator.SetTrigger("OnMine");
            mine = ObjectFactory.Instance.CreateObject(minePrefab, strikePoint.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(bossPrams.mineIntervalSec);
        }

        private IEnumerator Slap()
        {
            Debug.Log("ヒラテウチー");
            yield return new WaitForSeconds(bossPrams.slapOccurrenceTime);
            animator.SetTrigger("OnSlap");
            slap = true;
            slapCollider.OnTriggerEnterAsObservable().Where(_ => slap == true)
                .Subscribe(other =>
                {
                    var damaged = other.gameObject.GetComponent<IDamaged>();
                    if (damaged != null)
                    {
                        damaged.OnDamage(bossPrams.slapDamage, transform.position, transform);
                        slap = false;
                    }
                })
                .AddTo(this);
            yield return new WaitForSeconds(bossPrams.slapIntervalSec);
            yield return new WaitUntil(() => onGround == true);
            animator.SetTrigger("OnGround");
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
            return Vector3.Distance(playerTransform.position, transform.position);
        }

        private IEnumerator Dead()
        {
            alive = false;
            Destroy(bossHealthBar);
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene("ClearSmasherScene");
        }

        public DamageResult OnDamage(uint damage, Vector3 hitPoint, Transform attacker)
        {
            var currentTime = Time.time;
            health -= (int)damage;
            if (health < bossPrams.health / 2)
            {
                bossRb.AddRelativeForce(bossPrams.kickbackHalf * Vector3.back);
            }
            else if (health < bossPrams.health / 3)
            {
                bossRb.AddRelativeForce(bossPrams.kickbackOneThird * Vector3.back);
            }
            else if (health < bossPrams.health / 10)
            {
                bossRb.AddRelativeForce(bossPrams.kickbackTenth * Vector3.back);
            }

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
                    other.gameObject.GetComponent<IDamaged>()
                        .OnDamage(bossPrams.chargeAttackDamage, transform.position, transform);
                }
                else if (fallAttack)
                {
                    other.gameObject.GetComponent<IDamaged>()
                        .OnDamage(bossPrams.fallAttackDamage, transform.position, transform);
                }
            }

            if (other.gameObject.CompareTag("Ground"))
            {
                onGround = true;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                onGround = false;
            }
        }
    }
}