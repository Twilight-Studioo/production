#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyParams))]
public class EnemyParamsEditor : Editor
{
    private SerializedProperty speed;
    private SerializedProperty damage;

    private void OnEnable()
    {
        speed = serializedObject.FindProperty("speed");
        damage = serializedObject.FindProperty("damage");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("敵のパラメーター", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(speed, new GUIContent("Speed"));
        EditorGUILayout.PropertyField(damage, new GUIContent("Damage"));

        serializedObject.ApplyModifiedProperties();
    }
}

#endif
