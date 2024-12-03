#region

using System.Collections;
using DynamicActFlow.Runtime.Core.Action;
using UnityEngine;

#endregion

namespace DynamicActFlow.Runtime.Core.Flow
{
    public abstract class FlowScopeBase : MonoBehaviour
    {
        protected abstract IEnumerator Flow(IFlowBuilder context);

        protected abstract void FlowStart();

        protected abstract void FlowStop();

        protected abstract ActionRef Action(string actionName);
    }
}