#region

using System.Collections.Generic;
using Feature.Common.Constants;
using UnityEngine;

#endregion

namespace Feature.Common.Environment
{
    public class EnemyStart : MonoBehaviour
    {
        [SerializeField] private EnemyType spawnEnemyType = EnemyType.SimpleEnemy1;

        [SerializeField, Header("スポーン距離"), Tooltip("プレイヤーからのスポーン距離"),]
        private Vector2 resumeDistance = new(20f, 10f);

        [SerializeField, Header("巡回地点"),] private List<Vector3> points;

        public EnemyType SpawnEnemyType => spawnEnemyType;

        private void OnDrawGizmos()
        {
            Gizmos.color = new(0f, 1f, 0f, 0.7f);
            Gizmos.DrawSphere(transform.position, 0.4f);
        }


        private void OnDrawGizmosSelected()
        {
            PointGizmos();
            SpawnAreaGizmos();
        }

        private void SpawnAreaGizmos()
        {
            Gizmos.color = new(1f, 0.92156863f, 0.015686275f, 0.2f);
            // set line width
            Gizmos.DrawCube(transform.position, new(resumeDistance.x, resumeDistance.y, 1f));
        }

        private void PointGizmos()
        {
            if (points == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            for (var i = 0; i < points.Count; i++)
            {
                Gizmos.DrawSphere(points[i], 0.2f);
                if (i + 1 < points.Count)
                {
                    Gizmos.DrawLine(points[i], points[i + 1]);
                }
            }
        }
    }
}