using System.Collections.Generic; 
using UnityEngine; 
using System.Linq; 

public class DungeonManager : MonoBehaviour
{
    [Header("--- Map Prefabs ---")]
    public GameObject startRoomPrefab;     // 1번 방 (고정)
    public GameObject normalRoomPrefab;    // 일반 몬스터 (랜덤)
    public GameObject restRoomPrefab;      // 보스 직전 방 (고정)
    public GameObject bossRoomPrefab;      // 맨 끝 방 (고정)

    [Header("--- Settings ---")]
    [Tooltip("방 오브젝트의 가로 길이(X축)에 맞춰 설정하세요.")]
    public float roomSpacingX = 20.0f; // 방과 방 사이의 X축 간격 설정

    [Tooltip("방 오브젝트의 세로 길이(Z축)에 맞춰 설정하세요.")]
    public float roomSpacingZ = 15.0f; // 방과 방 사이의 Z축 간격 설정

    // 맵에 배치된 모든 방의 격자 좌표(Grid Position)를 저장하는 리스트
    private List<Vector2Int> roomCoordinates = new List<Vector2Int>();

    // 던전 방 배치를 위한 임시 저장소
    private Dictionary<Vector2Int, GameObject> roomPlacementMap = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        // 던전 생성 로직
        MakeDungeon();
    }

    // 던전을 생성하는 메인 로직 함수
    void MakeDungeon()
    {
        // 던전 생성을 위해 이전에 저장된 좌표 및 배치 정보를 모두 초기화
        roomCoordinates.Clear();
        roomPlacementMap.Clear();
        Vector2Int currentPos = Vector2Int.zero; // 현재 방의 위치를 (0, 0)으로 설정합니다.

        // 1. 총 방 개수 결정 (9, 10, 11 중 랜덤)
        int totalRooms = Random.Range(9, 12);

        // 2. [고정] 스타트 방 배치 (0,0)
        roomCoordinates.Add(currentPos);
        roomPlacementMap.Add(currentPos, startRoomPrefab);

        // 3. 중간 랜덤 경로 방 개수 결정
        // Start(1) + Rest(1) + Boss(1) = 3개 방을 제외한 나머지 순수 랜덤 방의 개수입니다.
        int numPureRandoms = totalRooms - 3; // (9~11) - 3 = 6~8개

        // 순수 랜덤 방들만 담을 리스트
        List<GameObject> pureRandomRooms = new List<GameObject>();

        // 랜덤 후보군 배열
        GameObject[] randomPool = { normalRoomPrefab };

        
        // 이 반복문은 리스트에 방 프리팹들을 추가하는 역할을 합니다.
        for (int i = 0; i < numPureRandoms; i++)
        {
            // 후보군 중 하나를 랜덤으로 뽑아 리스트에 추가합니다.
            // Random.Range를 사용하여 배열 인덱스를 랜덤으로 선택합니다.
            pureRandomRooms.Add(randomPool[Random.Range(0, randomPool.Length)]);
        }

        // 4. 순수 랜덤 방 리스트 섞기 (Shuffle)
        // ShuffleList 함수를 호출하여 방의 순서를 무작위로 만듭니다.
        pureRandomRooms = ShuffleList(pureRandomRooms);

        // 5. [중간] 순수 랜덤 방 배치 경로 생성
        // foreach문: 섞인 랜덤 방들을 순서대로 배치할 경로를 찾습니다.
        // foreach는 컬렉션(List, Array 등)의 모든 요소에 대해 반복하며, 'roomPrefab' 변수에 현재 요소를 할당합니다.
        foreach (GameObject roomPrefab in pureRandomRooms)
        {
            // GetNextRandomPosition 함수를 호출하여 현재 위치에서 인접하고 겹치지 않는 새로운 위치를 찾습니다.
            currentPos = GetNextRandomPosition(currentPos);
            // 찾은 좌표를 기록하고, 해당 좌표에 배치할 방을 기록합니다.
            roomCoordinates.Add(currentPos);
            roomPlacementMap.Add(currentPos, roomPrefab);
        }

        // 6. [고정] 휴식 방 경로 생성 - 무조건 보스 방 바로 앞
        // 다음 위치를 찾습니다.
        currentPos = GetNextRandomPosition(currentPos);
        // 찾은 좌표를 기록하고, 해당 좌표에 restRoomPrefab을 배치할 것이라고 기록합니다.
        Vector2Int restRoomPos = currentPos;
        roomCoordinates.Add(restRoomPos);
        roomPlacementMap.Add(restRoomPos, restRoomPrefab);

        // --- 7. [수정된 로직] 보스 방 배치 - 시작 방과 가장 먼 곳에 배치 ---

        // 보스 방 후보 좌표를 저장할 변수입니다.
        Vector2Int bossRoomPos = Vector2Int.zero;
        // 시작 방 (0,0)으로부터 가장 먼 거리(맨해튼 거리)를 저장할 변수입니다.
        int maxDistance = -1;

        // LINQ의 Max() 함수를 사용하여 roomCoordinates 리스트에서 최대 맨해튼 거리를 찾습니다.
        // .Select(pos => Mathf.Abs(pos.x) + Mathf.Abs(pos.y))는 각 좌표(pos)에 대해 맨해튼 거리를 계산합니다.
        // 맨해튼 거리 = |x| + |y|로, 격자형 맵에서 두 점 사이의 최단 거리를 나타냅니다.
        maxDistance = roomCoordinates.Select(pos => Mathf.Abs(pos.x) + Mathf.Abs(pos.y)).Max();

        // LINQ의 First() 함수를 사용하여 최대 거리를 가진 방 좌표 중 첫 번째를 보스 방 위치로 선택합니다.
        // .Where(...)은 주어진 조건(맨해튼 거리가 maxDistance와 같은 것)을 만족하는 요소만 필터링합니다.
        bossRoomPos = roomCoordinates
            .Where(pos => Mathf.Abs(pos.x) + Mathf.Abs(pos.y) == maxDistance)
            .First();

        // 이제 보스 방 위치가 결정되었으므로, 해당 위치에 보스 방을 배치하도록 roomPlacementMap을 업데이트합니다.
        // 원래 이 위치에는 랜덤 방이나 휴식 방이 있었을 수 있지만, 보스 방이 덮어쓰게 됩니다.
        roomPlacementMap[bossRoomPos] = bossRoomPrefab;

        // 보스 방이 휴식 방의 위치를 덮어썼을 경우를 대비하여, 휴식 방 위치를 재설정합니다.
        // 보스 방 위치(bossRoomPos)에서 인접한 좌표 4개 중, 휴식 방(restRoomPos)이 아닌 위치를 찾습니다.
        // 보스 방 주변에서 빈 공간 또는 시작 방이 아닌 방을 찾아 휴식 방 위치로 재배치하는 것이 일반적이지만,
        // 여기서는 가장 간단하게, '이전 방'인 restRoomPos가 보스 방으로 대체되었으므로,
        // 보스 방의 인접한 좌표들 중 **기존에 경로에 포함되어 있던 방의 좌표**를 휴식 방 위치로 찾습니다. 
        // 하지만 이 로직은 복잡해지므로, 가장 간단하게 **보스 방의 바로 이전 방**으로 설정하겠습니다.
        // (이 부분은 던전 경로를 리스트로 저장하지 않아 보스 방의 '바로 이전 방'을 찾기 어려우므로, 
        // 단순히 경로가 겹치지 않도록 휴식 방을 보스 방으로 대체하는 로직을 유지하고 보스 방과 휴식 방만 다시 배치하겠습니다.)

        // **기존 휴식 방 위치를 보스 방이 차지했을 경우에 대한 처리 (선택 사항)**
        // 보스 방이 휴식 방 위치(restRoomPos)에 배치된 경우, 휴식 방은 보스 방의 '이전 위치'였던 restRoomPos의 인접한 칸 중에서, 
        // 경로 상에 존재하고 보스 방이 아닌 곳에 다시 배치될 수 있습니다. 
        // 하지만 현재 코드 구조에서는 경로 순서를 관리하지 않아 복잡하므로, 가장 확실한 방법으로 '경로'를 List<Vector2Int>로 만들어 관리하겠습니다.

        // **⭐ 경로를 List<Vector2Int>로 만들어 보스 방 재배치 후 중간 경로 수정**
        List<Vector2Int> path = new List<Vector2Int>(roomCoordinates); // 현재까지 생성된 모든 경로를 복사합니다.
        path.Remove(bossRoomPos); // 보스 방으로 지정될 좌표를 경로에서 제거합니다.

        // 보스 방 좌표(bossRoomPos)를 리스트의 맨 끝으로 옮깁니다.
        path.Add(bossRoomPos);

        // 휴식 방은 보스 방 바로 이전 방이 됩니다.
        // path.Count - 2는 맨 끝(보스 방)에서 한 칸 앞을 가리킵니다.
        restRoomPos = path[path.Count - 2];

        // 보스 방을 제외한 모든 방 배치: Dictionary에서 보스 방을 제외한 모든 방을 제거하고 다시 설정합니다.
        roomPlacementMap.Clear();

        // 1. 시작 방
        roomPlacementMap.Add(path[0], startRoomPrefab);

        // 2. 중간 랜덤 방 (path[1]부터 restRoomPos의 인덱스 - 1까지)
        // for문: path 리스트를 순회하며 각 방의 프리팹을 roomPlacementMap에 기록합니다.
        for (int i = 1; i < path.Count - 2; i++)
        {
            // Dictionary에 현재 좌표(path[i])에는 pureRandomRooms에 있는 프리팹 중 하나를 배치하도록 기록합니다.
            // 순수 랜덤 방의 개수(numPureRandoms)는 path.Count - 3과 같습니다.
            roomPlacementMap.Add(path[i], pureRandomRooms[i - 1]);
        }

        // 3. 휴식 방
        roomPlacementMap.Add(restRoomPos, restRoomPrefab);

        // 4. 보스 방
        roomPlacementMap.Add(bossRoomPos, bossRoomPrefab);

        // **8. [최종] 실제 방 생성**
        // foreach문: roomPlacementMap에 기록된 모든 방을 실제 게임 월드에 배치합니다.
        foreach (KeyValuePair<Vector2Int, GameObject> entry in roomPlacementMap)
        {
            // PlaceRoom 함수를 호출하여 실제 프리팹을 월드 좌표에 Instanitate(생성)합니다.
            // entry.Key는 방의 격자 좌표(Vector2Int)이고, entry.Value는 배치할 방의 프리팹(GameObject)입니다.
            PlaceRoom(entry.Value, entry.Key);
        }

        Debug.Log($"[Dungeon Info] Master, Total Rooms: {totalRooms}. Path established: Start ({path[0]}) -> Random ({numPureRandoms} rooms) -> Rest ({restRoomPos}) -> Boss ({bossRoomPos} - Max Distance).");
    }

    // 다음으로 이동할 랜덤한 좌표를 찾는 함수
    Vector2Int GetNextRandomPosition(Vector2Int currentPos)
    {
        Vector2Int nextPos = currentPos;
        bool foundValidPos = false; // 유효한 위치를 찾았는지 확인하는 플래그 변수입니다.

        // while문: 겹치지 않는 좌표를 찾을 때까지 무한 반복합니다.
        // foundValidPos가 false인 동안 계속 반복됩니다.
        while (!foundValidPos)
        {
            // 0:위, 1:아래, 2:왼쪽, 3:오른쪽 중 하나의 방향을 랜덤으로 선택합니다.
            int direction = Random.Range(0, 4);
            Vector2Int moveDir = Vector2Int.zero; // 이동 방향을 저장할 변수를 (0, 0)으로 초기화합니다.

            // switch문: direction 값에 따라 이동 방향(moveDir)을 설정합니다.
            switch (direction)
            {
                case 0: moveDir = Vector2Int.up; break;    // 위로 이동 (Z+ 증가)
                case 1: moveDir = Vector2Int.down; break;  // 아래로 이동 (Z- 감소)
                case 2: moveDir = Vector2Int.left; break;  // 왼쪽으로 이동 (X- 감소)
                case 3: moveDir = Vector2Int.right; break; // 오른쪽으로 이동 (X+ 증가)
            }

            // 현재 위치에 이동 방향을 더하여 잠재적인 다음 위치를 계산합니다.
            Vector2Int potentialPos = currentPos + moveDir;

            // if문: roomCoordinates 리스트에 potentialPos가 포함되어 있지 않다면 (즉, 빈 공간이라면)
            // !roomCoordinates.Contains(potentialPos)는 해당 좌표에 아직 방이 배치되지 않았음을 확인합니다.
            if (!roomCoordinates.Contains(potentialPos))
            {
                nextPos = potentialPos; // 다음 위치를 확정합니다.
                foundValidPos = true;   // 유효한 위치를 찾았으므로, while 루프를 종료하도록 플래그를 true로 설정합니다.
            }
        }

        return nextPos; // 겹치지 않는 다음 방의 격자 좌표를 반환합니다.
    }

    // 실제 프리팹을 게임 세상에 소환하는 함수 (수정된 간격 변수 적용됨)
    // gridPos: 방의 격자 좌표 (Vector2Int)
    void PlaceRoom(GameObject prefab, Vector2Int gridPos)
    {
        // 격자 좌표(gridPos)와 설정된 간격(roomSpacingX/Z)을 곱하여 실제 월드 좌표(Vector3)를 계산합니다.
        Vector3 worldPos = new Vector3(
            gridPos.x * roomSpacingX, // X축 월드 좌표 = 격자 X * 간격 X
            0,                        // Y축은 0으로 고정 (2D 던전이므로)
            gridPos.y * roomSpacingZ  // Z축 월드 좌표 = 격자 Y * 간격 Z (Unity에서 Y는 보통 Z축으로 사용)
        );

        // Instantiate 함수를 사용하여 prefab을 계산된 worldPos 위치에 기본 회전(Quaternion.identity)으로 생성합니다.
        Instantiate(prefab, worldPos, Quaternion.identity);
    }

    // 리스트를 무작위로 섞는 함수 (Fisher-Yates Shuffle 알고리즘)
    List<T> ShuffleList<T>(List<T> list)
    {
        System.Random random = new System.Random(); // 새로운 난수 생성기 인스턴스를 생성합니다.

        // for문: 리스트의 맨 끝 요소부터 시작하여 (i = list.Count - 1) 두 번째 요소(i > 0)까지 역순으로 순회합니다.
        // 맨 끝 요소는 섞이지 않기 때문에, 첫 번째 요소까지(i > 0) 반복합니다.
        for (int i = list.Count - 1; i > 0; i--)
        {
            // random.Next(i + 1)은 0부터 i까지의 인덱스 중 무작위 인덱스를 선택합니다.
            int randomIndex = random.Next(i + 1);

            // 일반적인 변수 스와핑(값 교환)을 위한 임시 변수입니다.
            T temp = list[i];
            // 현재 요소(list[i])와 무작위 인덱스의 요소(list[randomIndex])의 위치를 바꿉니다.
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list; // 섞인 리스트를 반환합니다.
    }
}