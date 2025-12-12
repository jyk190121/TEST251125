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

    public GameObject Player;
    public Vector3 PlayerPos = new Vector3(0f, 0f, 0f);

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
    void SpawnRooms(Vector2Int bossPos, Vector2Int restPos)
    {
        spawnedRooms.Clear();

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

    public void Initialize()
    {

    }
}
