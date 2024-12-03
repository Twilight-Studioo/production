#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DynamicActFlow.Runtime.Core.Action;

#endregion

namespace DynamicActFlow.Runtime.Core
{
    /// <summary>
    ///     Factory class used to create instances of ActionBase subclasses based on a given tag.
    /// </summary>
    internal static class FlowFactory
    {
        private static readonly Dictionary<string, Type> ActionMap = new();

        private static readonly Dictionary<string, Type> TriggerMap = new();

        static FlowFactory()
        {
            RegisterActions();
        }

        private static void RegisterActions()
        {
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(ActionBase)) && t.GetCustomAttribute<ActionTagAttribute>() != null)
                .ToList()
                .ForEach(type =>
                {
                    var tagAttribute = type.GetCustomAttribute<ActionTagAttribute>();
                    ActionMap[tagAttribute.Tag] = type;
                });

            AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(TriggerBase)) && t.GetCustomAttribute<TriggerTagAttribute>() != null)
                .ToList()
                .ForEach(type =>
                {
                    var tagAttribute = type.GetCustomAttribute<TriggerTagAttribute>();
                    TriggerMap[tagAttribute.Tag] = type;
                });
        }

        /// <summary>
        ///     Creates an instance of ActionBase subclass based on the given tag.
        /// </summary>
        /// <param name="tag">The tag used to identify the action.</param>
        /// <returns>An instance of ActionBase subclass.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no action is found with the given tag.</exception>
        public static ActionBase CreateAction(string tag)
        {
            if (!ActionMap.TryGetValue(tag, out var value))
            {
                throw new InvalidOperationException("No action found with tag: " + tag);
            }

            var action = (ActionBase)Activator.CreateInstance(value);
            action.SetDefaultProperty();
            action.OnCreated();
            return action;
        }

        /// <summary>
        ///     Creates an instance of TriggerBase subclass based on the given tag.
        /// </summary>
        /// <param name="tag">The tag used to identify the trigger.</param>
        /// <returns>An instance of TriggerBase subclass.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no trigger is found with the given tag.</exception>
        public static TriggerBase CreateTrigger(string tag)
        {
            if (!TriggerMap.TryGetValue(tag, out var value))
            {
                throw new InvalidOperationException("No trigger found with tag: " + tag);
            }

            var trigger = (TriggerBase)Activator.CreateInstance(value);
            trigger.OnCreated();
            return trigger;
        }

        private static PropertyInfo[] GetAllProperties<TB>(this TB action) where TB : class =>
            action.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );

        private static void SetProperty<TA, TB, T>(this TB action, string propertyName, T value)
            where TA : ParameterBaseAttribute where TB : class
        {
            try
            {
                var propertyInfo = action
                    .GetAllProperties()
                    .FirstOrDefault(prop => prop.GetCustomAttributes<TA>(false)
                        .Any(attr => attr.Tag == propertyName));

                if (propertyInfo == null || !propertyInfo.CanWrite)
                {
                    return;
                }

                var parameter = propertyInfo.GetCustomAttributes(typeof(TA), false)
                    .Cast<TA>()
                    .FirstOrDefault(attr => attr.Tag == propertyName);

                // 値の型が正しいかチェックし、プロパティに値を設定
                if (propertyInfo.PropertyType == value.GetType())
                {
                    propertyInfo.SetValue(action, value);
                    return;
                }

                // if can set attr.Value to propertyInfo
                if (parameter == null || parameter.DefaultValue.GetType() != propertyInfo.PropertyType)
                {
                    throw new InvalidOperationException(
                        $"Type mismatch for property '{propertyName}'. Expected type {propertyInfo.PropertyType}, but got type {value.GetType()}.");
                }

                propertyInfo.SetValue(action, parameter.DefaultValue);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error setting property {propertyName} on {action.GetType().Name}: {e.Message}");
                throw;
            }
        }

        public static void SetProperty<T>(this ActionBase action, string propertyName, T value)
        {
            action.SetProperty<ActionParameterAttribute, ActionBase, T>(propertyName, value);
        }

        public static void SetProperty<T>(this TriggerBase trigger, string propertyName, T value)
        {
            trigger.SetProperty<TriggerParameterAttribute, TriggerBase, T>(propertyName, value);
        }

        private static void SetDefaultProperty(this ActionBase action)
        {
            // インスタンスのすべてのプロパティをループ処理
            foreach (var property in action.GetAllProperties())
            {
                // ActionParameterAttributeを取得
                var attribute = property.GetCustomAttribute<ActionParameterAttribute>();
                if (attribute is not { DefaultValue: not null, Tag: not null, })
                {
                    continue;
                }

                // プロパティの型がDefaultValueの型と一致するか確認し、一致すれば値を設定
                if (property.PropertyType.IsInstanceOfType(attribute.DefaultValue))
                {
                    property.SetValue(action, attribute.DefaultValue);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Type mismatch for property '{property.Name}'. Expected type {property.PropertyType}, but got type {attribute.DefaultValue.GetType()}.");
                }
            }
        }
    }
}