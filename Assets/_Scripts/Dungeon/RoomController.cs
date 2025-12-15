using UnityEngine;
using System.Collections.Generic;



public class RoomController : MonoBehaviour
{
    [Header("문 설정")]
    public GameObject doorUp;
    public GameObject doorDown;
    public GameObject doorLeft;
    public GameObject doorRight;

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


    public void SetDoorActive(bool up, bool down, bool left, bool right)
    {
        doorUp?.SetActive(up);
        doorDown?.SetActive(down);
        doorLeft?.SetActive(left);
        doorRight?.SetActive(right);
    }

    void SetDoorInteractable(GameObject door, bool enable)
    {
        if (door == null) return;

        Collider col = door.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = enable;
        }
    }

    public void SetAllDoorsInteractable(bool enable)
    {
        SetDoorInteractable(doorUp, enable);
        SetDoorInteractable(doorDown, enable);
        SetDoorInteractable(doorLeft, enable);
        SetDoorInteractable(doorRight, enable);
    }

    public void OnPlayerEnterRoom()
    {
        // 시작방 / 휴식방은 항상 문 열림
        if (isStartRoom || isRestRoom)
        {
            SetAllDoorsInteractable(true);
            return;
        }

        // 전투방은 입장 시 문 잠금
        SetAllDoorsInteractable(false);

        // 몬스터 1회 스폰
        SpawnMonstersOnce();
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

        SpawnMonster();             // 기존 몬스터 생성 함수 호출
    }


    public void ClearDungeon(GameObject monster)
    {
        if (isCleared) return;

        if (aliveMonsters.Contains(monster))
        {
            aliveMonsters.Remove(monster);
        }

        // 모두 죽으면 문 작동 가능
        if (aliveMonsters.Count == 0)
        {
            isCleared = true;
            SetAllDoorsInteractable(true);
            Debug.Log("방 클리어! 문 열림");
        }
    }

}
