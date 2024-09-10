using System.Collections.Generic;
using UnityEngine;

namespace Feature.Interface
{
    public interface IEnemyPresenter
    {
        public GameObject GameObject();
        
        public void Execute(
            Transform playerTransform,
            List<Vector3> patrolPoints
        );
    }
}