#region

using System;
using System.Linq;
using Core.Utilities;
using Feature.Common.Parameter;
using Feature.Component.Environment;
using Feature.Interface;
using Feature.Presenter;
using UnityEngine;
using VContainer;

#endregion

namespace Main.Factory
{
    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField] private EnemiesSetting settings;
        
        [Inject] private DamageEffectFactory damageEffectFactory;

        private readonly IObjectUtil objectUtil = new ObjectUtil();

        public void Subscribe()
        {
            if (settings == null)
            {
                throw new("Settings is not set");
            }
            damageEffectFactory.CheckNull();

            settings.SettingValidate();

            var points = objectUtil.FindObjectsOfType<EnemyStart>();
            foreach (var enemyStart in points)
            {
                try
                {
                    if (settings.GetEnemyTypes().All(x => x != enemyStart.SpawnEnemyType))
                    {
                        throw new($"EnemyType {enemyStart.SpawnEnemyType} is not found in settings");
                    }

                    enemyStart.OnRequestSpawn = t => SpawnEnemy(enemyStart, t);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    throw;
                }
            }
        }

        private IEnemy SpawnEnemy(EnemyStart start, Transform t)
        {
            var enemyRef = settings.reference.Find(x => x.type == start.SpawnEnemyType);
            var enemy = ObjectFactory.Instance.CreateObject(enemyRef.reference, t.position, t.rotation);
            var enemyComponent = enemy.GetComponent<IEnemy>();
            var agent = enemy.GetComponent<IEnemyAgent>();
            var presenter = new EnemyPresenter(enemyComponent, agent, start.GetParam ?? enemyRef.parameters);
            OnAddField?.Invoke(presenter);
            enemyComponent.OnHealth0Event += () => OnRemoveField?.Invoke(presenter);
            enemyComponent.OnDamageEvent += (damage, hitPoint) =>
            {
                var rotation = Quaternion.LookRotation(enemy.transform.position - hitPoint);
                Debug.Log($"rotation {rotation}");
                damageEffectFactory.PlayEffectAtPosition(enemy.transform.position, rotation, DamageEffectFactory.Type.Enemy);
                return new DamageResult.Damaged(enemy.transform);
            };
            presenter.Execute(start.Points);
            return enemyComponent;
        }

        public event Action<IEnemyPresenter> OnAddField;

        public event Action<IEnemyPresenter> OnRemoveField;
    }
}