using UnityEngine;
using System.Collections.Generic;



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

    public bool isSpawned = false;

    public bool isCleared = false;

    public bool isStartRoom = false;
    public bool isBossRoom = false;
    public bool isRestRoom = false;

    private List<GameObject> aliveMonsters = new List<GameObject>();

    public bool IsBoss = false;

    private void Start()
    {
        if (isStartRoom || isRestRoom)
        {
            isCleared = true; // 자동 클리어
            UnlockDoors();    // 문을 열어 통과 가능하게 합니다.
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

        if (monsterPrefabs.Count == 0) return;
        if (spawnPoints.Count == 0) return;

        foreach (var point in spawnPoints)
        {
            int rand = Random.Range(0, monsterPrefabs.Count);
            GameObject monster = Instantiate(monsterPrefabs[rand], point.position, Quaternion.identity);
            aliveMonsters.Add(monster);
            //monster.GetComponent<MonsterTest>().SetupRoom(this);

            monster.GetComponent<TestMonster>()?.SetupRoom(this);

            if (monster.layer == LayerMask.NameToLayer("Boss"))
            {
                IsBoss = true;
            }
        }

    }

    public void SpawnMonstersOnce()
    {
        if (isSpawned) return;       // 이미 스폰했으면 더 이상 스폰 안 함

        isSpawned = true;            // 스폰 표시

        LockDoors();

        SpawnMonster();             // 기존 몬스터 생성 함수 호출
    }

    
    public void ClearDungeon(GameObject monster)
    {
        if (isCleared) return;

        if (aliveMonsters.Contains(monster))
        {
            aliveMonsters.Remove(monster);
        }

        // 모두 죽으면 문 열기
        if (aliveMonsters.Count == 0)
        {
            isCleared = true;
            Debug.Log("방 클리어! 문 열림");
            UnlockDoors();

            if (isBossRoom || IsBoss)
            {
                SpawnExitPortal();
            }
        }
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

    private void SpawnExitPortal()
    {
        GameObject portal = Instantiate(PortalPrefab, PortalSpawnPoint.transform.position, Quaternion.identity);
        Debug.Log("보스 클리어! 마을 복귀용 포탈이 생성되었습니다.");
    }
}
