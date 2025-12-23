using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

//PlayerMove에서 입력받은 값에 따라 실행되는 함수를 정리한 스크립트
public class PlayerControll : MonoBehaviour, IHitResponder
{
    CharacterController CC;
    PlayerModel model;
    PlayerAnimController PAC;
    BattleRecord BR;

    Item weapon;
    public GameObject arrowPrefab;
    public GameObject arrowSpawnPoint;

    bool isMove = false;
    bool isAttacking = false;
    bool isCharge = false;
    //활쏘는 동안 가만히
    bool isBow = false;
    //창 돌진
    bool isSpearCharge = false;
    bool isSpearChargeAttack = false;
    float SpearChargedTime = 0f;
    Vector3 SpearDir;
    float speartime = 0f;

    int comboTime = 0;


    //콤보시스템
    private int comboInputBuffer = 0;
    private const int MaxComboInput = 3; // 최대 2번까지 미리 입력 허용 (총 3타 콤보이므로)
    

    //구르기
    //1. 구르기 거리
    //2. 구르기 했나?
    float rollDistance = 2f;
    bool isRolling = false;
    float rollTimer = 1.2f;

    //공격 속도
    float attackTimer;

    //공격 회전 관련 변수
    private Quaternion originalRotation; // 공격 직전의 원래 회전값
    private bool needsRotationRevert = false; // 공격 후 회전 복구가 필요한지 여부

    // 창과 활의 임시 회전값 (예시: 활은 오른쪽 30도, 창은 왼쪽 40도)
    private const float BowAttackAngle = 90f;
    private const float SpearAttackAngle = 40f;
    private const float SwordShieldAngle = 40f;

    //임시 무기별 공격 시간 -> 공격때 넣을거임
    float sword = 1.5f;
    float shield = 0.2f;
    float bow = 1.5f;
    float spear = 0.69f;

    //공격용 무기의 DamageDealer를 받는 리스트
    private List<DamageDealer> meleeWeaponDealers = new List<DamageDealer>();

    //무기별 스타일 분리용
    int weaponnumber = 5;

    // 넉백 관련
    Vector3 knockbackVelocity;
    float knockbackTimer;

    public float knockbackDuration = 0.25f;
    public float knockbackPower = 6f;
    public float knockbackDamping = 12f;

    private void OnEnable()
    {
        DataManager.OnEquipmentChanged += RefreshWeapon;
    }

    void Start()
    {
        PAC = GetComponentInChildren<PlayerAnimController>();
        CC = GetComponent<CharacterController>();
        model = _MasterManager.Instance.DataManager.GetStat();
        Debug.Log("모델" + model);
        meleeWeaponDealers.AddRange(GetComponentsInChildren<DamageDealer>(true));
        Debug.Log(model.ATT);
        BR = FindAnyObjectByType<BattleRecord>();
        weapon = _MasterManager.Instance.DataManager.GetWeapon();
        weaponType();
    }

    public void Update()
    {
        // 넉백 타이머
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;

            knockbackVelocity = Vector3.Lerp(
                knockbackVelocity,
                Vector3.zero,
                knockbackDamping * Time.deltaTime
            );

            CC.Move(knockbackVelocity * Time.deltaTime);
        }


        if (weaponnumber == 0) return;

        //창찌르기 차지공격
        if (isSpearCharge && SpearChargedTime < 2f)
        {
            SpearChargedTime += Time.deltaTime;
        }
        if (!isSpearCharge && SpearChargedTime >= 0f)
        {
            SpearChargedTime -= Time.deltaTime;
            //임의로 이동 막기 위해서
            isSpearChargeAttack = true;
            CC.Move(SpearDir * Time.deltaTime * 8f);
            PAC.HandleSpearChargeAttack(isSpearChargeAttack);
        }
        

        if(SpearChargedTime < 0f && speartime < 0.8)
        {
            speartime += Time.deltaTime;
        }
        else if(speartime > 0.8)
        {
            speartime = 0f;
            isSpearChargeAttack = false;
        }


        if (isRolling)
        {
            // 1. 구르기 타이머 감소
            rollTimer -= Time.deltaTime;

            // 2. 구르기 이동 처리 (CharacterController 사용)
            // rolldir은 Roll() 호출 시 transform.forward로 설정되었으므로, 현재 방향으로 이동합니다.
            // Roll 시에는 model.moveSpeed보다 빠른 Roll 전용 속도를 사용합니다.
            float rollSpeed = 7f; // 구르기 속도 (임의의 값, 필요에 따라 조정)
            Vector3 rollMovement = transform.forward * rollSpeed;
            CC.Move(rollMovement * Time.deltaTime);

            // 3. 구르기 종료 처리
            if (rollTimer <= 0f)
            {
                rollTimer = 1.2f; // 타이머 초기화 (쿨다운 또는 다음 구르기 대기 시간으로 사용할 경우)
                isRolling = false;
                gameObject.layer = 7;
                // 구르기 애니메이션이 끝나면 Idle 상태로 돌아가도록 애니메이션 컨트롤러에 알려줄 수 있습니다.
                // PAC.EndRollingAnim(); 같은 함수를 호출할 수 있습니다.
                Debug.Log("구르기 끝");
            }
        }

        if (isAttacking)
        {
            if (!isCharge)
            {
                attackTimer -= Time.deltaTime;
            }

            if (attackTimer <= 0f)
            {
                isAttacking = false;
                isBow = false;

                comboInputBuffer = 0;

                if (comboTime > 0)
                {
                    comboTime = 0;
                }
            }
        }


        //공격 후 정면 복구
        if (needsRotationRevert && !isAttacking && !isMove && !isCharge && !isSpearChargeAttack)
        {
            PAC.HandleSpearChargeAttack(false);
            transform.rotation = originalRotation;
            needsRotationRevert = false;
        }

        if (comboInputBuffer != 0)
        {
            foreach (var dealer in meleeWeaponDealers)
            {
                dealer.ResetHitTargets();
            }
        }
        ComboInputBuffer();
    }


    //장비 장착시 호출! player의 스탯값 변경!
    public void RefreshStat()
    {
        model = _MasterManager.Instance.DataManager.GetStat();
    }
    public void RefreshWeapon()
    {
        weapon = _MasterManager.Instance.DataManager.GetWeapon();
        weaponType();
    }
    //장착중인 무기에 따라 스타일 변경
    void weaponType()
    {
        if (weapon == null) return;

        if (weapon.itemID == 2001 || weapon.itemID == 2002)
        {
            weaponnumber = 1;
        }
        else if(weapon.itemID == 2003 || weapon.itemID == 2004)
        {
            weaponnumber = 2;
        }
        else if(weapon.itemID == 2005 || weapon.itemID == 2006)
        {
            weaponnumber = 3;
        }
    }

    public void Idle()
    {
        if (!isMove) return;
        isMove = false;
        PAC.HandleMovementAnim(isMove);
    }
    public void Move(Vector3 dir)
    {
        if (isRolling || isCharge || isBow || isSpearChargeAttack || isAttacking) return;
        SpearDir = dir;
        isMove = true;
        PAC.HandleMovementAnim(isMove);
        // 수평 이동
        Vector3 move = dir.normalized * model.moveSpeed;
        CC.Move(move * Time.deltaTime);
        
        // 회전
        if (!isAttacking)
        {
            if (dir != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = targetRot;
            }
        }
    }

    public void Attack()
    {
        if (isRolling) return;
        //if (weapon == null)
        //{
        //    Debug.Log("무기 없음!");
        //    return;
        //}
        Quaternion targetRotation = transform.rotation; // 부모의 현재 회전을 기준으로 시작
        bool shouldRotate = false;
        isMove = false;

        if (weaponnumber == 1) // 검 공격
        {
            if (isAttacking == false)
            {
                comboTime = 1; // 1타 시작
                comboInputBuffer = 0;
                foreach (var dealer in meleeWeaponDealers)
                {
                    dealer.ResetHitTargets();
                }
            }
            else
            {
                if (comboInputBuffer < MaxComboInput -1)
                {
                    comboInputBuffer++;
                }
                return;
            }
        }
        else // 기타 무기 (창, 활 등 콤보가 없는 무기)
        {
            // 콤보가 없는 무기는 무조건 1타로 고정
            if (isAttacking) return; // 공격 중이면 추가 입력 무시
            comboTime = 1;

            //원본 회전값
            originalRotation = transform.rotation;
            if (weaponnumber == 2)
            {
                targetRotation = originalRotation * Quaternion.Euler(0, SpearAttackAngle, 0);
                shouldRotate = true;
                needsRotationRevert = true;
            }
            if(weaponnumber == 3)
            {
                targetRotation = originalRotation * Quaternion.Euler(0, BowAttackAngle, 0);
                shouldRotate = true;
                needsRotationRevert = true;
                isBow = true;
            }
            foreach (var dealer in meleeWeaponDealers)
            {
                dealer.ResetHitTargets();
            }
        }

        // 🌟 1. 부모 오브젝트의 회전을 적용합니다. 🌟
        if (shouldRotate)
        {
            // 💡 Move() 함수와 마찬가지로 즉시 회전하도록 transform.rotation을 직접 설정합니다.
            transform.rotation = targetRotation;
        }

        isAttacking = true;

        //isAttacking 해제 시간
        switch (weaponnumber)
        {
            case 1:
                attackTimer = sword;
                break;
            case 2:
                attackTimer = spear;
                break;
            case 3:
                attackTimer = bow;
                break;
        }

        PAC.HandleAttack(weaponnumber, comboTime);
        if (weaponnumber == 3)
        {
            fireArrow();
        }
    }


    //보조공격
    public void SubCharge()
    {
        if (isRolling || isAttacking) return;
        //if (weapon == null) return;
        PAC.HandleMovementAnim(false);

        isAttacking = true;
        isCharge = true;
        isMove = false;
        
        if (weaponnumber != 1)
        {
            foreach (var dealer in meleeWeaponDealers)
            {
                dealer.ResetHitTargets();
            }
        }

        Quaternion targetRotation = transform.rotation; // 부모의 현재 회전을 기준으로 시작
        bool shouldRotate = false;

        originalRotation = transform.rotation;

        if (weaponnumber == 1)
        {
            targetRotation = originalRotation * Quaternion.Euler(0, SwordShieldAngle, 0);
            shouldRotate = true;
            needsRotationRevert = true;
        }
        if (weaponnumber == 2)
        {
            targetRotation = originalRotation * Quaternion.Euler(0, SpearAttackAngle, 0);
            shouldRotate = true;
            needsRotationRevert = true;
            isSpearCharge = true;
        }
        if (weaponnumber == 3)
        {
            targetRotation = originalRotation * Quaternion.Euler(0, BowAttackAngle, 0);
            shouldRotate = true;
            needsRotationRevert = true;
            isBow = true;
        }
        // 부모 오브젝트의 회전을 적용
        if (shouldRotate)
        {
            //즉시 회전하도록 transform.rotation을 직접 설정
            transform.rotation = targetRotation;
        }

        switch (weaponnumber)
        {
            case 1:
                attackTimer = shield;
                break;
            case 2:
                attackTimer = spear + 0.15f;
                break;
            case 3:
                attackTimer = bow + 0.2f;
                break;
        }


        PAC.HandleCharge(isCharge, weaponnumber);
    }

    public void SubAttack()
    {
        Debug.Log("서브어택");
        if(!isCharge) return;
        isCharge = false;
        isSpearCharge = false;

        PAC.HandleCharge(isCharge, weaponnumber);
        if(weaponnumber == 3)
        {
            fireArrow();
            isBow = true;
        }
    }

    public void Roll(Vector3 rolldir)
    {
        if (isRolling) return;
        PAC.HandleRollingAnim();
        isRolling = true;
        gameObject.layer = 30;

        Debug.Log("구르기 시작");
    }

    public void TakeDamage(DamageData damage)
    {
        _MasterManager.Instance.DataManager.ChangeHP((int)damage.damageAmount);

        ApplyKnockback(damage.hitDirection);

        Debug.Log("체력: " + model.HP);
        if(model.HP <= 0)
        {
            model.HP = 0;
            Die();
        }
    }

    void ApplyKnockback(Vector3 dir)
    {
        if (isRolling) return; // 구르기 중엔 무시 (선택)

        dir.y = 0; // 바닥 고정
        knockbackVelocity = dir.normalized * knockbackPower;
        knockbackTimer = knockbackDuration;
    }


    public void Die()
    {
        _MasterManager.Instance.DataManager.ChangeReturn(true);
    }

    //칼 콤보 공격 처리
    void ComboInputBuffer()
    {
        if (comboInputBuffer > 0)
        {
            comboInputBuffer--;           // 저장된 값 소모

            comboTime++; // 다음 콤보 카운트 증가

            PAC.HandleAttack(weaponnumber, comboTime);

            // 콤보 횟수 리셋 (3타 후 다시 1타로)
            if (comboTime > 3)
            {
                comboTime = 1;
            }
        }
    }

    //화살 발사
    void fireArrow()
    {
        Quaternion arrowrotation = transform.rotation;
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.transform.position, arrowSpawnPoint.transform.rotation);
    
        DamageDealer dealer = arrow.GetComponent<DamageDealer>();

        if(dealer != null)
        {
            dealer.SetOwner(gameObject);

            dealer.SetDamage(model.ATT);
        }
    
        Projectile projectile = arrow.GetComponent<Projectile>();
        if(projectile != null)
        {
            projectile.Launch();
        }
    }

    //공격중 반환
    public bool OnAttack()
    {
        if (isCharge) return false;
        else if (!isAttacking) return false;
        else return true;
    }

    //팬턴트 사용
    public void UsePendent()
    {
        if (_MasterManager.Instance.DataManager.HojuMoney() < 200) return;

        _MasterManager.Instance.DataManager.SpendMoney(200);

        Debug.Log("200원 나감");

        _MasterManager.Instance.DataManager.SetisPendant(true);
        _MasterManager.Instance.DataManager.ChangeReturn(true);
    }

    void DontMove()
    {
        CC.enabled = false;
    }
}
