#region

using System;
using System.Collections.Generic;
using Feature.Common.Constants;
using UnityEngine;

#endregion

namespace Feature.Interface
{
    public delegate uint GetHealth();

    public interface IEnemyAgent
    {
        public GetHealth OnGetHealth { set; }

        public EnemyType EnemyType { get; }

        public Action RequireDestroy { set; }
        public void FlowExecute();

        public void FlowCancel();

        public void SetParams(EnemyParams @params);

        public void SetPatrolPoints(List<Vector3> pts);

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker);

        public event Action OnTakeDamageEvent;

        public void Delete();
    }
}