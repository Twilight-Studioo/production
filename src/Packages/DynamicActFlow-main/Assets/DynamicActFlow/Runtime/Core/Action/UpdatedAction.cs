#region

using System.Collections;

#endregion

namespace DynamicActFlow.Runtime.Core.Action
{
    /// <summary>
    ///     Represents an abstract base class for updated actions.
    /// </summary>
    public abstract class UpdatedAction : ActionBase
    {
        protected virtual void Start()
        {
        }

        protected abstract void Update();

        protected abstract bool CheckIfEnd();

        protected virtual void OnEnd()
        {
        }


        protected override IEnumerator OnAction()
        {
            Start();
            StartTrigger();
            while (!CheckIfEnd() && !IfEndWithTrigger())
            {
                Update();
                yield return null;
            }

            OnEnd();
        }
    }
}