#region

using Core.Utilities;
using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.Component
{
    public class CircularSaw : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 0.05f;

        [SerializeField] private Transform[] wayPoints;
        [SerializeField] private float moveLineSpeed = 2f;
        [SerializeField] private float moveRunawaySpeed = 20f;
        [SerializeField] private bool MoveRight = true;
        [SerializeField] private GameObject original;
        [SerializeField] private float repopTimeValue = 5f;

        private readonly uint damage = 10;
        private readonly Vector3 directionMovement = new(1, 0, 0);

        private Rigidbody circularsaw;
        private SphereCollider circularsawIsTrigger;
        private int currentWaypointIndex;
        private Vector3 element0;
        private bool isReturning;

        private bool isSpawn;

        private bool outLine;
        private float repopTime;

        private GameObject saw;

        private void Start()
        {
            circularsaw = GetComponent<Rigidbody>();
            circularsawIsTrigger = GetComponent<SphereCollider>();
            element0 = wayPoints[0].position;
            transform.position = element0;
            repopTime = repopTimeValue;
        }

        private void Update()
        {
            if (MoveRight)
            {
                transform.Rotate(0, 0, -rotationSpeed);
            }
            else
            {
                transform.Rotate(0, 0, rotationSpeed);
            }

            if (wayPoints.Length != 0 && !outLine)
            {
                MoveWayPoints();
                outLine = gameObject.GetComponent<DamagedTrigger>().IsSwapped();
            }
            else if (outLine)
            {
                Runaway();
                if (repopTime <= 0 && !isSpawn)
                {
                    Repop();
                }
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
                other.gameObject.GetComponent<IEnemy>().OnDamage(damage, transform.position, transform);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<IDamaged>().OnDamage(damage, transform.position, transform);
            }

            if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<IEnemy>().OnDamage(damage, transform.position, transform);
            }
        }

        private void MoveWayPoints()
        {
            var targetWayPoint = wayPoints[currentWaypointIndex];

            // 現在位置からウェイポイントまでの距離
            var distance = Vector3.Distance(transform.position, targetWayPoint.position);

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
            var direction = (targetWayPoint.position - transform.position).normalized;
            transform.position += direction * (moveLineSpeed * Time.deltaTime);
        }

        private void Runaway()
        {
            repopTime -= Time.deltaTime;
            circularsawIsTrigger.isTrigger = false;
            circularsaw.useGravity = true;
            if (MoveRight)
            {
                circularsaw.MovePosition(transform.position + directionMovement * moveRunawaySpeed * Time.deltaTime);
            }
            else
            {
                circularsaw.MovePosition(transform.position + directionMovement * -moveRunawaySpeed * Time.deltaTime);
            }
        }

        private void Repop()
        {
            isSpawn = true;
            outLine = false;
            circularsawIsTrigger.isTrigger = true;
            circularsaw.useGravity = false;
            transform.position = element0;
            saw = ObjectFactory.Instance.CreateObject(this.gameObject, original.transform.position, Quaternion.identity);
            gameObject.GetComponent<DamagedTrigger>().Delete();
        }
    }
}