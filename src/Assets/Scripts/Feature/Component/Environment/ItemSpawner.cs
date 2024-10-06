#region

using System;
using System.Collections.Generic;
using Feature.Common.Constants;
using Feature.Interface;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

#endregion

namespace Feature.Component.Environment
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject item;               // 複数からランダムに選ぶようにしたい（後々）
        [SerializeField] private uint spawnQuantity = 1;        // 1度に何個スポーンするか
        [SerializeField] private float spawnDistance = 20.0f;   // アイテムをスポーンし始める距離
        [SerializeField] private float RespawnTimeSec = 5.0f;   // リスポーンするまでの秒数

        public GetTransform GetPlayerTransform;
        
        private bool isCt = false;

        private void Awake()
        {
            if(this.item == null)
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
            if(isCt || GetPlayerTransform() == null)
            {
                return;
            }

            if(Vector3.Distance(transform.position, GetPlayerTransform().position)　> spawnDistance)
            {
                return;
            }

            Spawn();
        }

        private void Spawn()
        {
            isCt = true;

            var pos = transform.position;

            for(int i = 0; i < spawnQuantity; i++)
            {
                Instantiate(item, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
            }

            Observable
                .Timer(TimeSpan.FromSeconds(RespawnTimeSec))
                .Subscribe(_ => { isCt = false; });
        }
    }
}
    
