#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Feature.Common
{
    [CustomEditor(typeof(CharacterParams))]
    public class CharacterParamsEditor : Editor
    {
        private SerializedProperty health;
        private SerializedProperty speed;
        private SerializedProperty jumpPower;
        private SerializedProperty attackPower;

        private SerializedProperty swapContinueMaxMillis;
        private SerializedProperty swapContinueTimeScale;
        private SerializedProperty canSwapDistance;
        private SerializedProperty swapReturnTimeMillis;
        private SerializedProperty swapReturnCurve;

        private SerializedProperty maxHasStamina;
        private SerializedProperty enterSwapUseStamina;
        private SerializedProperty swapModeStaminaUsageIntervalMillis;
        private SerializedProperty swapModeStaminaUsage;
        private SerializedProperty swapExecUseResource;
        private SerializedProperty resourceRecoveryQuantity;
        private SerializedProperty recoveryResourceTimeMillis;
        private SerializedProperty recoveryTimeMillis;

        private bool showSwapParameters = true;
        private bool showStaminaParameters = true;

        private void OnEnable()
        {
            // 基本パラメータ
            health = serializedObject.FindProperty("health");
            speed = serializedObject.FindProperty("speed");
            jumpPower = serializedObject.FindProperty("jumpPower");
            attackPower = serializedObject.FindProperty("attackPower");

            // スワップ関連
            swapContinueMaxMillis = serializedObject.FindProperty("swapContinueMaxMillis");
            swapContinueTimeScale = serializedObject.FindProperty("swapContinueTimeScale");
            canSwapDistance = serializedObject.FindProperty("canSwapDistance");
            swapReturnTimeMillis = serializedObject.FindProperty("swapReturnTimeMillis");
            swapReturnCurve = serializedObject.FindProperty("swapReturnCurve");

            // スタミナ関連
            maxHasStamina = serializedObject.FindProperty("maxHasStamina");
            enterSwapUseStamina = serializedObject.FindProperty("enterSwapUseStamina");
            swapModeStaminaUsageIntervalMillis = serializedObject.FindProperty("swapModeStaminaUsageIntervalMillis");
            swapModeStaminaUsage = serializedObject.FindProperty("swapModeStaminaUsage");
            swapExecUseResource = serializedObject.FindProperty("swapExecUseStamina");
            recoveryTimeMillis = serializedObject.FindProperty("recoveryTimeMillis");
            resourceRecoveryQuantity = serializedObject.FindProperty("resourceRecoveryQuantity");
            recoveryResourceTimeMillis = serializedObject.FindProperty("recoveryStaminaTimeMillis");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("基本パラメーター", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(health);
            EditorGUILayout.PropertyField(speed);
            EditorGUILayout.PropertyField(jumpPower);
            EditorGUILayout.PropertyField(attackPower);

            EditorGUILayout.Space();
            showSwapParameters = EditorGUILayout.Foldout(showSwapParameters, "スワップパラメーター");
            if (showSwapParameters)
            {
                EditorGUILayout.PropertyField(swapContinueMaxMillis, new GUIContent("スワップの最大継続時間(Milli)"));
                EditorGUILayout.PropertyField(swapContinueTimeScale, new GUIContent("スワップ中のTimeScale"));
                EditorGUILayout.PropertyField(canSwapDistance, new GUIContent("スワップ可能な最大距離"));
                EditorGUILayout.PropertyField(swapReturnTimeMillis, new GUIContent("スワップ後何秒で元のタイムスケールに戻るか"));
                EditorGUILayout.PropertyField(swapReturnCurve, new GUIContent("元のタイムスケールへの戻り方"));
            }

            EditorGUILayout.Space();
            showStaminaParameters = EditorGUILayout.Foldout(showStaminaParameters, "スタミナパラメーター");
            if (showStaminaParameters)
            {
                EditorGUILayout.PropertyField(maxHasStamina, new GUIContent("スタミナを持つ最大値"));
                EditorGUILayout.PropertyField(enterSwapUseStamina, new GUIContent("スワップモードに入ったときのスタミナ消費量"));
                EditorGUILayout.PropertyField(swapModeStaminaUsageIntervalMillis, new GUIContent("スワップモード中何秒ごとにスタミナを消費するか"));
                EditorGUILayout.PropertyField(swapModeStaminaUsage, new GUIContent("スワップモード中に継続して消費するスタミナ"));
                EditorGUILayout.PropertyField(swapExecUseResource, new GUIContent("スワップした時に消費するスタミナ"));
                EditorGUILayout.PropertyField(recoveryTimeMillis, new GUIContent("スワップして何秒で回復し始めるか"));
                EditorGUILayout.PropertyField(resourceRecoveryQuantity, new GUIContent("1回の回復で増えるスタミナの量"));
                EditorGUILayout.PropertyField(recoveryResourceTimeMillis, new GUIContent("何ミリ秒ごとにリソースが回復するか"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
