#region

using System;
using System.Linq;
using Feature.Common.Constants;
using Feature.Common.Environment;
using Feature.Common.Parameter;
using Feature.Enemy;
using Feature.Interface;
using UnityEngine;

#endregion

namespace Main.Factory
{
    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField] private EnemiesSetting settings;

        public GetTransform GetPlayerTransform;

        private void Awake()
        {
            if (settings == null)
            {
                throw new("Settings is not set");
            }

            settings.SettingValidate();

            var points = FindObjectsOfType<EnemyStart>();
            foreach (var enemyStart in points)
            {
                if (settings.GetEnemyTypes().All(x => x != enemyStart.SpawnEnemyType))
                {
                    throw new($"EnemyType {enemyStart.SpawnEnemyType} is not found in settings");
                }

                enemyStart.GetPlayerTransform = () => GetPlayerTransform();
                enemyStart.OnRequestSpawn = t => SpawnEnemy(enemyStart.SpawnEnemyType, t);
            }
        }

        private IEnemy SpawnEnemy(EnemyType type, Transform transform)
        {
            var enemyRef = settings.reference.Find(x => x.type == type);
            var enemy = Instantiate(enemyRef.reference, transform.position, transform.rotation);
            OnAddField?.Invoke(enemy);
            var enemyComponent = enemy.GetComponent<IEnemy>();
            var enemyParams = enemy.GetComponent<IEnemyAgent>();
            enemyParams.SetParams(enemyRef.parameters);
            enemyParams.SetPlayerTransform(GetPlayerTransform());
            return enemyComponent;
        }

        public event Action<GameObject> OnAddField;
    }
}