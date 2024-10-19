#region

using System;
using System.Collections.Generic;
using Core.Utilities;
using Feature.Common.Constants;
using Feature.Interface;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

#endregion

namespace Feature.Component.Environment
{
    public delegate IEnemy OnRequestSpawnEvent(Transform transform);

    public class EnemyStart : MonoBehaviour
    {
        [SerializeField] private EnemyType spawnEnemyType = EnemyType.SimpleEnemy1;

        [SerializeField, Header("スポーン距離"), Tooltip("プレイヤーからのスポーン距離"),]
        private Vector2 resumeDistance = new(20f, 10f);

        [SerializeField, Header("巡回地点"),] private List<Vector3> points;

        [SerializeField] private float respawnTimeSec = 5f;

        [SerializeField, Header("上書きする設定"), CanBeNull,]
        private EnemyParams overrideParams;

        private bool canSpawn = true;
        
        private Transform playerTransform;

        private bool isSpawned;

        public OnRequestSpawnEvent OnRequestSpawn;

        [CanBeNull] public EnemyParams GetParam => overrideParams;

        public List<Vector3> Points => points;

        public EnemyType SpawnEnemyType => spawnEnemyType;

        private void FixedUpdate()
        {
            SpawnCheck();
        }

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

        private void SpawnCheck()
        {
            if (!playerTransform)
            {
                playerTransform = ObjectFactory.Instance.FindPlayer()?.transform;
            }
            if (!canSpawn || isSpawned || OnRequestSpawn == null || !playerTransform)
            {
                return;
            }

            if (Vector3.Distance(transform.position, playerTransform.position) > resumeDistance.x)
            {
                return;
            }

            var enemy = OnRequestSpawn(transform);
            if (enemy == null)
            {
                return;
            }

            isSpawned = true;
            canSpawn = false;
            enemy.OnHealth0Event += () =>
            {
                isSpawned = false;
                Observable
                    .Timer(TimeSpan.FromSeconds(respawnTimeSec))
                    .Subscribe(_ => canSpawn = true);
            };
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