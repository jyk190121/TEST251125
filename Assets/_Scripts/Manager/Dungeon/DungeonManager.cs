using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public enum RoomType
{
    Start,
    Normal,
    Trap,
    Rest,
    Boss
}

[System.Serializable]
public class RoomPrefabData
{
    public RoomType roomType;     // 방 타입
    public GameObject prefab;     // 해당 타입의 프리팹
}

public class DungeonManager : MonoBehaviour
{
    [Header("=== Room Prefabs ===")]
    [Tooltip("여기에 RoomType - Prefab 세트를 추가하세요.")]
    public List<RoomPrefabData> roomPrefabList = new();

    
    private Dictionary<RoomType, GameObject> prefabDictionary;

    [Header("=== Settings ===")]
    public int minRooms = 8;
    public int maxRooms = 10;

    [Header("--- Spacing Settings ---")]
    public float roomSpacingX = 20.0f;
    public float roomSpacingZ = 15.0f;

    private List<Vector2Int> roomPositions = new();
    private Dictionary<Vector2Int, GameObject> spawnedRooms = new();

    public CinemachineCamera bossCam;
    public Vector3 bossCamOffset = new Vector3(0f, 5f, -10f);

    public GameObject player;
    public Vector3 playerPos = new Vector3(0f, 0f, 0f);

    private readonly Vector2Int[] dirs = new Vector2Int[]
    {
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(0,1),
        new Vector2Int(0,-1)
    };

    void Start()
    {
        // Enum + Prefab 매핑
        prefabDictionary = new Dictionary<RoomType, GameObject>();
        foreach (var data in roomPrefabList)
        {
            prefabDictionary[data.roomType] = data.prefab;
        }

        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        CreateRoomPositions();
        var (bossPos, restPos, parents) = BFSFindSpecialRooms();
        SpawnRooms(bossPos, restPos);
        SetupDoors();

        SetupBossCameraPosition(bossPos);

        SpawnPlayer();
    }

    void SetupBossCameraPosition(Vector2Int bossGridPos)
    {
        Vector3 bossCenterWorldPos = new Vector3(
            bossGridPos.x * roomSpacingX,
            // X 좌표: 그리드 X 위치에 방 간격 X를 곱하여 월드 X 위치를 계산합니다.
            0f,
            // Y 좌표: 던전 맵이 X-Z 평면이므로 0으로 고정합니다.
            bossGridPos.y * roomSpacingZ
        // Z 좌표: 그리드 Y 위치에 방 간격 Z를 곱하여 월드 Z 위치를 계산합니다.
        );

        Vector3 cameraPosition = bossCenterWorldPos + bossCamOffset;

        bossCam.transform.position = cameraPosition;

        bossCam.transform.rotation = Quaternion.LookRotation(bossCenterWorldPos - cameraPosition);

        bossCam.Priority = 0;
    }

    // ------------------------------------
    // 1. 랜덤 방 좌표 생성
    // ------------------------------------
    void CreateRoomPositions()
    {
        roomPositions.Clear();
        roomPositions.Add(Vector2Int.zero);  // 시작 방

        int target = Random.Range(minRooms, maxRooms + 1);

        while (roomPositions.Count < target)
        {
            Vector2Int basePos = roomPositions[Random.Range(0, roomPositions.Count)];
            Vector2Int newPos = basePos + dirs[Random.Range(0, dirs.Length)];

            if (!roomPositions.Contains(newPos))
                roomPositions.Add(newPos);
        }
    }

    // ------------------------------------
    // 2. BFS로 보스방, 쉬는방 결정
    // ------------------------------------
    (Vector2Int bossPos, Vector2Int restPos, Dictionary<Vector2Int, Vector2Int> parents)
    BFSFindSpecialRooms()
    {
        Dictionary<Vector2Int, int> dist = new();
        Dictionary<Vector2Int, Vector2Int> parent = new();
        Queue<Vector2Int> q = new();

        Vector2Int start = Vector2Int.zero;

        q.Enqueue(start);
        dist[start] = 0;
        parent[start] = start;

        while (q.Count > 0)
        {
            Vector2Int cur = q.Dequeue();

            foreach (var dir in dirs)
            {
                Vector2Int next = cur + dir;

                if (roomPositions.Contains(next) && !dist.ContainsKey(next))
                {
                    dist[next] = dist[cur] + 1;
                    parent[next] = cur;
                    q.Enqueue(next);
                }
            }
        }

        // 가장 먼 곳 = 보스방
        Vector2Int bossPos = start;
        int maxDist = 0;

        foreach (var kv in dist)
        {
            if (kv.Value > maxDist)
            {
                maxDist = kv.Value;
                bossPos = kv.Key;
            }
        }

        // 보스방 바로 이전 = 쉬는방
        Vector2Int restPos = parent[bossPos];

        return (bossPos, restPos, parent);
    }

    // ------------------------------------
    //  방 타입 결정 함수 (여기서 확장 가능)
    // ------------------------------------
    RoomType GetRoomType(Vector2Int pos, Vector2Int bossPos, Vector2Int restPos)
    {
        if (pos == Vector2Int.zero)
            return RoomType.Start;

        if (pos == bossPos)
            return RoomType.Boss;

        if (pos == restPos)
            return RoomType.Rest;

        // 확률로 트랩방 생성 (원하면 조절 가능)
        if (Random.value < 0.15f)
            return RoomType.Trap;

        return RoomType.Normal;
    }

    // ------------------------------------
    // 3. 프리팹 생성
    // ------------------------------------

    private Vector2Int startRoomGridPos = Vector2Int.zero;

    void SpawnRooms(Vector2Int bossPos, Vector2Int restPos)
    {
        spawnedRooms.Clear();

        startRoomGridPos = Vector2Int.zero;

        foreach (var pos in roomPositions)
        {
            RoomType type = GetRoomType(pos, bossPos, restPos);

            if (!prefabDictionary.ContainsKey(type))
            {
                Debug.LogError($"프리팹 Dictionary에 '{type}' 타입이 등록되지 않음!");
                continue;
            }

            GameObject prefabToUse = prefabDictionary[type];

            Vector3 worldPos = new Vector3(
                pos.x * roomSpacingX,
                0f,
                pos.y * roomSpacingZ
            );

            GameObject room = Instantiate(prefabToUse, worldPos, Quaternion.identity);
            spawnedRooms[pos] = room;

            RoomController controller = room.GetComponent<RoomController>();
            if (controller != null)
            {
                controller.isStartRoom = (type == RoomType.Start);
                controller.isRestRoom = (type == RoomType.Rest);
                controller.isBossRoom = (type == RoomType.Boss);
            }
        }
    }

    // ------------------------------------
    // 4. 문 활성화
    // ------------------------------------
    void SetupDoors()
    {
        foreach (var kv in spawnedRooms)
        {
            Vector2Int pos = kv.Key;
            GameObject room = kv.Value;

            RoomController doors = room.GetComponent<RoomController>();

            if (doors == null)
            {
                Debug.LogWarning($"Room prefab '{room.name}'에 RoomController가 없습니다.");
                continue;
            }

            doors.SetDoorActive(
                up: spawnedRooms.ContainsKey(pos + Vector2Int.up),
                down: spawnedRooms.ContainsKey(pos + Vector2Int.down),
                left: spawnedRooms.ContainsKey(pos + Vector2Int.left),
                right: spawnedRooms.ContainsKey(pos + Vector2Int.right)
            );
        }
    }

    void SpawnPlayer()
    {
        // 1. 시작 방의 그리드 위치(Vector2Int.zero)를 월드 좌표로 변환합니다.
        Vector3 startRoomCenterWorldPos = new Vector3(
            // X 월드 좌표: 시작 방 그리드 X (0) * 방 간격 X
            startRoomGridPos.x * roomSpacingX,
            // Y 월드 좌표: 던전은 X-Z 평면이므로 기본 높이는 0f
            0f,
            // Z 월드 좌표: 시작 방 그리드 Y (0) * 방 간격 Z
            startRoomGridPos.y * roomSpacingZ
        );

        // 2. 플레이어의 최종 스폰 월드 위치를 계산합니다.
        // 시작 방의 중심 위치(`startRoomCenterWorldPos`)에,
        // 미리 설정된 플레이어의 상대적 위치(`PlayerPos`)를 더하여 최종 위치를 결정합니다.
        // 현재 `PlayerPos`는 (0, 0, 0)이므로, 결과적으로 시작 방의 중앙이 됩니다.
        Vector3 finalSpawnPosition = startRoomCenterWorldPos + playerPos;

        // 3. 플레이어 오브젝트(`Player` 프리팹)가 설정되어 있는지 확인합니다.
        // if문은 `Player` 변수가 null이 아닌지 (즉, 프리팹이 할당되었는지) 검사하여, 
        // 프리팹이 없을 경우 Instatiate를 시도하여 발생하는 에러(Null Reference Exception)를 방지합니다.
        if (player != null)
        {
            // Instantiate 함수는 플레이어 프리팹을 씬에 복사(생성)하고, 
            // `finalSpawnPosition` 위치에 `Quaternion.identity` (회전 없음)로 배치합니다.
            // 생성된 플레이어 오브젝트의 인스턴스를 `spawnedPlayer` 변수에 저장합니다.
            GameObject spawnedPlayer = Instantiate(
                player,
                finalSpawnPosition,
                Quaternion.identity
            );

            // 생성된 플레이어의 이름을 설정하여 씬에서 쉽게 식별하도록 합니다.
            spawnedPlayer.name = "Player";

            // 플레이어 스폰 위치를 확인하기 위한 로그입니다.
            Debug.Log($"플레이어 프리팹을 시작 방 (World: {finalSpawnPosition})에 성공적으로 생성했습니다.");
        }
        // else문은 `Player` 변수에 프리팹이 할당되지 않았을 경우 실행됩니다.
        else
        {
            Debug.LogError("Player GameObject (프리팹)가 DungeonManager에 할당되지 않았습니다. 인스턴스화 할 수 없습니다.");
        }
    }

    public void Initialize()
    {

    }
}
