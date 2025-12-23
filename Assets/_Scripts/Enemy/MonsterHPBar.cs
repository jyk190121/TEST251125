using UnityEngine;
using UnityEngine.UI;

public class MonsterHPBar : MonoBehaviour
{
    public Image fillImage;

    NormalMosterFSM monster;
    Transform cam;

    float maxHP;

    void Start()
    {
        cam = Camera.main.transform;
        monster = GetComponentInParent<NormalMosterFSM>();

        // 일반몹 기준 maxHP는 고정
        maxHP = monster.monsterData.HP;
    }

    void LateUpdate()
    {
        // 카메라 바라보기
        transform.forward = cam.forward;

        // FSM currentHP만 반영
        fillImage.fillAmount = monster.currentHP / maxHP;
    }

}

