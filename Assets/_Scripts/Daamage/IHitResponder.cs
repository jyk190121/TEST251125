using UnityEngine;

//데미지를 받는 객체에 붙이는 인터페이스
//데미지 받기, 사망 관련 함수 필수
public interface IHitResponder
{
    void TakeDamage(DamageData damage);

    void Die();
}
