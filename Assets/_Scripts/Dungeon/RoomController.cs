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

    public static bool isCleared = false;

    public bool isStartRoom = false;
    public bool isBossRoom = false;
    public bool isRestRoom = false;


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
            Instantiate(monsterPrefabs[rand], point.position, Quaternion.identity);
        }
    }

    public void SpawnMonstersOnce()
    {
        if (isSpawned) return;       // ★ 이미 스폰했으면 더 이상 스폰 안 함

        isSpawned = true;            // 스폰 표시

        SpawnMonster();             // 기존 몬스터 생성 함수 호출
    }


    public static void ClearDungeon()
    {
        isCleared = true;
        Debug.Log("던전 클리어! 문이 열렸습니다.");
    }

    
}
