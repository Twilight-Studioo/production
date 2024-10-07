using System;
using UnityEngine;

namespace Feature.Component
{
    public class CircularSaw:MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 0.05f;

        [SerializeField] private Transform[] wayPoints;
        [SerializeField] private float moveSpeed = 2f;
        private int currentWaypointIndex = 0;
        private bool isReturning = false;
        
        void Update()
        {
            transform.Rotate(new Vector3(0,rotationSpeed,0));
            if (wayPoints.Length != 0)
            {
                MoveWayPoints();
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
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }
}