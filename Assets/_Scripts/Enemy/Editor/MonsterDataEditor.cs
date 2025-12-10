using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MonsterData))]
public class MonsterDataEditor : Editor
{
    // SerializedProperty 변수
    SerializedProperty mobID;
    SerializedProperty mobName;
    SerializedProperty prefab;

    SerializedProperty HP;
    SerializedProperty Speed;
    SerializedProperty Attack;
    SerializedProperty Defense;
    SerializedProperty CoolTime;
    SerializedProperty attackRange;
    SerializedProperty detectionRange;
    SerializedProperty minAttackRange;

    SerializedProperty Class;
    SerializedProperty Type;
    SerializedProperty Race;

    SerializedProperty Pattern;

    SerializedProperty windupTime;
    SerializedProperty recoveryTime;
    SerializedProperty poise;

    SerializedProperty attackRadius;
    SerializedProperty attackAngle;
    SerializedProperty aoeRange;
    SerializedProperty aoeDamageMultiplier;

    SerializedProperty projectileSpeed;
    SerializedProperty projectileLifeTime;
    SerializedProperty projectileArc;
    SerializedProperty projectileCount;
    SerializedProperty predictionFactor;
    SerializedProperty shotInterval;
    SerializedProperty burstCount;

    SerializedProperty moveRange;
    SerializedProperty moveInterval;
    SerializedProperty laserWarningTime;
    SerializedProperty laserFireTime;
    SerializedProperty laserCooldown;
    SerializedProperty laserDamage;
    SerializedProperty laserLength;
    SerializedProperty laserDirections;

    SerializedProperty patternCooldown;
    SerializedProperty phaseTwoHpRate;
    SerializedProperty jumpAoeRadius;
    SerializedProperty screamRange;
    SerializedProperty chargeDistance;
    SerializedProperty chargeStoppingTime;
    SerializedProperty shoutRange;

    SerializedProperty attackFX;
    SerializedProperty hitFX;
    SerializedProperty deathFX;

    SerializedProperty DropTable;

    // Foldout 상태
    bool foldStats = true;
    bool foldPattern = true;
    bool foldMelee = true;
    bool foldProjectile = true;
    bool foldDrone = true;
    bool foldBoss = true;
    bool foldFX = true;
    bool foldDrop = true;

    void OnEnable()
    {
        // 공통 정보
        mobID = serializedObject.FindProperty("mobID");
        mobName = serializedObject.FindProperty("mobName");
        prefab = serializedObject.FindProperty("MobPrefab");

        // 스탯
        HP = serializedObject.FindProperty("HP");
        Speed = serializedObject.FindProperty("Speed");
        Attack = serializedObject.FindProperty("Attack");
        Defense = serializedObject.FindProperty("Defense");
        CoolTime = serializedObject.FindProperty("CoolTime");
        attackRange = serializedObject.FindProperty("attackRange");
        detectionRange = serializedObject.FindProperty("detectionRange");
        minAttackRange = serializedObject.FindProperty("minAttackRange");

        // 타입 관련
        Class = serializedObject.FindProperty("Class");
        Type = serializedObject.FindProperty("Type");
        Race = serializedObject.FindProperty("Race");

        // 패턴
        Pattern = serializedObject.FindProperty("Pattern");

        // 공통 패턴 파라미터
        windupTime = serializedObject.FindProperty("windupTime");
        recoveryTime = serializedObject.FindProperty("recoveryTime");
        poise = serializedObject.FindProperty("poise");

        // 근접
        attackRadius = serializedObject.FindProperty("attackRadius");
        attackAngle = serializedObject.FindProperty("attackAngle");
        aoeRange = serializedObject.FindProperty("aoeRange");
        aoeDamageMultiplier = serializedObject.FindProperty("aoeDamageMultiplier");

        // 투사체 몹
        projectileSpeed = serializedObject.FindProperty("projectileSpeed");
        projectileLifeTime = serializedObject.FindProperty("projectileLifeTime");
        projectileArc = serializedObject.FindProperty("projectileArc");
        projectileCount = serializedObject.FindProperty("projectileCount");
        predictionFactor = serializedObject.FindProperty("predictionFactor");
        shotInterval = serializedObject.FindProperty("shotInterval");
        burstCount = serializedObject.FindProperty("burstCount");

        // 드론
        moveRange = serializedObject.FindProperty("moveRange");
        moveInterval = serializedObject.FindProperty("moveInterval");
        laserWarningTime = serializedObject.FindProperty("laserWarningTime");
        laserFireTime = serializedObject.FindProperty("laserFireTime");
        laserCooldown = serializedObject.FindProperty("laserCooldown");
        laserDamage = serializedObject.FindProperty("laserDamage");
        laserLength = serializedObject.FindProperty("laserLength");
        laserDirections = serializedObject.FindProperty("laserDirections");

        // 보스
        patternCooldown = serializedObject.FindProperty("patternCooldown");
        phaseTwoHpRate = serializedObject.FindProperty("phaseTwoHpRate");
        jumpAoeRadius = serializedObject.FindProperty("jumpAoeRadius");
        screamRange = serializedObject.FindProperty("screamRange");
        chargeDistance = serializedObject.FindProperty("chargeDistance");
        chargeStoppingTime = serializedObject.FindProperty("chargeStoppingTime");
        shoutRange = serializedObject.FindProperty("shoutRange");

        // FX
        attackFX = serializedObject.FindProperty("attackFX");
        hitFX = serializedObject.FindProperty("hitFX");
        deathFX = serializedObject.FindProperty("deathFX");

        // 드랍
        DropTable = serializedObject.FindProperty("DropTable");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUIStyle bold = new GUIStyle(EditorStyles.boldLabel);
        bold.fontSize = 13;

        /*───────────────────────────────────────────────*
         * 기본 정보
         *───────────────────────────────────────────────*/
        EditorGUILayout.LabelField("📌 기본 정보", bold);
        EditorGUILayout.PropertyField(mobID);
        EditorGUILayout.PropertyField(mobName);
        EditorGUILayout.PropertyField(prefab);
        EditorGUILayout.Space(8);


        /*───────────────────────────────────────────────*
         * 스탯
         *───────────────────────────────────────────────*/
        foldStats = EditorGUILayout.Foldout(foldStats, "📌 기본 스탯", true);
        if (foldStats)
        {
            EditorGUILayout.PropertyField(HP);
            EditorGUILayout.PropertyField(Speed);
            EditorGUILayout.PropertyField(Attack);
            EditorGUILayout.PropertyField(Defense);
            EditorGUILayout.PropertyField(CoolTime);

            EditorGUILayout.Space(3);
            EditorGUILayout.PropertyField(attackRange);
            EditorGUILayout.PropertyField(detectionRange);

            if ((Type.enumValueIndex == (int)global::Type.Range))
            {
                EditorGUILayout.PropertyField(minAttackRange);
            }
        }
        EditorGUILayout.Space(8);

        /*───────────────────────────────────────────────*
         * 타입 / 종족
         *───────────────────────────────────────────────*/
        EditorGUILayout.LabelField("📌 몬스터 분류", bold);
        EditorGUILayout.PropertyField(Class);
        EditorGUILayout.PropertyField(Type);
        EditorGUILayout.PropertyField(Race);
        EditorGUILayout.Space(8);

        /*───────────────────────────────────────────────*
         * 패턴 배열
         *───────────────────────────────────────────────*/
        foldPattern = EditorGUILayout.Foldout(foldPattern, "📌 행동 패턴", true);
        if (foldPattern)
        {
            EditorGUILayout.PropertyField(Pattern, true);
        }
        EditorGUILayout.Space(8);


        /*───────────────────────────────────────────────*
         * 공통 패턴 파라미터
         *───────────────────────────────────────────────*/
        EditorGUILayout.LabelField("📌 패턴 공통 파라미터", bold);
        EditorGUILayout.PropertyField(windupTime);
        EditorGUILayout.PropertyField(recoveryTime);
        EditorGUILayout.PropertyField(poise);
        EditorGUILayout.Space(8);


        /*───────────────────────────────────────────────*
         * 근접 공격 (Type: Melee)
         *───────────────────────────────────────────────*/
        if ((global::Type)Type.enumValueIndex == global::Type.Melee ||
            (Race.enumValueIndex == (int)global::Race.Golem) ||
            (Race.enumValueIndex == (int)global::Race.Mimic))
        {
            foldMelee = EditorGUILayout.Foldout(foldMelee, "📌 근접 공격 설정", true);
            if (foldMelee)
            {
                EditorGUILayout.PropertyField(attackRadius);
                EditorGUILayout.PropertyField(attackAngle);
                EditorGUILayout.PropertyField(aoeRange);
                EditorGUILayout.PropertyField(aoeDamageMultiplier);
            }
            EditorGUILayout.Space(8);
        }


        /*───────────────────────────────────────────────*
         * 비홀더(투사체 몹)
         *───────────────────────────────────────────────*/
        if ((Race.enumValueIndex == (int)global::Race.Beholder))
        {
            foldProjectile = EditorGUILayout.Foldout(foldProjectile, "📌 투사체 설정 (비홀더)", true);
            if (foldProjectile)
            {
                EditorGUILayout.PropertyField(projectileSpeed);
                EditorGUILayout.PropertyField(projectileLifeTime);
                EditorGUILayout.PropertyField(projectileArc);
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(projectileCount);
                EditorGUILayout.PropertyField(shotInterval);
                EditorGUILayout.PropertyField(burstCount);
                EditorGUILayout.PropertyField(predictionFactor);
            }
            EditorGUILayout.Space(8);
        }


        /*───────────────────────────────────────────────*
         * 드론 레이저 몹
         *───────────────────────────────────────────────*/
        if (Race.enumValueIndex == (int)global::Race.Drone)
        {
            foldDrone = EditorGUILayout.Foldout(foldDrone, "📌 드론 레이저 설정", true);
            if (foldDrone)
            {
                EditorGUILayout.PropertyField(moveRange);
                EditorGUILayout.PropertyField(moveInterval);
                EditorGUILayout.Space(4);

                EditorGUILayout.PropertyField(laserWarningTime);
                EditorGUILayout.PropertyField(laserFireTime);
                EditorGUILayout.PropertyField(laserCooldown);
                EditorGUILayout.Space(4);

                EditorGUILayout.PropertyField(laserDamage);
                EditorGUILayout.PropertyField(laserLength);
                EditorGUILayout.PropertyField(laserDirections);
            }
            EditorGUILayout.Space(8);
        }


        /*───────────────────────────────────────────────*
         * 보스
         *───────────────────────────────────────────────*/
        if (Class.enumValueIndex == (int)global::Class.Boss)
        {
            foldBoss = EditorGUILayout.Foldout(foldBoss, "📌 보스 설정", true);
            if (foldBoss)
            {
                EditorGUILayout.PropertyField(patternCooldown);
                EditorGUILayout.PropertyField(phaseTwoHpRate);

                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("드래곤 패턴", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(jumpAoeRadius);
                EditorGUILayout.PropertyField(screamRange);

                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField("코뿔소 패턴", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(chargeDistance);
                EditorGUILayout.PropertyField(chargeStoppingTime);
                EditorGUILayout.PropertyField(shoutRange);
            }
            EditorGUILayout.Space(8);
        }


        /*───────────────────────────────────────────────*
         * FX 섹션
         *───────────────────────────────────────────────*/
        foldFX = EditorGUILayout.Foldout(foldFX, "📌 FX 설정", true);
        if (foldFX)
        {
            EditorGUILayout.PropertyField(attackFX);
            EditorGUILayout.PropertyField(hitFX);
            EditorGUILayout.PropertyField(deathFX);
        }
        EditorGUILayout.Space(8);


        /*───────────────────────────────────────────────*
         * 드랍 테이블 배열
         *───────────────────────────────────────────────*/
        foldDrop = EditorGUILayout.Foldout(foldDrop, "📌 드랍 테이블", true);
        if (foldDrop)
        {
            EditorGUILayout.PropertyField(DropTable, true);
        }
        EditorGUILayout.Space(8);


        serializedObject.ApplyModifiedProperties();
    }
}


