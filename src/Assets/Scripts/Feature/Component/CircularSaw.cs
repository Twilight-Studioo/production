using System;
using Codice.Client.BaseCommands.Replication;
using Feature.Interface;
using UnityEngine;

namespace Feature.Component
{
    public class CircularSaw:MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 0.05f;

        [SerializeField] private Transform[] wayPoints;
        [SerializeField] private float moveLineSpeed = 2f;
        [SerializeField] private float moveRunawaySpeed = 20f;
        private int currentWaypointIndex = 0;
        private bool isReturning = false;

        private bool outLine = false;
        private Vector3 directionMovement = new Vector3(1, 0, 0);
        [SerializeField] private bool MoveRight = true;
        
        private uint damage = 10;

        private Rigidbody circularsaw;
        private CapsuleCollider circularsawIsTrigger;

        private Vector3 RepopPosition;
        private bool isSpawn = false;
        [SerializeField] private GameObject original;
        [SerializeField] private float repopTime = 5f;

        private void Start()
        {
            circularsaw = GetComponent<Rigidbody>();
            circularsawIsTrigger = GetComponent<CapsuleCollider>();
            RepopPosition = this.transform.position;
        }

        void Update()
        {
            if (MoveRight)
            {
                transform.Rotate(0,-rotationSpeed,0);
            }
            else
            {
                transform.Rotate(0,rotationSpeed,0);
            }
            
            if (wayPoints.Length != 0 && !outLine)
            {
                MoveWayPoints();
                outLine = this.gameObject.GetComponent<DamagedTrigger>().IsSwapped();
            }
            else if (outLine)
            {
                Runaway();
                if (repopTime <= 0&& !isSpawn)
                {
                    Repop();
                }
            }
        }

        private void MoveWayPoints()
        {
            Transform targetWayPoint = wayPoints[currentWaypointIndex];

            // 現在位置からウェイポイントまでの距離
            float distance = Vector3.Distance(transform.position, targetWayPoint.position);

            // ウェイポイントに到達したら次のウェイポイントへ進む
            if (distance < 0.1f)
            {
                if (!isReturning)
                {
                    currentWaypointIndex++;

                    // 最後のウェイポイントに到達したら復路に切り替える
                    if (currentWaypointIndex >= wayPoints.Length)
                    {
                        isReturning = true;
                        currentWaypointIndex = wayPoints.Length - 1; // 最後のウェイポイント
                    }
                }
                else
                {
                    currentWaypointIndex--;

                    // 最初のウェイポイントに到達したら往路に切り替える
                    if (currentWaypointIndex < 0)
                    {
                        isReturning = false;
                        currentWaypointIndex = 0; // 最初のウェイポイント
                    }
                }
            }
            // 現在のウェイポイントに向かって移動する
            Vector3 direction = (targetWayPoint.position - transform.position).normalized;
            transform.position += direction * (moveLineSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<IDamaged>().OnDamage(damage, transform.position, transform);
            }
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<IEnemy>().OnDamage(damage,transform.position,transform);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<IDamaged>().OnDamage(damage, transform.position, transform);
            }
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<IEnemy>().OnDamage(damage,transform.position,transform);
            }
        }

        private void Runaway()
        {
            repopTime -= Time.deltaTime;
            circularsawIsTrigger.isTrigger = false;
            circularsaw.useGravity = true;
            if (MoveRight)
            {
                circularsaw.MovePosition(transform.position + (directionMovement * moveRunawaySpeed * Time.deltaTime));
            }
            else
            {
                circularsaw.MovePosition(transform.position + (directionMovement * -moveRunawaySpeed * Time.deltaTime));
            }
        }

        private void Repop()
        {
            isSpawn = true;
            /*outLine = false;
            circularsawIsTrigger.isTrigger = true;
            circularsaw.useGravity = false;*/
            Instantiate(original, original.transform.position, Quaternion.identity);
        }
    }
}