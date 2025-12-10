using UnityEditor;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    //만약 기본 데미지 데이터가 없다면 이걸로 추가
    [SerializeField]
    private float baseDamage = 0f;

    //이 데미지를 발생시킨 주체
    private GameObject damageOwner;


    private void OnEnable()
    {
        // 1. 공격 주체를 부모 오브젝트(플레이어 본체)로 설정
        // 이 무기 콜라이더의 Root 오브젝트가 플레이어 본체여야 합니다.
        damageOwner = transform.root.gameObject;

        // 2. 플레이어 스탯을 가져와 초기 데미지 설정

        if (gameObject.layer == 7) // 플레이어 레이어
        {
            PlayerModel player = _MasterManager.Instance.DataManager.GetStat();
            baseDamage += player.ATT;
        }
        if(gameObject.layer == 9) //몬스터 레이어
        {

        }
    }

    //데미지 출처를 설정하는 함수 -> 투사체용
    public void SetOwner(GameObject owner)
    {
        damageOwner = owner;
    }

    //데미지 설정 -> 투사체용
    public void SetDamage(float damage)
    {
        baseDamage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        IHitResponder responder = other.GetComponent<IHitResponder>();

        if(responder != null)
        {
            //때린 사람 -> 맞은 사람 방향
            Vector3 hitDir = (other.transform.position - damageOwner.transform.position).normalized;

            //DamageData 구조체 생성
            DamageData data = new DamageData(baseDamage, damageOwner, hitDir);

            //TakeDamage 함수에 전달 -> 데미지, 데미지 준 사람, 방향
            responder.TakeDamage(data);

            //투사체면?
            if(gameObject.GetComponent<Projectile>() != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
