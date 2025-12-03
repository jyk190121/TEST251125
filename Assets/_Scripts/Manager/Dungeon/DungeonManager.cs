using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [Header("=== Room Prefabs ===")]
    public GameObject startRoomPrefab;
    public GameObject normalRoomPrefab;
    public GameObject restRoomPrefab;
    public GameObject bossRoomPrefab;

    [Header("=== Settings ===")]
    [Tooltip("생성할 최소 방 개수")]
    public int minRooms = 8;

    [Tooltip("생성할 최대 방 개수")]
    public int maxRooms = 10;

    [Header("--- Spacing Settings ---")]
    [Tooltip("방 오브젝트의 가로 길이(X축)에 맞춰 설정하세요.")]
    public float roomSpacingX = 20.0f;

    [Tooltip("방 오브젝트의 세로 길이(Z축)에 맞춰 설정하세요.")]
    public float roomSpacingZ = 15.0f;

    // 랜덤 생성 좌표 데이터
    private List<Vector2Int> roomPositions = new();
    private Dictionary<Vector2Int, GameObject> spawnedRooms = new();

    private readonly Vector2Int[] dirs = new Vector2Int[]
    {
        new Vector2Int(1,0),   // Right
        new Vector2Int(-1,0),  // Left
        new Vector2Int(0,1),   // Up
        new Vector2Int(0,-1)   // Down
    };

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        CreateRoomPositions();
        var (bossPos, restPos, parents) = BFSFindSpecialRooms();
        SpawnRooms(bossPos, restPos);
        SetupDoors();
    }

    // ------------------------------------
    // 1. 방 위치 랜덤 생성 (트리 구조)
    // ------------------------------------
    void CreateRoomPositions()
    {
        roomPositions.Clear();
        roomPositions.Add(Vector2Int.zero);  // 시작방

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
    // 2. BFS로 보스방 & 쉬는방 찾기
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

        // 가장 먼 방 = 보스방
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

        // 보스방 바로 앞 방 = 쉬는방
        Vector2Int restPos = parent[bossPos];

        return (bossPos, restPos, parent);
    }

    // ------------------------------------
    // 3. 방 프리팹 생성 (X, Z 배치)
    // ------------------------------------
    void SpawnRooms(Vector2Int bossPos, Vector2Int restPos)
    {
        spawnedRooms.Clear();

        foreach (var pos in roomPositions)
        {
            GameObject prefabToUse;

            if (pos == Vector2Int.zero)
                prefabToUse = startRoomPrefab;
            else if (pos == bossPos)
                prefabToUse = bossRoomPrefab;
            else if (pos == restPos)
                prefabToUse = restRoomPrefab;
            else
                prefabToUse = normalRoomPrefab;

            // X, Z 간격을 따로 적용
            Vector3 worldPos = new Vector3(
                pos.x * roomSpacingX,
                0f,
                pos.y * roomSpacingZ
            );

            GameObject room = Instantiate(prefabToUse, worldPos, Quaternion.identity);
            spawnedRooms[pos] = room;
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

            RoomDoorController doors = room.GetComponent<RoomDoorController>();

            if (doors == null)
            {
                Debug.LogWarning($"Room prefab '{room.name}'에 RoomDoorController가 없습니다.");
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
