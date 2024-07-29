#region

using System;
using System.Linq;
using Feature.Common.Constants;
using Feature.Common.Environment;
using Feature.Common.Parameter;
using UnityEngine;

#endregion

namespace Main.Factory
{
    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField] private EnemiesSetting settings;

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
            }
        }

        private void SpawnEnemy(EnemyType type)
        {
            var enemyRef = settings.reference.Find(x => x.type == type);
            var enemy = Instantiate(enemyRef.reference, new(0, 2, 0), Quaternion.identity);
            OnAddField?.Invoke(enemy);
        }

        public event Action<GameObject> OnAddField;
    }
}