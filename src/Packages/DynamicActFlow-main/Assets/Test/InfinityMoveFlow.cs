using System.Collections;
using DynamicActFlow.Runtime.Core.Action;
using DynamicActFlow.Runtime.Core.Flow;
using UnityEngine;

namespace Test
{
    public class InfinityMoveFlow: FlowScope
    {
        [SerializeField] private Transform target;

        private void Awake()
        {
            autoPlay = true;
        }
        protected override IEnumerator Flow(IFlowBuilder context)
        {
            for (var _ = 0; _ < 2; _++)
            {
                yield return Wait(2f);

                yield return Action("MoveLoop")
                    .Param("Direction", Vector3.right)
                    .Param("LoopCount", 10)
                    .Param("Range", 5f)
                    .IfEnd(
                        Trigger("Distance")
                            .Param("Target", 1f)
                            .Param("Object", target)
                            .Build()
                    )
                    .Build();
                
            }
        }
    }
}