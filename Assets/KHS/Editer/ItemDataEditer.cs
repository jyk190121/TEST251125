using UnityEditor;
using UnityEngine;

//
// 이 스크립트는 ItemData 전용 커스텀 인스펙터를 만든다.
//
// 목적:
// - ItemType(Material / Equipment / Potion)에 따라
//   필요한 필드만 인스펙터에 표시하여 가독성을 높임
// - 잘못된 데이터 입력을 방지하고 편의성 향상
//

[CustomEditor(typeof(ItemData))]     // ItemData에만 적용되는 에디터
public class ItemDataEditor : Editor
{
    // --------------------------
    // SerializedProperty 변수들
    // (각 필드를 인스펙터에 안전하게 표시/변경하기 위한 유니티 방식)
    // --------------------------

    SerializedProperty itemID;
    SerializedProperty itemName;
    SerializedProperty icon;
    SerializedProperty description;

    SerializedProperty type;

    SerializedProperty isStackable;
    SerializedProperty maxStack;

    SerializedProperty buyPrice;
    SerializedProperty sellPrice;

    // Material 전용
    SerializedProperty rarity;
    SerializedProperty dropMonsters;

    // Equipment 전용
    SerializedProperty equipmentType;
    SerializedProperty equipmentSlot;
    SerializedProperty tier;

    SerializedProperty attack;
    SerializedProperty defense;
    SerializedProperty speed;
    SerializedProperty hpPlus;

    SerializedProperty craftRecipe;
    SerializedProperty upgradeRecipe;

    // Potion 전용
    SerializedProperty healAmount;
    SerializedProperty coolDown;


    // -------------------------------------------------
    // OnEnable()
    // - Unity가 에디터를 초기화할 때 호출
    // - ItemData의 각 변수를 SerializedProperty로 연결
    // -------------------------------------------------
    void OnEnable()
    {
        itemID = serializedObject.FindProperty("itemID");
        itemName = serializedObject.FindProperty("itemName");
        icon = serializedObject.FindProperty("icon");
        description = serializedObject.FindProperty("description");

        type = serializedObject.FindProperty("type");

        isStackable = serializedObject.FindProperty("isStackable");
        maxStack = serializedObject.FindProperty("maxStack");

        buyPrice = serializedObject.FindProperty("buyPrice");
        sellPrice = serializedObject.FindProperty("sellPrice");

        rarity = serializedObject.FindProperty("rarity");
        dropMonsters = serializedObject.FindProperty("dropMonsters");

        equipmentType = serializedObject.FindProperty("equipmentType");
        equipmentSlot = serializedObject.FindProperty("equipmentSlot");
        tier = serializedObject.FindProperty("tier");

        attack = serializedObject.FindProperty("attack");
        defense = serializedObject.FindProperty("defense");
        speed = serializedObject.FindProperty("speed");
        hpPlus = serializedObject.FindProperty("hpPlus");

        craftRecipe = serializedObject.FindProperty("craftRecipe");
        upgradeRecipe = serializedObject.FindProperty("upgradeRecipe");

        healAmount = serializedObject.FindProperty("healAmount");
        coolDown = serializedObject.FindProperty("coolDown");
    }


    // -------------------------------------------------
    // OnInspectorGUI()
    // - 실제 인스펙터 UI를 그리는 함수
    // - 기본 정보는 항상 표시하고
    // - ItemType에 따라 필요한 필드만 선택적으로 표시
    // -------------------------------------------------
    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // 최신 데이터 동기화

        // ============ 공통 정보 출력 ============

        EditorGUILayout.LabelField("== 기본 정보 ==", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(itemID);
        EditorGUILayout.PropertyField(itemName);
        EditorGUILayout.PropertyField(icon);
        EditorGUILayout.PropertyField(description);

        EditorGUILayout.Space(10);

        // 아이템 타입
        EditorGUILayout.PropertyField(type);

        // 스택 여부
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("== 스택 설정 ==", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(isStackable);
        if (isStackable.boolValue)
            EditorGUILayout.PropertyField(maxStack);

        // 가격 설정
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("== 가격 설정 ==", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(buyPrice);
        EditorGUILayout.PropertyField(sellPrice);

        EditorGUILayout.Space(15);


        // -------------------------------------------------
        // ItemType에 따라 필요한 항목만 출력
        // -------------------------------------------------

        ItemType t = (ItemType)type.enumValueIndex;

        switch (t)
        {
            case ItemType.Material:
                DrawMaterial();
                break;

            case ItemType.Equipment:
                DrawEquipment();
                break;

            case ItemType.Potion:
                DrawPotion();
                break;
        }

        serializedObject.ApplyModifiedProperties(); // 변경사항 저장
    }


    // ===========================
    // 재료 타입(Material) UI 출력
    // ===========================
    void DrawMaterial()
    {
        EditorGUILayout.LabelField("== 재료 설정 ==", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(rarity);
        EditorGUILayout.PropertyField(dropMonsters, true); // true: 리스트 확장 가능
    }


    // ===========================
    // 장비 타입(Equipment) UI 출력
    // ===========================
    void DrawEquipment()
    {
        EditorGUILayout.LabelField("== 장비 설정 ==", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(equipmentType);
        EditorGUILayout.PropertyField(equipmentSlot);
        EditorGUILayout.PropertyField(tier);

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("== 추가 능력치 ==", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(attack);
        EditorGUILayout.PropertyField(defense);
        EditorGUILayout.PropertyField(speed);
        EditorGUILayout.PropertyField(hpPlus);

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("== 제작 및 업그레이드 재료 ==", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(craftRecipe, true);
        EditorGUILayout.PropertyField(upgradeRecipe, true);
    }


    // ===========================
    // 포션 타입(Potion) UI 출력
    // ===========================
    void DrawPotion()
    {
        EditorGUILayout.LabelField("== 포션 설정 ==", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(healAmount);
        EditorGUILayout.PropertyField(coolDown);
    }
}
