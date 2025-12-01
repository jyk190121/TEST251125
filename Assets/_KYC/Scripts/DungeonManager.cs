using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IsaacDungeonGenerator : MonoBehaviour
{
    [Header("--- Map Prefabs ---")]
    public GameObject startRoomPrefab;    // 1번 방 (고정)
    public GameObject normalRoomPrefab;   // 일반 몬스터 (랜덤 후보)
    public GameObject restRoomPrefab;     // 보스 직전 방 (고정)
    public GameObject bossRoomPrefab;     // 맨 끝 방 (고정)

    [Header("--- Settings ---")]
    [Tooltip("방 오브젝트의 가로 길이(X축)에 맞춰 설정하세요.")]
    public float roomSpacingX = 20.0f;

    [Tooltip("방 오브젝트의 세로 길이(Z축)에 맞춰 설정하세요.")]
    public float roomSpacingZ = 15.0f;

    private List<Vector2Int> roomCoordinates = new List<Vector2Int>();

    void Start()
    {
        MakeDungeon();
    }

    void MakeDungeon()
    {
        roomCoordinates.Clear();
        Vector2Int currentPos = Vector2Int.zero;

        // 1. 총 방 개수 결정 (6, 7, 8 중 랜덤)
        int totalRooms = Random.Range(6, 9);

        // 2. [고정] 스타트 방 생성 (0,0) - 무조건 1번째 방
        PlaceRoom(startRoomPrefab, currentPos);
        roomCoordinates.Add(currentPos);

        // 3. 중간 랜덤 경로 방 리스트 생성
        // Start(1) + Rest(1) + Boss(1) = 3개 방을 제외한 나머지 순수 랜덤 방의 개수입니다.
        int numPureRandoms = totalRooms - 3; // 3~5개

        // 순수 랜덤 방들만 담을 리스트입니다.
        List<GameObject> pureRandomRooms = new List<GameObject>();

        // 랜덤 후보군 배열: Normal, Treasure, Merchant
        GameObject[] randomPool = { normalRoomPrefab };

        // for문: 순수 랜덤 방들을 개수(3~5개)만큼 채워 넣습니다.
        for (int i = 0; i < numPureRandoms; i++)
        {
            // 후보군 중 하나를 랜덤으로 뽑아 리스트에 추가합니다.
            pureRandomRooms.Add(randomPool[Random.Range(0, randomPool.Length)]);
        }

        // 4. 순수 랜덤 방 리스트 섞기 (Shuffle)
        // 이 방들의 순서만 무작위로 만듭니다.
        pureRandomRooms = ShuffleList(pureRandomRooms);

        // 5. [중간] 순수 랜덤 방 배치
        // foreach문: 섞인 랜덤 방들을 순서대로 배치합니다.
        foreach (GameObject roomPrefab in pureRandomRooms)
        {
            currentPos = GetNextRandomPosition(currentPos);
            PlaceRoom(roomPrefab, currentPos);
            roomCoordinates.Add(currentPos);
        }

        // 6. [고정] 휴식 방 생성 - 무조건 보스 방 바로 앞
        currentPos = GetNextRandomPosition(currentPos);
        PlaceRoom(restRoomPrefab, currentPos);
        roomCoordinates.Add(currentPos);

        // 7. [고정] 보스 방 생성 - 무조건 맨 끝 방
        currentPos = GetNextRandomPosition(currentPos);
        PlaceRoom(bossRoomPrefab, currentPos);
        roomCoordinates.Add(currentPos);

        Debug.Log($"[Dungeon Info] Master, Total Rooms: {totalRooms}. Path established: Start -> Random ({numPureRandoms} rooms) -> Rest -> Boss.");
    }

    // 다음으로 이동할 랜덤한 좌표를 찾는 함수
    Vector2Int GetNextRandomPosition(Vector2Int currentPos)
    {
        Vector2Int nextPos = currentPos;
        bool foundValidPos = false;

        // while문: 겹치지 않는 좌표를 찾을 때까지 무한 반복합니다.
        while (!foundValidPos)
        {
            // 0:위, 1:아래, 2:왼쪽, 3:오른쪽
            int direction = Random.Range(0, 4);
            Vector2Int moveDir = Vector2Int.zero;

            switch (direction)
            {
                case 0: moveDir = Vector2Int.up; break;    // 위로 이동 (Z+)
                case 1: moveDir = Vector2Int.down; break;  // 아래로 이동 (Z-)
                case 2: moveDir = Vector2Int.left; break;  // 왼쪽으로 이동 (X-)
                case 3: moveDir = Vector2Int.right; break; // 오른쪽으로 이동 (X+)
            }

            Vector2Int potentialPos = currentPos + moveDir;

            // if문: 리스트에 이 좌표가 없다면 (빈 공간이라면) 유효한 위치입니다.
            if (!roomCoordinates.Contains(potentialPos))
            {
                nextPos = potentialPos;
                foundValidPos = true; // 유효한 위치를 찾았으니 반복을 중단합니다.
            }
        }

        return nextPos;
    }

    // 실제 프리팹을 게임 세상에 소환하는 함수 (수정된 간격 변수 적용됨)
    void PlaceRoom(GameObject prefab, Vector2Int gridPos)
    {
        Vector3 worldPos = new Vector3(
            gridPos.x * roomSpacingX,
            0,
            gridPos.y * roomSpacingZ
        );

        Instantiate(prefab, worldPos, Quaternion.identity);
    }

    // 리스트를 무작위로 섞는 함수 (이전과 동일)
    List<T> ShuffleList<T>(List<T> list)
    {
        System.Random random = new System.Random();

        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }
}