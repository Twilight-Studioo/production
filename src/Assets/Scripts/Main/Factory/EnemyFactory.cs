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

        private readonly IObjectUtil objectUtil = new ObjectUtil();

        public GetTransform GetPlayerTransform;

        public void Subscribe()
        {
            if (settings == null)
            {
                throw new("Settings is not set");
            }

            settings.SettingValidate();

            var points = objectUtil.FindObjectsOfType<EnemyStart>();
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
            var enemy = ObjectFactory.Instance.CreateObject(enemyRef.reference, t.position, t.rotation);
            var enemyComponent = enemy.GetComponent<IEnemy>();
            var agent = enemy.GetComponent<IEnemyAgent>();
            var presenter = new EnemyPresenter(enemyComponent, agent, start.GetParam ?? enemyRef.parameters);
            OnAddField?.Invoke(presenter);
            enemyComponent.OnHealth0Event += () => OnRemoveField?.Invoke(presenter);
            presenter.Execute(GetPlayerTransform(), start.Points);
            return enemyComponent;
        }

        public event Action<IEnemyPresenter> OnAddField;

        public event Action<IEnemyPresenter> OnRemoveField;
    }
}