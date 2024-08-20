#region

using System;
using System.Linq;
using Core.Utilities;
using Feature.Common.Parameter;
using Feature.Enemy;
using Feature.Interface;
using Main.Environment;
using UnityEngine;

#endregion

namespace Main.Factory
{
    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField] private EnemiesSetting settings;

        public GetTransform GetPlayerTransform;

        public void Subscribe()
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
                enemyStart.OnRequestSpawn = t => SpawnEnemy(enemyStart, t);
            }
        }

        private IEnemy SpawnEnemy(EnemyStart start, Transform t)
        {
            var enemyRef = settings.reference.Find(x => x.type == start.SpawnEnemyType);
            var enemy = ObjectFactory.CreateObject(enemyRef.reference, t.position, t.rotation);
            OnAddField?.Invoke(enemy);
            var enemyComponent = enemy.GetComponent<IEnemy>();
            var enemyParams = enemy.GetComponent<IEnemyAgent>();
            enemyParams.SetParams(enemyRef.parameters);
            enemyParams.SetPlayerTransform(GetPlayerTransform());
            enemyParams.SetPatrolPoints(start.Points);
            enemyComponent.SetHealth(enemyRef.parameters.maxHp);
            enemyComponent.Execute();
            enemyComponent.OnHealth0Event += () => OnRemoveField?.Invoke(enemy);
            return enemyComponent;
        }

        public event Action<GameObject> OnAddField;
        
        public event Action<GameObject> OnRemoveField; 
    }
}