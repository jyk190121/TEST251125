using UnityEngine;

//데미지 전달에 필요한 정보를 담는 스크립트
//단지 구조만 형성되어있음
public struct DamageData
{
    //피해량
    public readonly float damageAmount;

    //피해 출처, 피해를 준 오브젝트
    public readonly GameObject damageSource;

    //충돌 방향(이후 넉백 같은거 추가하면 사용 가능
    public readonly Vector3 hitDirection;

    public DamageData(float amount, GameObject source, Vector3 direction)
    {
        this.damageAmount = amount;
        this.damageSource = source;
        this.hitDirection = direction;
    }
}
