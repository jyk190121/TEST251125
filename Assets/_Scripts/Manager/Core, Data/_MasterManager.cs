using UnityEngine;

public class _MasterManager : MonoBehaviour
{
    public static _MasterManager Instance { get; private set; }

    // ��� �Ŵ���
    public DataManager DataManager { get; private set; }
    public SaveLoadManager SaveLoadManager { get; private set; }
    public DayManager DayManager { get; private set; }
    public UIManager UIManager { get; private set; }
    public InventoryManager InventoryManager { get; private set; }
    public InputManager InputManager { get; private set; }
    public BattleManager BattleManager { get; private set; }
    public DungeonManager DungeonManager { get; private set; }
    public EnemyManager EnemyManager { get; private set; }
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

    //�Ŵ��� �ʱ�ȭ
    void InitializeManagers()
    {
        DataManager = GetComponent<DataManager>();
        SaveLoadManager = GetComponent<SaveLoadManager>();
        DayManager = GetComponent<DayManager>();
        UIManager = GetComponent<UIManager>();
        InputManager = GetComponent<InputManager>();
        BattleManager = GetComponent<BattleManager>();
        DungeonManager = GetComponent<DungeonManager>();
        EnemyManager = GetComponent<EnemyManager>();
        //ResourceManager = GetComponent<ResourceManager>();
        SoundManager = GetComponent<SoundManager>();

        // 1. ������ ���
        //ResourceManager.Initialize();
        DataManager.Initialize();
        SaveLoadManager.Initialize();

        // 2. UI / �Է�
        InputManager.Initialize();
        UIManager.Initialize();

        // 3. ���� / ���� / ��
        EnemyManager.Initialize();
        BattleManager.Initialize();
        //DungeonManager.Initialize();

        // 4. ����/ī�޶�/�ð�
        SoundManager.Initialize();
        DayManager.Initialize();


        Debug.Log("��� �Ŵ��� �ʱ�ȭ �Ϸ�");
    }
}
