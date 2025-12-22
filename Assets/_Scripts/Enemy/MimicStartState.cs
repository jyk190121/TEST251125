using UnityEngine;
using System.Collections;

/// <summary>
/// 미믹 전용 시작 연출 스크립트
/// - 상자 상태 → 변신
/// - 히트박스 On / Off
/// - 일반 몬스터 FSM 활성화 후 자신 비활성화
/// </summary>
public class MimicStartState : MonoBehaviour
{
    [Header("References")]
    public Animator anim;                       // 미믹 애니메이터
    public MonoBehaviour normalMonsterFSM;      // 일반 몬스터 FSM (처음엔 OFF)
    public GameObject revealHitBox;             // 변신 공격 히트박스

    [Header("Trigger Settings")]
    public float triggerRange = 2.5f;           // 플레이어 접근 거리
    public float hitBoxDelay = 0.3f;             // 애니 시작 후 히트박스 켜질 시간
    public float hitBoxDuration = 0.2f;          // 히트박스 유지 시간

    Transform target;
    bool isRevealed;

    void Start()
    {
        // FSM 비활성화
        if (normalMonsterFSM != null)
            normalMonsterFSM.enabled = false;

        // 히트박스 기본 OFF
        if (revealHitBox != null)
            revealHitBox.SetActive(false);

        // Animator 자동 참조
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (isRevealed) return;

        // 플레이어 탐색
        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p == null) return;
            target = p.transform;
        }

        // 거리 체크
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= triggerRange)
        {
            StartCoroutine(RevealProcess());
        }
    }

    IEnumerator RevealProcess()
    {
        isRevealed = true;

        // 변신 애니메이션
        anim.SetTrigger("WakeUp");

        // 애니 반영 대기
        yield return null;
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        float animLength = info.length;

        // 히트박스 ON 타이밍
        yield return new WaitForSeconds(hitBoxDelay);

        if (revealHitBox != null)
            revealHitBox.SetActive(true);

        // 히트박스 유지
        yield return new WaitForSeconds(hitBoxDuration);

        if (revealHitBox != null)
            revealHitBox.SetActive(false);

        // 애니 끝까지 대기
        float remain = animLength - (hitBoxDelay + hitBoxDuration);
        if (remain > 0f)
            yield return new WaitForSeconds(remain);

        // 일반 몬스터 FSM 활성화
        if (normalMonsterFSM != null)
            normalMonsterFSM.enabled = true;

        // 이 스크립트 역할 종료
        this.enabled = false;
    }
}

