#region

using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#endregion

namespace Core.Utilities.Parameter
{
#if UNITY_EDITOR

    [CustomEditor(typeof(BaseParameter), true)]
    public class SimpleEditor : Editor
    {
        private readonly Dictionary<string, bool> foldouts = new();
        private readonly Dictionary<string, List<SerializedProperty>> groupedProperties = new();

        private void OnEnable()
        {
            // プロパティの初期化
            InitializeProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // グループごとにプロパティを表示
            foreach (var group in groupedProperties)
            {
                foldouts[group.Key] = EditorGUILayout.Foldout(foldouts[group.Key], group.Key, true);
                if (foldouts[group.Key])
                {
                    EditorGUI.indentLevel++;
                    foreach (var prop in group.Value)
                    {
                        DrawPropertyWithTooltip(prop);
                    }

                    EditorGUI.indentLevel--;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void InitializeProperties()
        {
            groupedProperties.Clear();
            foldouts.Clear();

            var iterator = serializedObject.GetIterator();
            iterator.NextVisible(true); // 初期化

            var currentHeader = "Default";
            var targetType = target.GetType();

            do
            {
                if (iterator.propertyPath == "m_Script")
                {
                    continue;
                }

                var field = targetType.GetField(iterator.name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field == null)
                {
                    continue;
                }

                // Header属性の取得
                var headerAttr = field.GetCustomAttribute<ToggleGroupAttribute>();
                if (headerAttr != null)
                {
                    currentHeader = headerAttr.GroupName;
                }

                if (!groupedProperties.ContainsKey(currentHeader))
                {
                    groupedProperties[currentHeader] = new();
                    foldouts[currentHeader] = true; // デフォルトで開く
                }

                groupedProperties[currentHeader].Add(iterator.Copy());
            } while (iterator.NextVisible(false));
        }

        private void DrawPropertyWithTooltip(SerializedProperty property)
        {
            var field = target.GetType().GetField(property.name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
            {
                return;
            }

            // Tooltip属性の取得
            var tooltipAttr = field.GetCustomAttribute<TooltipAttribute>();
            var label = tooltipAttr != null
                ? new(ObjectNames.NicifyVariableName(property.name), tooltipAttr.tooltip)
                : new GUIContent(property.displayName);

            EditorGUILayout.PropertyField(property, label, true);
        }
    }
#endif
}