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
        [SerializeField] private GameObject item;               // �������烉���_���ɑI�Ԃ悤�ɂ������i��X�j
        [SerializeField] private uint spawnQuantity = 1;        // 1�x�ɉ��X�|�[�����邩
        [SerializeField] private float spawnDistance = 20.0f;   // �A�C�e�����X�|�[�����n�߂鋗��
        [SerializeField] private float RespawnTimeSec = 5.0f;   // ���X�|�[������܂ł̕b��

        public GetTransform GetPlayerTransform;
        
        private bool isCt = false;

        private void Awake()
        {
            if(this.item == null)
            {
                Debug.LogWarning("ItemSpawner �ɃA�C�e�����Z�b�g����Ă��܂���I");
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

            if(Vector3.Distance(transform.position, GetPlayerTransform().position)�@> spawnDistance)
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
    
