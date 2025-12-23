using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    public Image fillImage;

    DragonFSM dragon;
    Transform cam;

    float maxHP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main.transform;
        dragon = GetComponentInParent<DragonFSM>();

        maxHP = dragon.dragonData.HP;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 카메라 바라보기
        transform.forward = cam.forward;

        // FSM currentHP만 반영
        fillImage.fillAmount = dragon.currentHP / maxHP;
    }
}
