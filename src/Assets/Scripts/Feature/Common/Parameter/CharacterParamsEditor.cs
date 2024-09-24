#if UNITY_EDITOR

#region

using UnityEditor;
using UnityEngine;


#endregion

namespace Feature.Common.Parameter
{
    [CustomEditor(typeof(CharacterParams))]
    public class CharacterParamsEditor : Editor
    {
        private SerializedProperty attackPower;
        private SerializedProperty canSwapDistance;
        private SerializedProperty enterSwapUseStamina;
        private SerializedProperty health;
        private SerializedProperty jumpPower;
        private SerializedProperty comboTimeWindow;
        private SerializedProperty comboAngleOffset;
        private SerializedProperty maxComboCount;
        private SerializedProperty attackCoolTime;

        private SerializedProperty maxHasStamina;
        private SerializedProperty recoveryResourceTimeMillis;
        private SerializedProperty recoveryTimeMillis;
        private SerializedProperty resourceRecoveryQuantity;
        private bool showStaminaParameters = true;

        private bool showSwapParameters = true;
        private SerializedProperty speed;

        private SerializedProperty swapContinueMaxMillis;
        private SerializedProperty swapContinueTimeScale;
        private SerializedProperty swapExecUseResource;
        private SerializedProperty swapModeStaminaUsage;
        private SerializedProperty swapModeStaminaUsageIntervalMillis;
        private SerializedProperty swapReturnCurve;
        private SerializedProperty swapReturnTimeMillis;
        
        private SerializedProperty useDaggerUseStamina;

        private SerializedProperty snapPower;

        private void OnEnable()
        {
            // 基本パラメータ
            health = serializedObject.FindProperty("health");
            speed = serializedObject.FindProperty("speed");
            jumpPower = serializedObject.FindProperty("jumpPower");
            attackPower = serializedObject.FindProperty("attackPower");
            comboTimeWindow =serializedObject.FindProperty("comboTimeWindow");
            comboAngleOffset = serializedObject.FindProperty("comboAngleOffset");
            maxComboCount = serializedObject.FindProperty("maxComboCount");
            attackCoolTime = serializedObject.FindProperty("attackCoolTime");

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
            useDaggerUseStamina = serializedObject.FindProperty("useDaggerUseStamina");
            snapPower = serializedObject.FindProperty("snapPower");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("基本パラメーター", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(health);
            EditorGUILayout.PropertyField(speed);
            EditorGUILayout.PropertyField(jumpPower);
            EditorGUILayout.PropertyField(attackPower);
            //EditorGUILayout.PropertyField(snapPower, new GUIContent("攻撃方向への移動量"));
            EditorGUILayout.PropertyField(comboTimeWindow,new GUIContent("〇秒以内で連続攻撃"));
            EditorGUILayout.PropertyField(comboAngleOffset,new GUIContent("連続攻撃時の角度変化"));
            EditorGUILayout.PropertyField(maxComboCount,new GUIContent("連続攻撃の最大回数"));
            EditorGUILayout.PropertyField(attackCoolTime,new GUIContent("攻撃のクールタイム"));

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
                EditorGUILayout.PropertyField(swapModeStaminaUsageIntervalMillis,
                    new GUIContent("スワップモード中何秒ごとにスタミナを消費するか"));
                EditorGUILayout.PropertyField(swapModeStaminaUsage, new GUIContent("スワップモード中に継続して消費するスタミナ"));
                EditorGUILayout.PropertyField(swapExecUseResource, new GUIContent("スワップした時に消費するスタミナ"));
                EditorGUILayout.PropertyField(recoveryTimeMillis, new GUIContent("スワップして何秒で回復し始めるか"));
                EditorGUILayout.PropertyField(resourceRecoveryQuantity, new GUIContent("1回の回復で増えるスタミナの量"));
                EditorGUILayout.PropertyField(recoveryResourceTimeMillis, new GUIContent("何ミリ秒ごとにリソースが回復するか"));
                EditorGUILayout.PropertyField(useDaggerUseStamina, new GUIContent("クナイを飛ばしたときのスタミナ消費"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif