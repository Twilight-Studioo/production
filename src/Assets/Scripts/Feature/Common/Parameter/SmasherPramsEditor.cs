using UnityEditor;

namespace Feature.Common.Parameter
{
    [CustomEditor(typeof(SmasherPrams))]
    public class SmasherPramsEditor : Editor
    {
        private SerializedProperty chargeAttackTime;
        private SerializedProperty chargeSpeed;
        private SerializedProperty chargeTime;
        private SerializedProperty chargeAttackDamage;
        private SerializedProperty chargeIntervalSec;
        private SerializedProperty upperHeight;
        private SerializedProperty upperDamage;
        private SerializedProperty upperIntervalSec;
        private SerializedProperty slapDistance;
        private SerializedProperty slapDamage;
        private SerializedProperty fallAttackDistance;
        private SerializedProperty fallSpeed;
        private SerializedProperty fallAttackDamage;
        private SerializedProperty fallAttackIntervalSec;
        private SerializedProperty debrisDamage;
        private SerializedProperty debrisSpeed;
        private SerializedProperty debrisAttackIntervalSec;
        private SerializedProperty mineIntervalSec;
        private SerializedProperty mineSpeedVertical;
        private SerializedProperty mineSpeedBeside;
        private SerializedProperty mineDamage;

        private void OnEnable()
        {
            chargeAttackTime = serializedObject.FindProperty("chargeAttackTime");
            chargeSpeed = serializedObject.FindProperty("chargeSpeed");
            chargeTime = serializedObject.FindProperty("chargeTime");
            chargeAttackDamage = serializedObject.FindProperty("chargeAttackDamage");
            chargeIntervalSec = serializedObject.FindProperty("chargeIntervalSec");
            upperHeight = serializedObject.FindProperty("upperHeight");
            upperDamage = serializedObject.FindProperty("upperDamage");
            upperIntervalSec = serializedObject.FindProperty("upperIntervalSec");
            slapDistance = serializedObject.FindProperty("slapDistance");
            slapDamage = serializedObject.FindProperty("slapDamage");
            fallAttackDistance = serializedObject.FindProperty("fallAttackDistance");
            fallSpeed = serializedObject.FindProperty("fallSpeed");
            fallAttackDamage = serializedObject.FindProperty("fallAttackDamage");
            fallAttackIntervalSec = serializedObject.FindProperty("fallAttackIntervalSec");
            debrisDamage = serializedObject.FindProperty("debrisDamage");
            debrisSpeed = serializedObject.FindProperty("debrisSpeed");
            debrisAttackIntervalSec = serializedObject.FindProperty("debrisAttackIntervalSec");
            mineIntervalSec = serializedObject.FindProperty("mineIntervalSec");
            mineSpeedVertical = serializedObject.FindProperty("mineSpeedVertical");
            mineSpeedBeside = serializedObject.FindProperty("mineSpeedBeside");
            mineDamage = serializedObject.FindProperty("mineDamage");
        }

        public override void OnInspectorGUI()
        {
            // 更新開始
            serializedObject.Update();

            EditorGUILayout.LabelField("Smasher Parameters", EditorStyles.boldLabel);

            // 突進系パラメータ
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("突進攻撃", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(chargeAttackTime);
            EditorGUILayout.PropertyField(chargeSpeed);
            EditorGUILayout.PropertyField(chargeTime);
            EditorGUILayout.PropertyField(chargeAttackDamage);
            EditorGUILayout.PropertyField(chargeIntervalSec);

            // アッパー系パラメータ
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("アッパー", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(upperHeight);
            EditorGUILayout.PropertyField(upperDamage);
            EditorGUILayout.PropertyField(upperIntervalSec);

            // 平手打ち
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("平手打ち", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(slapDistance);
            EditorGUILayout.PropertyField(slapDamage);

            // 落下攻撃
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("落下攻撃", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(fallAttackDistance);
            EditorGUILayout.PropertyField(fallSpeed);
            EditorGUILayout.PropertyField(fallAttackDamage);
            EditorGUILayout.PropertyField(fallAttackIntervalSec);

            // 瓦礫攻撃
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("瓦礫攻撃", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(debrisDamage);
            EditorGUILayout.PropertyField(debrisSpeed);
            EditorGUILayout.PropertyField(debrisAttackIntervalSec);

            // 地雷攻撃
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("地雷攻撃", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(mineIntervalSec);
            EditorGUILayout.PropertyField(mineSpeedVertical);
            EditorGUILayout.PropertyField(mineSpeedBeside);
            EditorGUILayout.PropertyField(mineDamage);

            // 更新終了
            serializedObject.ApplyModifiedProperties();
        }
    }
}