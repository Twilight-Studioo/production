#region

using System;
using System.Collections.Generic;
using Feature.Common.Constants;
using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.Common.Parameter
{
    [CreateAssetMenu(fileName = "EnemiesSetting.asset", menuName = "EnemiesSetting", order = 0)]
    public class EnemiesSetting : ScriptableObject
    {
        public List<SerializedEnemyType> reference;

        public List<EnemyType> GetEnemyTypes()
        {
            var types = new List<EnemyType>();
            foreach (var enemyType in reference)
            {
                types.Add(enemyType.type);
            }

            return types;
        }


        public void SettingValidate()
        {
            GetEnemyTypes().ForEach(type =>
            {
                // 同じtypeが存在しないかチェック
                if (GetEnemyTypes().FindAll(x => x == type).Count > 1)
                {
                    throw new($"Duplicate EnemyType {type}");
                }
            });

            reference.ForEach(x =>
            {
                if (x.reference.GetComponent<IEnemy>() == null)
                {
                    throw new($"EnemyType {x.type} is not implement IEnemy");
                }
            });
            Debug.Log("EnemiesSetting Success Validate");
        }
    }

    [Serializable]
    public class SerializedEnemyType
    {
        public EnemyType type;
        public GameObject reference;
        public EnemyParams parameters;
    }
}