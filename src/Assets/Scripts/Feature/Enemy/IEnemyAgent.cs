#region

using System;
using System.Collections.Generic;
using Feature.Common.Parameter;
using UnityEngine;

#endregion

namespace Feature.Enemy
{
    public delegate void OnTakeDamage();

    public interface IEnemyAgent
    {
        public void FlowExecute();

        public void FlowCancel();

        public void SetParams(EnemyParams @params);

        public void SetPlayerTransform(Transform playerTransform);

        public void SetPatrolPoints(List<Vector3> pts);

        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker);

        public event Action OnTakeDamageEvent;
    }
}