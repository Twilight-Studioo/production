#region

using System.Collections.Generic;
using Feature.Common.Constants;
using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.Presenter
{
    public class EnemyPresenter : IEnemyPresenter
    {
        private readonly IEnemyAgent agent;
        private readonly IEnemy enemyView;

        public EnemyPresenter(
            IEnemy enemyView,
            IEnemyAgent agent,
            EnemyParams @params
        )
        {
            this.enemyView = enemyView;
            this.agent = agent;
            this.agent.SetParams(@params);

            this.enemyView.SetHealth(@params.maxHp);
        }

        public void Execute(
            List<Vector3> patrolPoints
        )
        {
            agent.SetPatrolPoints(patrolPoints);
            enemyView.Execute();
        }

        public GameObject GameObject() => enemyView.GameObject();
    }
}