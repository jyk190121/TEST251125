using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;



public class RoomController : MonoBehaviour
{
    [Header("문 설정")]
    public GameObject doorUp;
    public GameObject doorDown;
    public GameObject doorLeft;
    public GameObject doorRight;

    public List<Collider> doorColliders = new List<Collider>();

    public GameObject PortalSpawnPoint;
    public GameObject PortalPrefab;

    [Header("몬스터 스폰 포인트")]
    public List<Transform> spawnPoints;

    [Header("몬스터 프리팹 리스트")]
    public List<GameObject> monsterPrefabs;   // 랜덤 스폰용

    private GameObject selectedNormalMonsterPrefab; 

    [Header("보스 몬스터")]
    public GameObject bossMonsterPrefab;

    [Header("방 확인용")]
    public bool isSpawned = false;

    public bool isCleared = false;

    public bool isStartRoom = false;
    public bool isBossRoom = false;
    public bool isRestRoom = false;

    public bool IsNormalRoom =>
    !isStartRoom && !isRestRoom && !isBossRoom;

    private List<GameObject> aliveMonsters = new List<GameObject>();

    public bool IsBoss = false;

    private bool isSpawningInProgress = false;

    private void Start()
    {
        if (isStartRoom || isRestRoom)
        {
            isCleared = true; // 자동 클리어
            UnlockDoors();    // 문을 열어 통과 가능하게 합니다.
        }
    }

    private void Update()
    {
        // 팩트체크: 방이 이미 클리어 되었거나, 스폰 전이라면 체크할 필요가 없습니다.
        if (isCleared || !isSpawned || isSpawningInProgress) return;

        // 1. 리스트에서 이미 파괴된(null이 된) 몬스터를 모두 제거합니다.
        // monster == null 조건은 GameObject가 Destroy되었을 때 true가 됩니다.
        aliveMonsters.RemoveAll(monster => monster == null);

        // 2. 리스트가 비어있다면 (모든 몹이 죽었다면) 클리어 처리
        if (aliveMonsters.Count == 0)
        {
            CheckRoomClear();
        }
    }

    public void SetDoorActive(bool up, bool down, bool left, bool right)
    {
        doorUp?.SetActive(up);
        doorDown?.SetActive(down);
        doorLeft?.SetActive(left);
        doorRight?.SetActive(right);
    }

    public void SpawnMonster()
    {
        if (isStartRoom || isRestRoom) return; // 스타트, 쉬는방 제외

        if (isBossRoom)
        {
            if (bossMonsterPrefab == null) return;
            if (spawnPoints.Count == 0) return;

            // 첫 번째 스폰 포인트에만 1마리 생성
            Transform spawnPoint = spawnPoints[0];

            GameObject boss = Instantiate(
                bossMonsterPrefab,
                spawnPoint.position,
                Quaternion.identity
            );

            aliveMonsters.Add(boss);

            boss.GetComponent<DragonFSM>()?.SetupRoom(this);

            if (boss.layer == LayerMask.NameToLayer("Boss"))
            {
                IsBoss = true;
            }

            return; 
        }

        if (monsterPrefabs.Count == 0) return;
        if (spawnPoints.Count == 0) return;

        if (selectedNormalMonsterPrefab == null)
        {
            int rand = Random.Range(0, monsterPrefabs.Count);
            selectedNormalMonsterPrefab = monsterPrefabs[rand];
        }

        foreach (var point in spawnPoints)
        {
            GameObject monster = Instantiate(
                selectedNormalMonsterPrefab,
                point.position,
                Quaternion.identity
            );

            aliveMonsters.Add(monster);
            monster.GetComponent<NormalMosterFSM>()?.SetupRoom(this);
        }
        Debug.Log("몹생성");
    }

    public void SpawnMonstersOnce()
    {
        if (isSpawned) return;       // 이미 스폰했으면 더 이상 스폰 안 함

        isSpawned = true;            // 스폰 표시

        StartCoroutine(SpawnProcessRoutine());
    }

    IEnumerator SpawnProcessRoutine()
    {
        isSpawningInProgress = true; // "지금 몹 만드는 중이니까 기다려!"

        // 팩트체크: 실제 몬스터 생성 함수 실행
        SpawnMonster();

        LockDoors();

        // 0.2초 정도 여유를 주어 Instantiate가 완료되고 리스트에 들어갈 시간을 줍니다.
        yield return new WaitForSeconds(0.2f);

        isSpawningInProgress = false;

    }


    public void ClearDungeon(GameObject monster)
    {
        if (isCleared) return;

        if (aliveMonsters.Contains(monster))
        {
            aliveMonsters.Remove(monster);
        }

        // 모두 죽으면 문 열기
        CheckRoomClear();
    }

    public void LockDoors()
    {
        
        if (isCleared) return;

        foreach (var collider in doorColliders)
        {
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }

    public void UnlockDoors()
    {
        
        if (!isCleared) return;

        foreach (var collider in doorColliders)
        {
            if (collider != null)
            {   
                collider.enabled = false;
            }
        }
    }

    public void SpawnExitPortal() 
    {
        GameObject portal = Instantiate(PortalPrefab, PortalSpawnPoint.transform.position, Quaternion.identity);
        Debug.Log("보스 클리어! 마을 복귀용 포탈이 생성되었습니다.");
    }

    private void CheckRoomClear()
    {
        if (isCleared) return;
        if (aliveMonsters.Count > 0) return; // 아직 살아있는 몹이 있다면 중단

        isCleared = true;
        Debug.Log("Master, 모든 적을 처치했습니다! 문을 엽니다.");
        UnlockDoors();

        if (isBossRoom || IsBoss)
        {
            SpawnExitPortal();
        }
    }

    public void ClearAllMonsters()
    {
        for (int i = aliveMonsters.Count - 1; i >= 0; i--)
        {
            if (aliveMonsters[i] != null)
            {
                Destroy(aliveMonsters[i]);
            }
        }

        aliveMonsters.Clear();

        Debug.Log("잼 마스터, 플레이어의 상태 변화에 따라 모든 몬스터를 퇴거시켰습니다.");
    }
}
