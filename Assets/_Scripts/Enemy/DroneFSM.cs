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
    /*───────────────────────────────*
     * 상태 정의
     *───────────────────────────────*/
    enum DroneState
    {
        Idle,
        Move,
        Attack
    }

    DroneState state;

    /*───────────────────────────────*
     * 이동 / 공격 방향
     *───────────────────────────────*/
    Vector3 moveDir;      // 현재 이동 방향
    Vector3 attackDir;    // 현재 공격 방향

    /*───────────────────────────────*
     * 이동 설정
     *───────────────────────────────*/
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float minMoveDistance = 1.5f;
    public float maxMoveDistance = 4f;
    public float wallCheckDistance = 5f;
    public LayerMask wallLayer;

    float moveRemain;

    /*───────────────────────────────*
     * 공격 설정
     *───────────────────────────────*/
    [Header("Attack")]
    public GameObject laserPrefab;
    public Transform firePoint;

    public float attackCooldown = 4f;
    float attackTimer;

    /*───────────────────────────────*
     * 4방향 정의
     *───────────────────────────────*/
    readonly Vector3[] directions =
    {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right
    };

    /*───────────────────────────────*
     * 초기화
     *───────────────────────────────*/
    void Start()
    {
        state = DroneState.Idle;
        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        switch (state)
        {
            case DroneState.Idle:
                Idle();
                break;
            case DroneState.Move:
                Move();
                break;
            case DroneState.Attack:
                Attack();
                break;
        }
    }

    /*───────────────────────────────*
     * Idle
     *───────────────────────────────*/
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

    /*───────────────────────────────*
     * Move
     *───────────────────────────────*/
    void Move()
    {
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        moveRemain -= Time.deltaTime * moveSpeed;

        // 이동 중에도 공격 가능
        if (attackTimer <= 0f)
        {
            state = DroneState.Attack;
            return;
        }

        if (moveRemain <= 0f)
        {
            state = DroneState.Idle;
        }
    }

    /*───────────────────────────────*
     * Attack
     *───────────────────────────────*/
    void Attack()
    {
        attackDir = GetRandomDirection();
        FireLaser();

        attackTimer = attackCooldown;

        // 공격 방향 기준 수직 이동
        ChoosePerpendicularMove();

        state = DroneState.Move;
    }

    /*───────────────────────────────*
     * 레이저 발사
     *───────────────────────────────*/
    void FireLaser()
    {
        if (laserPrefab == null || firePoint == null)
            return;

        Quaternion rot = Quaternion.LookRotation(attackDir);
        Instantiate(laserPrefab, firePoint.position, rot);
    }

    /*───────────────────────────────*
     * 이동 방향 선택
     *───────────────────────────────*/
    void ChooseMoveDirection()
    {
        List<Vector3> validDirs = new List<Vector3>();

        foreach (var dir in directions)
        {
            if (!Physics.Raycast(transform.position, dir, wallCheckDistance, wallLayer))
                validDirs.Add(dir);
        }

        if (validDirs.Count == 0)
            moveDir = Vector3.zero;
        else
            moveDir = validDirs[Random.Range(0, validDirs.Count)];

        moveRemain = Random.Range(minMoveDistance, maxMoveDistance);
    }

    /*───────────────────────────────*
     * 공격 방향의 수직 이동
     *───────────────────────────────*/
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
        foreach (var dir in candidates)
        {
            if (!Physics.Raycast(transform.position, dir, wallCheckDistance, wallLayer))
                valid.Add(dir);
        }

        if (valid.Count > 0)
            moveDir = valid[Random.Range(0, valid.Count)];
        else
            moveDir = Vector3.zero;

        moveRemain = Random.Range(minMoveDistance, maxMoveDistance);
    }

    /*───────────────────────────────*
     * 랜덤 4방향
     *───────────────────────────────*/
    Vector3 GetRandomDirection()
    {
        return directions[Random.Range(0, directions.Length)];
    }

    /*───────────────────────────────*
     * Gizmos
     *───────────────────────────────*/
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, wallCheckDistance);
    }
}
