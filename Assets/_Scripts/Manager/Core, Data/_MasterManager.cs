using UnityEngine;

public class _MasterManager : MonoBehaviour
{
    public static _MasterManager Instance { get; private set; }

    // 모든 매니저
    public DataManager DataManager { get; private set; }
    public SaveLoadManager SaveLoadManager { get; private set; }
    public DayManager DayManager { get; private set; }
    public UIManager UIManager { get; private set; }
    public InventoryManager InventoryManager { get; private set; }
    public InputManager InputManager { get; private set; }
    public BattleManager BattleManager { get; private set; }
    public DungeonManager DungeonManager { get; private set; }
    public EnemyManager EnemyManager { get; private set; }
    public CameraManager CameraManager { get; private set; }
    public ResourceManager ResourceManager { get; private set; }
    public SoundManager SoundManager { get; private set; }

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

    //매니저 초기화
    void InitializeManagers()
    {
        DataManager = GetComponent<DataManager>();
        SaveLoadManager = GetComponent<SaveLoadManager>();
        DayManager = GetComponent<DayManager>();
        UIManager = GetComponent<UIManager>();
        InventoryManager = GetComponent<InventoryManager>();
        InputManager = GetComponent<InputManager>();
        BattleManager = GetComponent<BattleManager>();
        DungeonManager = GetComponent<DungeonManager>();
        EnemyManager = GetComponent<EnemyManager>();
        CameraManager = GetComponent<CameraManager>();
        ResourceManager = GetComponent<ResourceManager>();
        SoundManager = GetComponent<SoundManager>();

        // 1. 데이터 기반
        ResourceManager.Initialize();
        DataManager.Initialize();
        SaveLoadManager.Initialize();

        // 2. UI / 입력
        InputManager.Initialize();
        UIManager.Initialize();
        InventoryManager.Initialize();

        // 3. 전투 / 던전 / 적
        EnemyManager.Initialize();
        BattleManager.Initialize();
        DungeonManager.Initialize();

        // 4. 사운드/카메라/시간
        SoundManager.Initialize();
        CameraManager.Initialize();
        DayManager.Initialize();


        Debug.Log("모든 매니저 초기화 완료");
    }
}
