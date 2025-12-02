using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

public class DungeonManager : MonoBehaviour
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
        // 게임이 시작될 때 던전 생성 함수를 호출합니다.
        MakeDungeon();
    }

    // 방의 좌표와 그 방에 연결되는 문 방향 정보를 저장하기 위한 구조체입니다.
    private struct RoomData
    {
        public Vector2Int gridPos; // 방의 그리드 좌표
        // 문이 열린 방향을 저장합니다. (이 방에서 다음 방으로 가는 출구 방향)
        public bool hasDoorUp, hasDoorDown, hasDoorLeft, hasDoorRight;
    }

    // 생성된 방들의 좌표와 문 연결 정보를 저장하는 리스트로 변경합니다.
    private List<RoomData> roomPath = new List<RoomData>();

    void MakeDungeon()
    {
        // 던전 생성을 시작하기 전에 경로 및 좌표 정보를 초기화합니다.
        roomCoordinates.Clear();
        roomPath.Clear();

        Vector2Int currentPos = Vector2Int.zero;

        // 1. 총 방 개수 결정
        int totalRooms = Random.Range(10, 12);

        // 2. [고정] 스타트 방 생성 (0,0) - 무조건 1번째 방
        // PlaceRoom(startRoomPrefab, currentPos); // ➡️ 나중에 일괄 배치하기 위해 잠시 주석 처리
        roomCoordinates.Add(currentPos);

        // Start 방은 이웃한 방이 결정될 때까지 문 정보를 비워둡니다.
        RoomData startRoomData = new RoomData { gridPos = currentPos };
        roomPath.Add(startRoomData);

        // 3. 중간 랜덤 경로 방 리스트 생성
        int numPureRandoms = totalRooms - 3; // 3~5개
        List<GameObject> pureRandomRooms = new List<GameObject>();
        GameObject[] randomPool = { normalRoomPrefab };

        // 순수 랜덤 방들을 개수만큼 채워 넣습니다.
        for (int i = 0; i < numPureRandoms; i++)
        {
            pureRandomRooms.Add(randomPool[Random.Range(0, randomPool.Length)]);
        }

        // 4. 순수 랜덤 방 리스트 섞기 (Shuffle)
        pureRandomRooms = ShuffleList(pureRandomRooms);

        // 5. [중간] 순수 랜덤 방 배치 (경로 저장)
        // 섞인 랜덤 방들을 순서대로 배치하면서 경로를 기록합니다.
        foreach (GameObject roomPrefab in pureRandomRooms)
        {
            currentPos = GetNextRandomPosition(currentPos);
            roomCoordinates.Add(currentPos);
            // RoomData를 추가합니다. 문 정보는 다음 방이 결정될 때 업데이트됩니다.
            roomPath.Add(new RoomData { gridPos = currentPos });
        }

        // 6. [고정] 휴식 방 생성
        currentPos = GetNextRandomPosition(currentPos);
        roomCoordinates.Add(currentPos);
        roomPath.Add(new RoomData { gridPos = currentPos });

        // 7. [고정] 보스 방 생성
        currentPos = GetNextRandomPosition(currentPos);
        roomCoordinates.Add(currentPos);
        roomPath.Add(new RoomData { gridPos = currentPos });

        // 8. 방 생성 및 문 연결 정보 확정
        // 이제 roomPath에 순서대로 모든 방의 좌표가 있으므로,
        // 순서대로 두 방씩 짝지어 연결 정보를 확정합니다.
        for (int i = 0; i < roomPath.Count; i++)
        {
            // 현재 방의 정보입니다.
            RoomData currentRoomData = roomPath[i];

            // 주석: 다음 방의 좌표입니다. (마지막 방이 아니면 다음 방이 존재합니다.)
            Vector2Int nextPos = (i < roomPath.Count - 1) ? roomPath[i + 1].gridPos : Vector2Int.zero;

            // 이전 방의 좌표입니다. (첫 방이 아니면 이전 방이 존재합니다.)
            Vector2Int prevPos = (i > 0) ? roomPath[i - 1].gridPos : Vector2Int.zero;

            // 다음 방으로 나가는 출구 문을 결정합니다.
            if (i < roomPath.Count - 1)
            {
                // 다음 방 좌표 - 현재 방 좌표 = 이동 방향
                Vector2Int dirToNext = nextPos - currentRoomData.gridPos;

                // 이동 방향에 따라 currentRoomData에 문 정보를 설정합니다.
                if (dirToNext == Vector2Int.up) currentRoomData.hasDoorUp = true;
                else if (dirToNext == Vector2Int.down) currentRoomData.hasDoorDown = true;
                else if (dirToNext == Vector2Int.left) currentRoomData.hasDoorLeft = true;
                else if (dirToNext == Vector2Int.right) currentRoomData.hasDoorRight = true;
            }

            // 이전 방에서 들어오는 입구 문을 결정합니다.
            if (i > 0)
            {
                // 현재 방 좌표 - 이전 방 좌표 = 이동 방향
                Vector2Int dirFromPrev = currentRoomData.gridPos - prevPos;

                // 들어온 방향의 반대 방향 문을 현재 방에 설정합니다.
                if (dirFromPrev == Vector2Int.up) currentRoomData.hasDoorUp = true;
                else if (dirFromPrev == Vector2Int.down) currentRoomData.hasDoorDown = true;
                else if (dirFromPrev == Vector2Int.left) currentRoomData.hasDoorLeft = true;
                else if (dirFromPrev == Vector2Int.right) currentRoomData.hasDoorRight = true;
            }

            // RoomData를 업데이트합니다.
            roomPath[i] = currentRoomData;

            // 방의 종류를 결정하고 PlaceRoom 함수를 호출하여 실제 생성 및 문 정보를 전달합니다.
            GameObject prefabToPlace;
            if (i == 0) prefabToPlace = startRoomPrefab;
            else if (i == roomPath.Count - 1) prefabToPlace = bossRoomPrefab;
            else if (i == roomPath.Count - 2) prefabToPlace = restRoomPrefab;
            else prefabToPlace = pureRandomRooms[i - 1]; // Start 방 제외이므로 i-1 인덱스 사용

            PlaceRoom(prefabToPlace, currentRoomData); // 수정된 함수 호출
        }

        Debug.Log($"[Dungeon Info] Master, Total Rooms: {totalRooms}. Path established: Start -> Random ({numPureRandoms} rooms) -> Rest -> Boss.");
    }

    // 다음으로 이동할 랜덤한 좌표를 찾는 함수
    Vector2Int GetNextRandomPosition(Vector2Int currentPos)
    {
        Vector2Int nextPos = currentPos;
        bool foundValidPos = false;

        // 겹치지 않는 좌표를 찾을 때까지 무한 반복합니다.
        while (!foundValidPos)
        {
            // 0:위, 1:아래, 2:왼쪽, 3:오른쪽
            int direction = Random.Range(0, 4);
            Vector2Int moveDir = Vector2Int.zero;

            // switch-case문: 랜덤하게 선택된 방향에 따라 이동할 방향 벡터를 결정합니다.
            switch (direction)
            {
                case 0: moveDir = Vector2Int.up; break;    // 위로 이동 (Z+)
                case 1: moveDir = Vector2Int.down; break;  // 아래로 이동 (Z-)
                case 2: moveDir = Vector2Int.left; break;  // 왼쪽으로 이동 (X-)
                case 3: moveDir = Vector2Int.right; break; // 오른쪽으로 이동 (X+)
            }

            Vector2Int potentialPos = currentPos + moveDir;

            // roomCoordinates 리스트에 이 좌표가 없다면 (빈 공간이라면) 유효한 위치입니다.
            // Contains(T item): 리스트에 특정 요소가 포함되어 있는지 확인합니다.
            if (!roomCoordinates.Contains(potentialPos))
            {
                nextPos = potentialPos;
                foundValidPos = true; // 유효한 위치를 찾았으니 반복을 중단합니다.
            }
        }

        return nextPos;
    }

    // 실제 프리팹을 게임 세상에 소환하는 함수
    // 이제 RoomData를 인수로 받아 문 정보를 Room 스크립트에 전달합니다.
    void PlaceRoom(GameObject prefab, RoomData roomData)
    {
        Vector3 worldPos = new Vector3(
            roomData.gridPos.x * roomSpacingX, // X좌표 = 그리드 X * 간격 X
            0,
            roomData.gridPos.y * roomSpacingZ  // Z좌표 = 그리드 Y * 간격 Z (Unity에서 Y는 보통 Up/Down이므로 Z축을 사용합니다.)
        );

        // Instantiate(prefab, position, rotation): 프리팹을 월드에 소환하고 그 인스턴스를 반환합니다.
        GameObject newRoom = Instantiate(prefab, worldPos, Quaternion.identity);

        // GetNextRandomPosition에서 결정된 문 정보를 Room 컴포넌트에 전달합니다.
        // GetComponent<T>(): 게임 오브젝트에 부착된 특정 타입의 컴포넌트를 가져옵니다.
        Room roomComponent = newRoom.GetComponent<Room>();

        // if문: Room 컴포넌트가 있다면 문 초기화 함수를 호출하여 문을 엽니다.
        if (roomComponent != null)
        {
            // InitializeDoors(): Room 스크립트의 함수를 호출하여 문 활성화/비활성화 상태를 설정합니다.
            roomComponent.InitializeDoors(
                roomData.hasDoorUp,
                roomData.hasDoorDown,
                roomData.hasDoorLeft,
                roomData.hasDoorRight
            );
        }
        else
        {
            Debug.LogWarning($"Master, The Room component is missing on the {prefab.name} prefab. Door opening cannot be executed.");
        }
    }

    // 리스트를 무작위로 섞는 함수
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

    public void Initialize()
    {

    }
}