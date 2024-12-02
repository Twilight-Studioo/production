#region

using System;
using System.Collections.Generic;
using Core.Utilities;
using UniRx;
using UnityEditor;
using UnityEngine;
using ObjectFactory = Core.Utilities.ObjectFactory;

#endregion

namespace Feature.Component.Environment
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject item; // 複数からランダムに選ぶようにしたい（後々）
        [SerializeField] private uint spawnQuantity = 1; // 1度に何個スポーンするか
        [SerializeField] private float spawnDistance = 20.0f; // アイテムをスポーンし始める距離
        [SerializeField] private float respawnTimeSec = 5.0f; // リスポーンするまでの秒数
        
        [SerializeField] private List<Vector3> spawnPoints;

        private bool isCt;

        private Transform playerTransform;

        private void Awake()
        {
            if (item == null)
            {
                Debug.LogWarning("ItemSpawner にアイテムがセットされていません！");
            }
        }


        private void FixedUpdate()
        {
            SpawnCheck();
        }

        private void SpawnCheck()
        {
            if (isCt)
            {
                return;
            }

            if (playerTransform is null)
            {
                playerTransform = ObjectFactory.Instance.FindPlayer()?.transform;
            }

            if (playerTransform is null ||
                Vector3.Distance(transform.position, playerTransform.position)　> spawnDistance)
            {
                return;
            }

            Spawn();
        }

        private void Spawn()
        {
            isCt = true;

            var pos = spawnPoints.RandomElement();
            if (pos == default)
            {
                pos = transform.position;
            }

            for (var i = 0; i < spawnQuantity; i++)
            {
                ObjectFactory.Instance.CreateObject(item, new(pos.x, pos.y, 0), Quaternion.identity);
            }

            Observable
                .Timer(TimeSpan.FromSeconds(respawnTimeSec))
                .Subscribe(_ => { isCt = false; });
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            // 全てのポイントに対して球を描画
            for (var i = 0; i < spawnPoints.Count; i++)
            {
                var worldPosition = spawnPoints[i];
                Gizmos.DrawSphere(worldPosition, 0.4f);

                // 操作可能なハンドルを追加
                var newWorldPosition = Handles.PositionHandle(worldPosition, Quaternion.identity);

                if (newWorldPosition != worldPosition)
                {
                    Undo.RecordObject(this, "Move Spawn Point"); // Undo対応
                    spawnPoints[i] = transform.InverseTransformPoint(newWorldPosition);
                }
            }
        }
#endif
    }
}