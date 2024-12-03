#region

using System;
using DynamicActFlow.Runtime.Core.Flow;

#endregion

namespace DynamicActFlow.Runtime.Core.Action
{
    /// <summary>
    ///     Helper class that provides methods for creating actions and triggers in a flow.
    /// </summary>
    public static class Actions
    {
        /// <summary>
        ///     Creates a new instance of ActionRef for the specified tag.
        /// </summary>
        /// <param name="builder">The flow builder.</param>
        /// <param name="tag">The tag used to identify the action.</param>
        /// <returns>An instance of ActionRef.</returns>
        internal static ActionRef Action(this IFlowBuilder builder, string tag) =>
            builder.GetActionRef(FlowFactory.CreateAction(tag));

        /// <summary>
        ///     Sets a parameter for an action.
        /// </summary>
        /// <param name="action">The action reference.</param>
        /// <param name="key">The parameter key.</param>
        /// <param name="value">The parameter value.</param>
        /// <typeparam name="T">The type of the action reference.</typeparam>
        /// <returns>The action reference with the parameter set.</returns>
        public static T Param<T>(this T action, string key, object value) where T : Ref
        {
            action.SetParam(key, value);
            return action;
        }

        /// <summary>
        ///     Ends the conditional if statement.
        /// </summary>
        /// <param name="action">The action reference.</param>
        /// <param name="trigger">The trigger to check.</param>
        /// <returns>The updated action reference.</returns>
        public static ActionRef IfEnd(this ActionRef action, TriggerBase trigger)
        {
            if (action.Type() == ActionType.Called)
            {
                throw new NotSupportedParameterException("IfEnd is not supported for Called actions");
            }

            action.SetTrigger(trigger);
            return action;
        }

        /// <summary>
        ///     <p>Ends the conditional if statement.</p>
        /// </summary>
        /// <param name="action">The action reference.</param>
        /// <param name="triggers">The triggers to check.</param>
        /// <returns>The updated action reference.</returns>
        /// <exception cref="NotSupportedParameterException"></exception>
        [Obsolete("Use IfEnd instead")]
        public static ActionRef IfEnd(this ActionRef action, TriggerBase[] triggers)
        {
            if (action.Type() == ActionType.Called)
            {
                throw new NotSupportedParameterException("IfEnd is not supported for Called actions");
            }

            action.SetTriggers(triggers);
            return action;
        }

        /// <summary>
        ///     Ends the conditional if statements.
        /// </summary>
        /// <param name="action">The action reference.</param>
        /// <param name="trigger1">The trigger to check.</param>
        /// <param name="trigger2">The trigger to check.</param>
        /// <returns>The updated action reference.</returns>
        public static ActionRef IfEnd(this ActionRef action, TriggerBase trigger1, TriggerBase trigger2)
        {
            if (action.Type() == ActionType.Called)
            {
                throw new NotSupportedParameterException("IfEnd is not supported for Called actions");
            }

            action.SetTriggers(new[] { trigger1, trigger2, });
            return action;
        }

        /// <summary>
        ///     Ends the conditional if statements.
        /// </summary>
        /// <param name="action">The action reference.</param>
        /// <param name="trigger1">The trigger to check.</param>
        /// <param name="trigger2">The trigger to check.</param>
        /// <param name="trigger3">The trigger to check.</param>
        /// <returns>The updated action reference.</returns>
        public static ActionRef IfEnd(this ActionRef action, TriggerBase trigger1, TriggerBase trigger2,
            TriggerBase trigger3)
        {
            if (action.Type() == ActionType.Called)
            {
                throw new NotSupportedParameterException("IfEnd is not supported for Called actions");
            }

            action.SetTriggers(new[] { trigger1, trigger2, trigger3, });
            return action;
        }


        /// <summary>
        ///     Ends the conditional if statements.
        /// </summary>
        /// <param name="action">The action reference.</param>
        /// <param name="trigger1">The trigger to check.</param>
        /// <param name="trigger2">The trigger to check.</param>
        /// <param name="trigger3">The trigger to check.</param>
        /// <param name="trigger4">The trigger to check.</param>
        /// <returns>The updated action reference.</returns>
        public static ActionRef IfEnd(this ActionRef action, TriggerBase trigger1, TriggerBase trigger2,
            TriggerBase trigger3, TriggerBase trigger4)
        {
            if (action.Type() == ActionType.Called)
            {
                throw new NotSupportedParameterException("IfEnd is not supported for Called actions");
            }

            action.SetTriggers(new[] { trigger1, trigger2, trigger3, trigger4, });
            return action;
        }

        public static ActionRef Timeout(this ActionRef action, float timeoutSec)
        {
            if (action.Type() == ActionType.Called)
            {
                throw new NotSupportedParameterException("Timeout is not supported for Called actions");
            }

            action.SetTimeout(timeoutSec);
            return action;
        }


        /// <summary>
        ///     Creates a new instance of TriggerRef for the specified tag.
        /// </summary>
        /// <param name="builder">The flow builder.</param>
        /// <param name="tag">The tag used to identify the trigger.</param>
        /// <returns>An instance of TriggerRef.</returns>
        public static TriggerRef Trigger(this IFlowBuilder builder, string tag) =>
            builder.GetTriggerRef(FlowFactory.CreateTrigger(tag));
    }
}