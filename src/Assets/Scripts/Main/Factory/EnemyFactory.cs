#region

using System;
using System.Linq;
using Core.Utilities;
using Feature.Common.Parameter;
using Feature.Component.Environment;
using Feature.Interface;
using Feature.Presenter;
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
            var enemyComponent = enemy.GetComponent<IEnemy>();
            var agent = enemy.GetComponent<IEnemyAgent>();
            var presenter = new EnemyPresenter(enemyComponent, agent, enemyRef.parameters);
            OnAddField?.Invoke(presenter);
            agent.OnAddSwappableItem += OnAddSwappableItem;
            enemyComponent.OnHealth0Event += () => OnRemoveField?.Invoke(presenter);
            presenter.Execute(GetPlayerTransform(), start.Points);
            return enemyComponent;
        }

        public event Action<IEnemyPresenter> OnAddField;
        
        public event Action<ISwappable> OnAddSwappableItem; 
        
        public event Action<IEnemyPresenter> OnRemoveField; 
    }
}