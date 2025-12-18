using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    //만약 기본 데미지 데이터가 없다면 이걸로 추가
    [SerializeField]
    private float baseDamage = 0f;

    //이 데미지를 발생시킨 주체
    private GameObject damageOwner;
    PlayerControll PC;
    NormalMosterFSM NMF;
    //FSMTest FT;

    //중복 공격 방지
    //List 대신 HashSet인 이유
    //성능적인 측면에서 훨씬 유리함. List는 처음부터 끝까지 검색하지만, HashSet은 해시 함수로 메모리 주소를 즉시 계산-> 담긴거 하나하나 다 확인 안하고 필요한거만 찾음, 일정한 시간내에 검색
    //또한 HashSet은 중복된 항목을 저장하지 않는다! 맞은 놈은 또 다시 HashSet에 담기지 않는다
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    private void OnEnable()
    {
        damageOwner = transform.root.gameObject;

        // 플레이어 스탯을 가져와 초기 데미지 설정
        if (gameObject.layer == 7) // 플레이어 레이어
        {
            PlayerModel player = _MasterManager.Instance.DataManager.GetStat();
            baseDamage = player.ATT;
            Debug.Log($"{baseDamage} 무기 데미지 설정 완료");

            PC = damageOwner.GetComponent<PlayerControll>();
        }
        if(gameObject.layer == 11) //몬스터 레이어
        {
            MonsterData monster = damageOwner.GetComponent<NormalMosterFSM>().monsterData;
            baseDamage = monster.Attack;
            NMF = GetComponent<NormalMosterFSM>();
            //MonsterData monster = damageOwner.GetComponent<FSMTest>().monsterData;
            baseDamage = monster.Attack;
            NMF = GetComponent<NormalMosterFSM>();
            //FT = GetComponent<FSMTest>();
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

    public void ResetHitTargets()
    {
        hitTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {

        //플레이어 확인 및 공격중인지 확인
        if (gameObject.layer == 7)
        {
            PC = damageOwner.GetComponent<PlayerControll>();
            bool checkAttack = PC.OnAttack();
            if (!checkAttack)  return; 

        }

        if (gameObject.layer == 11)
        {
            NMF = GetComponent<NormalMosterFSM>();
            //FT = GetComponent<FSMTest>();   
            bool checkAttack = NMF.OnAttack();
            if (!checkAttack) return;
        }

        //이미 맞은놈이면 리턴
        if (hitTargets.Contains(other.gameObject)) return;
        //맞은게 나야? 쟤야?
        if (other.gameObject.layer == damageOwner.layer) return;
        
        //맞은 애
        IHitResponder responder = other.GetComponent<IHitResponder>();
        if(responder != null)
        {
            //hashSet에 맞은 놈 추가
            hitTargets.Add(other.gameObject);

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
