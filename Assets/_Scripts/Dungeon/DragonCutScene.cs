using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// 드래곤 보스 컷씬에 쓰일 스크립트
/// 등장시 포효 후 정상 FSM발동
/// </summary>
public class DragonCutScene : MonoBehaviour
{
    public Animator anim;
    public MonoBehaviour dragonFSM;

    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (dragonFSM != null) dragonFSM.enabled = false;

        StartCoroutine(StartScene());
    }

    IEnumerator StartScene()
    {
        anim.Play("Scream", 0, 0f);

        yield return new WaitUntil(() =>
            anim.GetCurrentAnimatorStateInfo(0).IsName("Scream")
        );

        yield return new WaitForSeconds(
            anim.GetCurrentAnimatorStateInfo(0).length
        );

        if (dragonFSM != null) dragonFSM.enabled = true;

        enabled = false;
    }
}
