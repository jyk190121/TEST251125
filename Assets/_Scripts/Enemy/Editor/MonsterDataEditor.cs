using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MonsterData))]
public class MonsterDataEditor : Editor
{
    SerializedProperty mobID, mobName, MobPrefab;

    SerializedProperty HP, Speed, Attack, Defense, CoolTime;
    SerializedProperty attackRange, detectionRange, minAttackRange;

    SerializedProperty Class, Type, Race;

    SerializedProperty NormalPatterns;
    SerializedProperty SpecialPatterns;
    SerializedProperty NormalpatternIDs;

    SerializedProperty windupTime, recoveryTime, poise, specialCoolTime;

    SerializedProperty aoeRange;

    SerializedProperty projectilePrefab;
    SerializedProperty laserPrefab;

    SerializedProperty moveRange, moveInterval;

    SerializedProperty patternCooldown, phaseTwoHpRate;
    SerializedProperty jumpAoeRadius, screamRange;
    SerializedProperty chargeDistance, chargeStoppingTime, shoutRange;

    SerializedProperty attackFX, hitFX, deathFX;
    SerializedProperty DropTable;

    bool foldStat = true;
    bool foldPattern = true;
    bool foldBoss = true;
    bool foldFX = true;
    bool foldDrop = true;

    void OnEnable()
    {
        mobID = serializedObject.FindProperty("mobID");
        mobName = serializedObject.FindProperty("mobName");
        MobPrefab = serializedObject.FindProperty("MobPrefab");

        HP = serializedObject.FindProperty("HP");
        Speed = serializedObject.FindProperty("Speed");
        Attack = serializedObject.FindProperty("Attack");
        Defense = serializedObject.FindProperty("Defense");
        CoolTime = serializedObject.FindProperty("CoolTime");
        attackRange = serializedObject.FindProperty("attackRange");
        detectionRange = serializedObject.FindProperty("detectionRange");
        minAttackRange = serializedObject.FindProperty("minAttackRange");

        Class = serializedObject.FindProperty("Class");
        Type = serializedObject.FindProperty("Type");
        Race = serializedObject.FindProperty("Race");

        NormalPatterns = serializedObject.FindProperty("NormalPatterns");
        SpecialPatterns = serializedObject.FindProperty("SpecialPatterns");
        NormalpatternIDs = serializedObject.FindProperty("NormalpatternIDs");

        windupTime = serializedObject.FindProperty("windupTime");
        recoveryTime = serializedObject.FindProperty("recoveryTime");
        poise = serializedObject.FindProperty("poise");
        specialCoolTime = serializedObject.FindProperty("specialCoolTime");

        aoeRange = serializedObject.FindProperty("aoeRange");

        projectilePrefab = serializedObject.FindProperty("projectilePrefab");
        laserPrefab = serializedObject.FindProperty("laserPrefab");

        moveRange = serializedObject.FindProperty("moveRange");
        moveInterval = serializedObject.FindProperty("moveInterval");

        patternCooldown = serializedObject.FindProperty("patternCooldown");
        phaseTwoHpRate = serializedObject.FindProperty("phaseTwoHpRate");
        jumpAoeRadius = serializedObject.FindProperty("jumpAoeRadius");
        screamRange = serializedObject.FindProperty("screamRange");
        chargeDistance = serializedObject.FindProperty("chargeDistance");
        chargeStoppingTime = serializedObject.FindProperty("chargeStoppingTime");
        shoutRange = serializedObject.FindProperty("shoutRange");

        attackFX = serializedObject.FindProperty("attackFX");
        hitFX = serializedObject.FindProperty("hitFX");
        deathFX = serializedObject.FindProperty("deathFX");

        DropTable = serializedObject.FindProperty("DropTable");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUIStyle bold = new GUIStyle(EditorStyles.boldLabel) { fontSize = 13 };

        EditorGUILayout.LabelField("📌 기본 정보", bold);
        EditorGUILayout.PropertyField(mobID);
        EditorGUILayout.PropertyField(mobName);
        EditorGUILayout.PropertyField(MobPrefab);
        EditorGUILayout.Space(6);

        foldStat = EditorGUILayout.Foldout(foldStat, "📌 기본 스탯", true);
        if (foldStat)
        {
            EditorGUILayout.PropertyField(HP);
            EditorGUILayout.PropertyField(Speed);
            EditorGUILayout.PropertyField(Attack);
            EditorGUILayout.PropertyField(Defense);
            EditorGUILayout.PropertyField(CoolTime);
            EditorGUILayout.PropertyField(specialCoolTime);

            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(attackRange);
            EditorGUILayout.PropertyField(detectionRange);
            EditorGUILayout.PropertyField(minAttackRange);
        }
        EditorGUILayout.Space(6);

        EditorGUILayout.LabelField("📌 몬스터 분류", bold);
        EditorGUILayout.PropertyField(Class);
        EditorGUILayout.PropertyField(Type);
        EditorGUILayout.PropertyField(Race);
        EditorGUILayout.Space(6);

        foldPattern = EditorGUILayout.Foldout(foldPattern, "📌 공격 패턴", true);
        if (foldPattern)
        {
            EditorGUILayout.LabelField("— 일반 공격 —", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(NormalPatterns, true);
            EditorGUILayout.PropertyField(NormalpatternIDs, true);

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("— 특수 공격 패턴 (ID 기준) —", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(SpecialPatterns, true);

            EditorGUI.indentLevel++;
            for (int i = 0; i < SpecialPatterns.arraySize; i++)
            {
                SerializedProperty elem = SpecialPatterns.GetArrayElementAtIndex(i);

                int rawValue = elem.intValue; // enum 실제 값 (100, 200, 300...)

                SpecialPattern sp = (SpecialPattern)rawValue;

                EditorGUILayout.LabelField(
                    $"• {sp}   (ID: {rawValue})",
                    EditorStyles.miniLabel
                );

            }
            EditorGUI.indentLevel--;

        }
        EditorGUILayout.Space(6);

        EditorGUILayout.LabelField("📌 공격 프리팹", bold);
        EditorGUILayout.PropertyField(projectilePrefab);
        EditorGUILayout.PropertyField(laserPrefab);
        EditorGUILayout.Space(6);

        if ((global::Race)Race.enumValueIndex == global::Race.Drone)
        {
            EditorGUILayout.LabelField("📌 드론 이동", bold);
            EditorGUILayout.PropertyField(moveRange);
            EditorGUILayout.PropertyField(moveInterval);
            EditorGUILayout.Space(6);
        }

        if ((global::Class)Class.enumValueIndex == global::Class.Boss)
        {
            foldBoss = EditorGUILayout.Foldout(foldBoss, "📌 보스 전용 파라미터", true);
            if (foldBoss)
            {
                EditorGUILayout.PropertyField(windupTime);
                EditorGUILayout.PropertyField(recoveryTime);
                EditorGUILayout.PropertyField(poise);
                

                EditorGUILayout.Space(4);
                EditorGUILayout.PropertyField(patternCooldown);
                EditorGUILayout.PropertyField(phaseTwoHpRate);

                EditorGUILayout.Space(4);
                EditorGUILayout.PropertyField(jumpAoeRadius);
                EditorGUILayout.PropertyField(screamRange);
                EditorGUILayout.PropertyField(chargeDistance);
                EditorGUILayout.PropertyField(chargeStoppingTime);
                EditorGUILayout.PropertyField(shoutRange);
            }
            EditorGUILayout.Space(6);
        }

        foldFX = EditorGUILayout.Foldout(foldFX, "📌 FX", true);
        if (foldFX)
        {
            EditorGUILayout.PropertyField(attackFX);
            EditorGUILayout.PropertyField(hitFX);
            EditorGUILayout.PropertyField(deathFX);
        }

        foldDrop = EditorGUILayout.Foldout(foldDrop, "📌 드랍 테이블", true);
        if (foldDrop)
            EditorGUILayout.PropertyField(DropTable, true);

        serializedObject.ApplyModifiedProperties();
    }
}





