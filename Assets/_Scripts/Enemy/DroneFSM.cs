using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 드론 전용 FSM
/// 
/// ✔ NavMesh 미사용
/// ✔ 이동 : 상/하/좌/우 랜덤 이동 (Raycast로 맵 경계 체크)
/// ✔ 공격 : 랜덤 4방향 레이저
/// ✔ 공격 중 이동 가능
/// ✔ 공격 방향 기준 수직 이동
/// ✔ 애니메이션 없음
/// </summary>
public class DroneFSM : MonoBehaviour
{
    enum DroneState { Idle, Move, Attack, GetHit, Die }
    DroneState state;

    RoomController roomController;
    public MonsterData drone;
    float currentHP;

    Vector3 moveDir;
    Vector3 attackDir;

    [Header("Movement")]
    public float moveSpeed;
    public float minMoveDistance = 1.5f;
    public float maxMoveDistance = 4f;

    // 🔥 이 변수 하나로만 벽 회피 처리
    public float wallCheckDistance = 5f;
    public LayerMask wallLayer;

    float moveRemain;

    [Header("Attack")]
    public GameObject laserPrefab;
    public Transform firePoint;
    public float attackCooldown;
    float attackTimer;

    readonly Vector3[] directions =
    {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right
    };

    void Start()
    {
        if (drone == null)
        {
            Debug.LogError("DroneFSM : MonsterData not assigned");
            enabled = false;
            return;
        }

        currentHP = drone.HP;
        moveSpeed = drone.Speed;
        attackCooldown = drone.CoolTime;

        state = DroneState.Idle;
        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        switch (state)
        {
            case DroneState.Idle: Idle(); break;
            case DroneState.Move: Move(); break;
            case DroneState.Attack: Attack(); break;
        }
    }

    void Idle()
    {
        if (attackTimer <= 0f)
        {
            state = DroneState.Attack;
            return;
        }

        ChooseMoveDirection();
        state = DroneState.Move;
    }

    void Move()
    {
        RotateTo(moveDir);

        Vector3 origin = transform.position + Vector3.up * 0.2f;
        float step = moveSpeed * Time.deltaTime;

        // 🔥 이동 중에도 "이 방향에 벽 있으면" 즉시 꺾기
        if (Physics.Raycast(origin, moveDir, wallCheckDistance, wallLayer))
        {
            ChooseMoveDirection();
            return;
        }

        transform.position += moveDir * step;
        moveRemain -= step;

        if (moveRemain <= 0f)
            state = DroneState.Idle;
    }

    void RotateTo(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        Quaternion target = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 10f);
    }

    void Attack()
    {
        attackDir = GetRandomDirection();
        RotateTo(attackDir);
        FireLaser();

        attackTimer = attackCooldown;
        ChoosePerpendicularMove();
        state = DroneState.Move;
    }

    void FireLaser()
    {
        if (laserPrefab == null || firePoint == null)
            return;

        Quaternion rot = Quaternion.LookRotation(attackDir);
        Instantiate(laserPrefab, firePoint.position, rot);
    }

    // 🔥 wallCheckDistance 하나로 방향 후보 필터링
    void ChooseMoveDirection()
    {
        List<Vector3> validDirs = new List<Vector3>();
        Vector3 origin = transform.position + Vector3.up * 0.2f;

        foreach (var dir in directions)
        {
            if (Physics.Raycast(origin, dir, wallCheckDistance, wallLayer))
                continue;

            validDirs.Add(dir);
        }

        moveDir = validDirs.Count > 0
            ? validDirs[Random.Range(0, validDirs.Count)]
            : Vector3.zero;

        moveRemain = Random.Range(minMoveDistance, maxMoveDistance);
    }

    // 🔥 공격 후 수직 이동도 동일 기준
    void ChoosePerpendicularMove()
    {
        List<Vector3> candidates = new List<Vector3>();

        if (attackDir == Vector3.forward || attackDir == Vector3.back)
        {
            candidates.Add(Vector3.left);
            candidates.Add(Vector3.right);
        }
        else
        {
            candidates.Add(Vector3.forward);
            candidates.Add(Vector3.back);
        }

        List<Vector3> valid = new List<Vector3>();
        Vector3 origin = transform.position + Vector3.up * 0.2f;

        foreach (var dir in candidates)
        {
            if (Physics.Raycast(origin, dir, wallCheckDistance, wallLayer))
                continue;

            valid.Add(dir);
        }

        moveDir = valid.Count > 0
            ? valid[Random.Range(0, valid.Count)]
            : Vector3.zero;

        moveRemain = Random.Range(minMoveDistance, maxMoveDistance);
    }

    Vector3 GetRandomDirection()
    {
        return directions[Random.Range(0, directions.Length)];
    }

    public void TakeDamage(DamageData data)
    {
        if (state == DroneState.Die) return;

        currentHP -= data.damageAmount;

        if (currentHP <= 0)
            Die();
    }

    public void Die()
    {
        state = DroneState.Die;
        StartCoroutine(DieProc());
    }

    IEnumerator DieProc()
    {
        DropItems();
        yield return new WaitForSeconds(3f);

        roomController?.ClearDungeon(gameObject);
        _MasterManager.Instance.DataManager.GetMonster(drone);
        Destroy(gameObject);
    }

    void DropItems()
    {
        if (drone.DropTable == null) return;

        foreach (var drop in drone.DropTable)
        {
            if (Random.value > drop.chance) continue;

            int count = Random.Range(drop.minCount, drop.maxCount + 1);
            for (int i = 0; i < count; i++)
                Instantiate(drop.itemPrefab, transform.position + GetRandomDropOffset(), Quaternion.identity);
        }
    }

    Vector3 GetRandomDropOffset()
    {
        Vector2 rand = Random.insideUnitCircle * 0.5f;
        return new Vector3(rand.x, 0f, rand.y);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, wallCheckDistance);

        if (!Application.isPlaying) return;

        foreach (var dir in directions)
        {
            Gizmos.color = Physics.Raycast(transform.position, dir, wallCheckDistance, wallLayer)
                ? Color.red
                : Color.green;

            Gizmos.DrawRay(transform.position, dir * wallCheckDistance);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, moveDir * wallCheckDistance);
    }
}

