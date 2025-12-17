using UnityEngine;

public class _MasterManager : MonoBehaviour
{
    public static _MasterManager Instance { get; private set; }

    // ��� �Ŵ���
    public DataManager DataManager { get; private set; }
    public SaveManager SaveManager { get; private set; }
    public DayManager DayManager { get; private set; }
    public UIManager UIManager { get; private set; }
    public InventoryManager InventoryManager { get; private set; }
    public InputManager InputManager { get; private set; }
    public DungeonManager DungeonManager { get; private set; }
    public EnemyManager EnemyManager { get; private set; }
    public ResourceManager ResourceManager { get; private set; }
    public SoundManager SoundManager { get; private set; }
    public GameSceneManager GameSceneManager { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeManagers();
    }

    //�Ŵ��� �ʱ�ȭ
    void InitializeManagers()
    {
        DataManager = GetComponent<DataManager>();
        SaveManager = GetComponent<SaveManager>();
        DayManager = GetComponent<DayManager>();
        UIManager = GetComponent<UIManager>();
        InputManager = GetComponent<InputManager>();
        //DungeonManager = GetComponent<DungeonManager>();
        EnemyManager = GetComponent<EnemyManager>();
        ResourceManager = GetComponent<ResourceManager>();
        InventoryManager = GetComponent<InventoryManager>();
        SoundManager = GetComponent<SoundManager>();
        GameSceneManager = GetComponent<GameSceneManager>();

        // 1. 리소스 및 기본 데이터
        ResourceManager.Initialize();
        DataManager.Initialize();
        SaveManager.Initialize();

        // 2. UI / 입력
        InputManager.Initialize();
        UIManager.Initialize();
        InventoryManager.Initialize();

        // 3. 게임 데이터 로드
        if (SaveManager.HasSaveData())
        {
            SaveManager.LoadGame(DataManager);
            Debug.Log("[MasterManager] 저장된 게임 데이터 로드됨");
        }

        // 4. 전투 / 적 / 던전
        EnemyManager.Initialize();
        //DungeonManager.Initialize();

        // 5. 사운드 / 카메라 / 시간
        SoundManager.Initialize();
        DayManager.Initialize();

        Debug.Log("게임 매니저 초기화 완료");
    }
}
