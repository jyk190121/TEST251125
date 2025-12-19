using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MonsterData))]
public class MonsterDataEditor : Editor
{
    // 기본 정보
    SerializedProperty mobID, mobName, MobPrefab;

    // 스탯
    SerializedProperty HP, Speed, Attack, Defense, CoolTime;
    SerializedProperty attackRange, detectionRange, minAttackRange;

    // 분류
    SerializedProperty Class, Type, Race;

    // 패턴
    SerializedProperty NormalPatterns;
    SerializedProperty SpecialPatterns;
    SerializedProperty NormalpatternIDs;

    // 공통 패턴 파라미터
    SerializedProperty windupTime, recoveryTime, poise, specialCoolTime;

    // 근접 / 원거리
    SerializedProperty aoeRange;
    SerializedProperty projectilePrefab;
    SerializedProperty laserPrefab;

    // 드론
    SerializedProperty moveRange, moveInterval;

    // 보스 전용
    SerializedProperty patternCooldown, phaseTwoHpRate;
    SerializedProperty jumpAoeRadius, roarRange;
    SerializedProperty chargeDistance, chargeStoppingTime;

    // FX
    SerializedProperty attackFX, hitFX, deathFX;

    // 드랍
    SerializedProperty DropTable;

    // Foldout
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
        roarRange = serializedObject.FindProperty("roarRange");
        chargeDistance = serializedObject.FindProperty("chargeDistance");
        chargeStoppingTime = serializedObject.FindProperty("chargeStoppingTime");

        attackFX = serializedObject.FindProperty("attackFX");
        hitFX = serializedObject.FindProperty("hitFX");
        deathFX = serializedObject.FindProperty("deathFX");

        DropTable = serializedObject.FindProperty("DropTable");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUIStyle bold = new GUIStyle(EditorStyles.boldLabel) { fontSize = 13 };

        // 기본 정보
        EditorGUILayout.LabelField("📌 기본 정보", bold);
        EditorGUILayout.PropertyField(mobID);
        EditorGUILayout.PropertyField(mobName);
        EditorGUILayout.PropertyField(MobPrefab);
        EditorGUILayout.Space(6);

        // 스탯
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

        // 분류
        EditorGUILayout.LabelField("📌 몬스터 분류", bold);
        EditorGUILayout.PropertyField(Class);
        EditorGUILayout.PropertyField(Type);
        EditorGUILayout.PropertyField(Race);
        EditorGUILayout.Space(6);

        // 패턴
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
                var elem = SpecialPatterns.GetArrayElementAtIndex(i);
                int rawValue = elem.intValue;
                SpecialPattern sp = (SpecialPattern)rawValue;

                EditorGUILayout.LabelField(
                    $"• {sp}   (ID: {rawValue})",
                    EditorStyles.miniLabel
                );
            }
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space(6);

        // 공격 프리팹
        EditorGUILayout.LabelField("📌 공격 프리팹", bold);
        EditorGUILayout.PropertyField(projectilePrefab);
        EditorGUILayout.PropertyField(laserPrefab);
        EditorGUILayout.Space(6);

        // 드론 전용
        if ((Race.enumValueIndex == (int)global::Race.Drone))
        {
            EditorGUILayout.LabelField("📌 드론 이동", bold);
            EditorGUILayout.PropertyField(moveRange);
            EditorGUILayout.PropertyField(moveInterval);
            EditorGUILayout.Space(6);
        }

        // 보스 전용
        if ((Class.enumValueIndex == (int)global::Class.Boss))
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
                EditorGUILayout.PropertyField(roarRange);
                EditorGUILayout.PropertyField(chargeDistance);
                EditorGUILayout.PropertyField(chargeStoppingTime);
            }
            EditorGUILayout.Space(6);
        }

        // FX
        foldFX = EditorGUILayout.Foldout(foldFX, "📌 FX", true);
        if (foldFX)
        {
            EditorGUILayout.PropertyField(attackFX);
            EditorGUILayout.PropertyField(hitFX);
            EditorGUILayout.PropertyField(deathFX);
        }

        // 드랍 테이블
        foldDrop = EditorGUILayout.Foldout(foldDrop, "📌 드랍 테이블", true);
        if (foldDrop)
        {
            EditorGUILayout.HelpBox(
                "Drop Chance는 0 ~ 1 범위입니다.\n" +
                "1 = 100%, 0.5 = 50%, 0 = 드랍 안 됨\n\n" +
                "확률이 있다면 minCount는 1 이상을 권장합니다.",
                MessageType.Info
            );

            EditorGUILayout.PropertyField(DropTable, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}






