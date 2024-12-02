using UnityEditor;

namespace Feature.Common.Parameter
{
    [CustomEditor(typeof(SmasherPrams))]
    public class SmasherPramsEditor : Editor
    {
        private SerializedProperty health;
        private SerializedProperty chargeAttackTime;
        private SerializedProperty chargeSpeed;
        private SerializedProperty chargeTime;
        private SerializedProperty chargeAttackDamage;
        private SerializedProperty chargeIntervalSec;
        private SerializedProperty upperHeight;
        private SerializedProperty upperDamage;
        private SerializedProperty upperOccurrenceTime;
        private SerializedProperty upperIntervalSec;
        private SerializedProperty jumpOccurrenceTime;
        private SerializedProperty jumpIntervalSec;
        private SerializedProperty slapDistance;
        private SerializedProperty slapDamage;
        private SerializedProperty slapOccurrenceTime;
        private SerializedProperty slapIntervalSec;
        private SerializedProperty fallAttackDistance;
        private SerializedProperty fallSpeed;
        private SerializedProperty fallAttackDamage;
        private SerializedProperty fallAttackOccurrenceTime;
        private SerializedProperty fallAttackIntervalSec;
        private SerializedProperty debrisDamage;
        private SerializedProperty debrisSpeed;
        private SerializedProperty debrisAttackOccurrenceTime;
        private SerializedProperty debrisAttackIntervalSec;
        private SerializedProperty mineOccurrenceTime;
        private SerializedProperty mineIntervalSec;
        private SerializedProperty mineSpeedVertical;
        private SerializedProperty mineSpeedBeside;
        private SerializedProperty mineDamage;
        private SerializedProperty kickTriggerTime;
        private SerializedProperty kickDamage;
        
        private void OnEnable()
        {
            health = serializedObject.FindProperty("health");
            chargeAttackTime = serializedObject.FindProperty("chargeAttackTime");
            chargeSpeed = serializedObject.FindProperty("chargeSpeed");
            chargeTime = serializedObject.FindProperty("chargeTime");
            chargeAttackDamage = serializedObject.FindProperty("chargeAttackDamage");
            chargeIntervalSec = serializedObject.FindProperty("chargeIntervalSec");
            upperHeight = serializedObject.FindProperty("upperHeight");
            upperDamage = serializedObject.FindProperty("upperDamage");
            upperOccurrenceTime = serializedObject.FindProperty("upperOccurrenceTime");
            upperIntervalSec = serializedObject.FindProperty("upperIntervalSec");
            jumpOccurrenceTime = serializedObject.FindProperty("jumpOccurrenceTime");
            jumpIntervalSec = serializedObject.FindProperty("jumpIntervalSec");
            slapDistance = serializedObject.FindProperty("slapDistance");
            slapDamage = serializedObject.FindProperty("slapDamage");
            slapOccurrenceTime = serializedObject.FindProperty("slapOccurrenceTime");
            slapIntervalSec = serializedObject.FindProperty("slapIntervalSec");
            fallAttackDistance = serializedObject.FindProperty("fallAttackDistance");
            fallSpeed = serializedObject.FindProperty("fallSpeed");
            fallAttackDamage = serializedObject.FindProperty("fallAttackDamage");
            fallAttackOccurrenceTime = serializedObject.FindProperty("fallAttackOccurrenceTime");
            fallAttackIntervalSec = serializedObject.FindProperty("fallAttackIntervalSec");
            debrisDamage = serializedObject.FindProperty("debrisDamage");
            debrisSpeed = serializedObject.FindProperty("debrisSpeed");
            debrisAttackOccurrenceTime = serializedObject.FindProperty("debrisAttackOccurrenceTime");
            debrisAttackIntervalSec = serializedObject.FindProperty("debrisAttackIntervalSec");
            mineOccurrenceTime = serializedObject.FindProperty("mineOccurrenceTime");
            mineIntervalSec = serializedObject.FindProperty("mineIntervalSec");
            mineSpeedVertical = serializedObject.FindProperty("mineSpeedVertical");
            mineSpeedBeside = serializedObject.FindProperty("mineSpeedBeside");
            mineDamage = serializedObject.FindProperty("mineDamage");
            kickTriggerTime = serializedObject.FindProperty("kickTriggerTime");
            kickDamage = serializedObject.FindProperty("kickDamage");
        }

        public override void OnInspectorGUI()
        {
            // 更新開始
            serializedObject.Update();

            EditorGUILayout.LabelField("Smasher Parameters", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(health);

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
            EditorGUILayout.PropertyField(upperOccurrenceTime);
            EditorGUILayout.PropertyField(upperIntervalSec);
            
            // ジャンプ系パラメータ
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("ジャンプ", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(jumpOccurrenceTime);
            EditorGUILayout.PropertyField(jumpIntervalSec);

            // 平手打ち
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("平手打ち", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(slapDistance);
            EditorGUILayout.PropertyField(slapDamage);
            EditorGUILayout.PropertyField(slapOccurrenceTime);
            EditorGUILayout.PropertyField(slapIntervalSec);
            
            // 落下攻撃
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("落下攻撃", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(fallAttackDistance);
            EditorGUILayout.PropertyField(fallSpeed);
            EditorGUILayout.PropertyField(fallAttackDamage);
            EditorGUILayout.PropertyField(fallAttackOccurrenceTime);
            EditorGUILayout.PropertyField(fallAttackIntervalSec);

            // 瓦礫攻撃
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("瓦礫攻撃", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(debrisDamage);
            EditorGUILayout.PropertyField(debrisSpeed);
            EditorGUILayout.PropertyField(debrisAttackOccurrenceTime);
            EditorGUILayout.PropertyField(debrisAttackIntervalSec);

            // 地雷攻撃
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("地雷攻撃", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(mineOccurrenceTime);
            EditorGUILayout.PropertyField(mineIntervalSec);
            EditorGUILayout.PropertyField(mineSpeedVertical);
            EditorGUILayout.PropertyField(mineSpeedBeside);
            EditorGUILayout.PropertyField(mineDamage);
            
            //被ダメ
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("被ダメ", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(kickTriggerTime);
            EditorGUILayout.PropertyField(kickDamage);

            // 更新終了
            serializedObject.ApplyModifiedProperties();
        }
    }
}