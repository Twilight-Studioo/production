#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Core.Utilities.Parameter
{

    [CustomPropertyDrawer(typeof(Vector3))]
    public class Vector3ToggleDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // スイッチの表示
            var toggleRect = new Rect(position.x, position.y, 50, EditorGUIUtility.singleLineHeight);
            var onRelative = GUI.Button(toggleRect, "import");

            var relativeValue = property.vector3Value;
            if (onRelative)
            {
                var referencePosition = GetReferencePosition(property);
                relativeValue = referencePosition;
            }
            var fieldRect = new Rect(position.x + 50, position.y, position.width - 50, EditorGUIUtility.singleLineHeight);
            relativeValue = EditorGUI.Vector3Field(fieldRect, property.name, relativeValue);
            property.vector3Value = relativeValue;

            EditorGUI.EndProperty();
        }
        
        
        /// <summary>
        /// SerializedProperty から関連するオブジェクトの Transform.position を取得
        /// </summary>
        private Vector3 GetReferencePosition(SerializedProperty property)
        {
            // 対象オブジェクトの取得
            var targetObject = property.serializedObject.targetObject;

            if (targetObject is MonoBehaviour monoBehaviour)
            {
                // MonoBehaviour の場合、Transform.position を基準座標に設定
                return monoBehaviour.transform.position;
            }
            else if (targetObject is ScriptableObject)
            {
                // ScriptableObject の場合、任意の基準値を返す（必要に応じて設定）
                return Vector3.zero;
            }

            // それ以外はデフォルト値として Vector3.zero を返す
            return Vector3.zero;
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // トグルスイッチ + Vector3 フィールドの高さを計算
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
#endif
