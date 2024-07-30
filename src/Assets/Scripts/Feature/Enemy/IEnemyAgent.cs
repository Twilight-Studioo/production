#region

using Feature.Common.Parameter;
using UnityEngine;

#endregion

namespace Feature.Enemy
{
    public interface IEnemyAgent
    {
        public void FlowExecute();

        public void FlowCancel();

        public void SetParams(EnemyParams @params);

        public void SetPlayerTransform(Transform playerTransform);
    }
}