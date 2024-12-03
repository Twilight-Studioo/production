#region

using System.Collections;
using UnityEngine;

#endregion

namespace DynamicActFlow.Runtime.Core.Action
{
    /// <summary>
    ///     Base class for actions that need to be updated in FixedUpdate().
    /// </summary>
    /// <remarks>
    ///     This class is meant to be subclassed and the following methods must be implemented:
    ///     - Start(): Called once at the beginning of the action.
    ///     - FixedUpdate(): Called in every FixedUpdate() until the action ends.
    ///     - CheckIfEnd(): Check if the action has ended.
    /// </remarks>
    public abstract class FixedUpdatedAction : ActionBase
    {
        protected virtual void Start()
        {
        }

        protected abstract void FixedUpdate();

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
                FixedUpdate();
                yield return new WaitForFixedUpdate();
            }

            OnEnd();
        }
    }
}