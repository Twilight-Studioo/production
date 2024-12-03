using System.Collections;
using DynamicActFlow.Runtime.Core.Action;
using DynamicActFlow.Runtime.Core.Flow;

namespace Mock
{
    public class FlowMock: FlowScope
    {
        protected override IEnumerator Flow(IFlowBuilder context)
        {
            yield return Action("MoveLoop")
                .Param("loopCount", 2)
                .Build();
        }
        
    }
}