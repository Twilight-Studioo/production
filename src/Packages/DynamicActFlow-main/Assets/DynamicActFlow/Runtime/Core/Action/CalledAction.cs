#region

using System.Collections;

#endregion

namespace DynamicActFlow.Runtime.Core.Action
{
    /// <summary>
    ///     CalledAction is an abstract base class for creating actions that can be executed within a flow.
    ///     It inherits from the ActionBase class.
    /// </summary>
    public abstract class CalledAction : ActionBase
    {
        protected abstract IEnumerator Call();

        protected override IEnumerator OnAction()
        {
            yield return Call();
        }
    }
}