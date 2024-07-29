#region

using System.Collections;
using System.Collections.Generic;
using DynamicActFlow.Runtime.Core.Action;
using DynamicActFlow.Runtime.Core.Flow;
using UnityEngine;

#endregion

namespace Feature.Enemy
{
    public class SimpleEnemy1Agent : FlowScope
    {
        [SerializeField] public List<Vector3> points;

        protected override IEnumerator Flow(IFlowBuilder context)
        {
            yield return Action("PointsAIMoveTo")
                .Param("Points", points)
                .IfEnd(
                    Trigger("PlayerDistance")
                        .Param("Distance", 1.0f)
                        .Build()
                )
                .Build();
            yield return Wait(2.0f);
        }
    }
}