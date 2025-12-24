using UnityEngine;

public class Effect : MonoBehaviour
{
   
    [Header("Settings")]
    public GameObject effectPrefab; 
    public Transform effectPoint;   

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.J))
        {
            Attack();
        }
    }

    void Attack()
    {
       
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
            Debug.LogWarning("Master, effectPrefab이나 effectPoint가 비어있습니다!");
        }
    }
}