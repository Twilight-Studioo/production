using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Core.Utilities.Parameter
{
#if UNITY_EDITOR
    [CustomEditor(typeof(BaseParameter), true)]
    public class SimpleEditor : Editor
    {
        private Dictionary<string, List<SerializedProperty>> groupedProperties = new Dictionary<string, List<SerializedProperty>>();
        private Dictionary<string, bool> foldouts = new Dictionary<string, bool>();

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
                foldouts[group.Key] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[group.Key], group.Key);
                if (foldouts[group.Key])
                {
                    foreach (var prop in group.Value)
                    {
                        DrawPropertyWithTooltip(prop);
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void InitializeProperties()
        {
            groupedProperties.Clear();
            foldouts.Clear();

            var iterator = serializedObject.GetIterator();
            var enterChildren = true;
            var currentHeader = "Default";
            var targetType = target.GetType();

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (iterator.propertyPath == "m_Script")
                    continue;

                var field = targetType.GetField(iterator.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field == null)
                    continue;

                // Header属性の取得
                var headerAttr = field.GetCustomAttribute<ToggleGroupAttribute>();
                if (headerAttr != null)
                {
                    currentHeader = headerAttr.GroupName;
                }

                if (!groupedProperties.ContainsKey(currentHeader))
                {
                    groupedProperties[currentHeader] = new List<SerializedProperty>();
                    foldouts[currentHeader] = true; // デフォルトで開く
                }

                groupedProperties[currentHeader].Add(iterator.Copy());
            }
        }

        private void DrawPropertyWithTooltip(SerializedProperty property)
        {
            var field = target.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
                return;

            // Tooltip属性の取得
            var tooltipAttr = field.GetCustomAttribute<TooltipAttribute>();
            var label = tooltipAttr != null ? tooltipAttr.tooltip : property.displayName;

            EditorGUILayout.PropertyField(property, new GUIContent(label));
        }
    }
#endif
}