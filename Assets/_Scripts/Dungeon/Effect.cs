using UnityEngine;

public class Effect : MonoBehaviour
{
    // [Header]는 인스펙터 창에서 변수들을 보기 좋게 그룹화해주는 역할을 합니다.
    [Header("Settings")]
    public GameObject effectPrefab; // 생성할 이펙트의 원본(Prefab)입니다.
    public Transform effectPoint;   // Master님이 말씀하신 '빈 오브젝트'의 위치 정보입니다.

    void Update()
    {
        // GetButtonDown은 버튼을 누른 그 순간 한 번만 true를 반환합니다.
        // "Fire1"은 기본적으로 마우스 왼쪽 클릭이나 Ctrl 키로 설정되어 있습니다.
        if (Input.GetKeyUp(KeyCode.J))
        {
            Attack();
        }
    }

    void Attack()
    {
        // 팩트체크: Instantiate는 오브젝트를 복제하여 씬에 생성하는 함수입니다.
        if (effectPrefab != null && effectPoint != null)
        {
            // Instantiate(복제할 대상, 생성될 위치, 생성될 회전값);
            // effectPoint.position: 빈 오브젝트의 현재 세계 좌표
            // effectPoint.rotation: 빈 오브젝트가 바라보고 있는 방향(회전)
            GameObject effect = Instantiate(effectPrefab, effectPoint.position, effectPoint.rotation);

            // 생성된 이펙트가 영원히 남아있으면 메모리 부하가 생기므로 2초 뒤에 삭제합니다.
            // Destroy(삭제할 오브젝트, 지연 시간);
            Destroy(effect, 2.0f);
        }
        else
        {
            // 변수가 연결되지 않았을 경우 에러를 방지하기 위한 디버그 메시지입니다.
            Debug.LogWarning("Master, effectPrefab이나 effectPoint가 비어있습니다!");
        }
    }
}