using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//전투 관련 데이터 처리(파밍한 아이템, 잡은 몬스터 보관했다가 Result Panel에 띄움)
public class BattleRecord: MonoBehaviour
{
    // 잡은 몬스터 리스트
    List<MonsterData> KilledMonster;

    // 얻은 아이템 리스트
    List<Item> items;

    //결과창 프리팹
    public GameObject resultPanel;

    //프리팹의 구성요소
    //상단의 죽은 이유 텍스트
    public TextMeshProUGUI deadReason;
    public TextMeshProUGUI goToVillage;
    public TextMeshProUGUI retry;
    public TextMeshProUGUI enter;

    //먹은 아이템 갯수, 잡은 몬스터 수
    public TextMeshProUGUI itemCount;
    public TextMeshProUGUI MonsterCount;

    //먹은 아이템, 잡은 몬스터 띄울 위치
    public Transform ItemList;
    public Transform MonsterList;

    //상단 죽은 이유 관련 이미지
    public Image HowImage;

    //상단에 띄워야할 이미지 3가지
    public List<Sprite> sprites;

    //UI 슬롯 프리팹
    public GameObject slotPrefab;
    TextMeshProUGUI slotPrefab_Count;

    // 인벤토리(아이템 목록) 패널
    public GameObject inventoryPrefab;
    public GameObject deleteInventory;

    //켜져있을때 입력값 확인
    bool Key_goToVillage = false;
    bool Key_Retry = false;
    bool Key_Enter = false;

    //사망시 아이템 제거
    bool ifDie = false;


    //사진 찍어 오자...
    [Header("Snapshot Settings")]
    public Camera snapshotCamera;      // 스냅샷 전용 카메라
    public RenderTexture snapshotRT;   // Target Texture에 연결된 RT
    public Transform snapshotPoint;    // 몬스터가 임시로 서 있을 위치 (먼 곳)

    void Start()
    {
        _MasterManager.Instance.DataManager.RegisterBattleRecord(this);
        KilledMonster = new List<MonsterData>();
        items = new List<Item>();
        resultPanel.SetActive(false);
        inventoryPrefab.SetActive(false);
        deleteInventory.SetActive(false);
    }

    private void Update()
    {
        if (Key_goToVillage)
        {
            if (Input.GetKeyDown(KeySetting.keys[KeyInput.CANCLE]))
            {
                if (ifDie)
                {
                    _MasterManager.Instance.InventoryManager.OnPlayerDeath();
                }
                _MasterManager.Instance.DataManager.SetisClear(false);
                _MasterManager.Instance.DataManager.SetisPendant(false);
                _MasterManager.Instance.DungeonManager.ChangeDay();
                GameSceneManager.game.LoadScene("Villiage");
            }
        }
        if (Key_Retry)
        {
            if (Input.GetKeyDown(KeySetting.keys[KeyInput.INTERACTIVE]))
            {
                if (ifDie)
                {
                    _MasterManager.Instance.InventoryManager.OnPlayerDeath();
                }
                _MasterManager.Instance.DataManager.SetisClear(false);
                _MasterManager.Instance.DataManager.SetisPendant(false);
                _MasterManager.Instance.DungeonManager.ChangeDay();
                GameSceneManager.game.ReloadCurrentScene();
            }
        }
        if (Key_Enter)
        {
            if (Input.GetKeyDown(KeySetting.keys[KeyInput.INTERACTIVE]))
            {
                if (_MasterManager.Instance.DataManager.dungeonCleared == 1)
                {
                    _MasterManager.Instance.DataManager.SetisClear(false);
                    _MasterManager.Instance.DataManager.SetisPendant(false);
                    _MasterManager.Instance.DataManager.dungeonCleared = 0;
                    GameSceneManager.game.LoadScene("Dungeon2Scene");
                }
                else return;
            }
        }

    }

    // 리스트에 몬스터 추가
    public void AddMonster(MonsterData monster)
    {
        KilledMonster.Add(monster);
    }

    public void AddItem(Item item)
    {
        items.Add(item);
    }

    // 몬스터 프리팹에서 이미지를 추출하는 간단한 원리
    private Sprite CaptureSprite(GameObject prefab)
    {
        // 1. 카메라 앞에 몬스터 임시 생성
        GameObject tempMob = Instantiate(prefab, snapshotPoint.position + new Vector3(0,-1.3f,-3f), Quaternion.identity);

        // 2. 카메라 렌더링 수동 실행
        snapshotCamera.Render();

        // 3. Render Texture의 픽셀 정보를 Texture2D로 복사
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = snapshotRT;

        Texture2D tex = new Texture2D(snapshotRT.width, snapshotRT.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, snapshotRT.width, snapshotRT.height), 0, 0);
        tex.Apply();

        RenderTexture.active = currentRT;
        
        // 4. 사용한 임시 몬스터 파괴
        //Destroy(tempMob);

        // 5. Texture2D를 UI에서 쓸 수 있는 Sprite로 변환
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }


    public void OpenResultPanel(bool Clear, bool Pendent)
    {
        _MasterManager.Instance.DungeonManager.OffMiniMap();

        //결과창 케이스 별 분리
        resultPanel.SetActive(true);            // 평소엔 꺼놨다가 키기
        inventoryPrefab.SetActive(true);
        retry.gameObject.SetActive(false);      // retry는 사망 시에만
        enter.gameObject.SetActive(false);      // enter는 클리어일때
        Key_goToVillage = true;
        
        if (Pendent)
        {
            deadReason.text = "펜던트로 탈출함";
            HowImage.sprite = sprites[0];
        }
        else if (Clear)
        {
            _MasterManager.Instance.DataManager.DungeonClear(1);
            enter.gameObject.SetActive(true);
            deadReason.text = "던전 클리어 후 복귀";
            enter.text = $"{KeySetting.keys[KeyInput.INTERACTIVE]}   2층 입장";
            HowImage.sprite = sprites[1];
            Key_Enter = true;
        }
        else
        {
            retry.gameObject.SetActive(true);
            deleteInventory.SetActive(true);    //플레이어 사망시 Delete된 아이템에 사선 표시
            deadReason.text = "사고로 사망";
            retry.text = $"{KeySetting.keys[KeyInput.INTERACTIVE]}   다시 플레이";
            HowImage.sprite = sprites[2];
            Key_Retry = true;
            ifDie = true;
        }

        goToVillage.text = $"{KeySetting.keys[KeyInput.CANCLE]}   마을로 가기";
        
        if(items == null)
        {
            itemCount.text = "0";
        }
        else
        {
            itemCount.text = items.Count.ToString();
        }
        if (KilledMonster == null)
        {
            MonsterCount.text = "0";
        }
        else
        {
            MonsterCount.text = KilledMonster.Count.ToString();
        }

        // 기존에 생성된 아이템/몬스터 UI 청소 (이미 생성된 게 있을 수 있으므로)
        foreach (Transform child in ItemList) Destroy(child.gameObject);
        foreach (Transform child in MonsterList) Destroy(child.gameObject);

        List<ResultItemCount> RIC = new List<ResultItemCount>();

        // 획득한 아이템 표시
        foreach (var item in items)
        {
            //리스트에 같은 값이 있는가?
            var found = RIC.Find(x => x.item.itemID == item.itemID);

            //이미 있음
            if(found != null)
            {
                found.count++;
            }
            //없을
            else
            {
                RIC.Add(new ResultItemCount(item, 1));
            }
        }

        foreach (var itemdata in RIC)
        {
            GameObject obj = Instantiate(slotPrefab, ItemList, false);

            // 2. 슬롯의 이미지 컴포넌트를 가져와서 아이템 아이콘으로 변경
            Image iconImage = obj.GetComponent<Image>();
            if (iconImage != null && itemdata.item.icon != null)
            {
                iconImage.sprite = itemdata.item.icon;
            }
            slotPrefab_Count = obj.GetComponentInChildren<TextMeshProUGUI>();
            slotPrefab_Count.text = $"X{itemdata.count.ToString()}";
        }

        // 잡은 몬스터 표시 (스냅샷 찍어오기..)
        foreach (var monster in KilledMonster)
        {
            if (monster.MobPrefab == null) continue;
            
            // 실시간으로 몬스터 사진 찍기
            Sprite snapshot = CaptureSprite(monster.MobPrefab);

            // UI 슬롯 생성 및 사진 넣기
            GameObject obj = Instantiate(slotPrefab, MonsterList, false);
            Image slotImage = obj.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.sprite = snapshot;
            }
            
        }


        _MasterManager.Instance.DataManager.SetisPendant(false);
        _MasterManager.Instance.DataManager.SetisClear(false);
    }
}

// 1. 데이터 구조 클래스
public class ResultItemCount
{
    public Item item;
    public int count;

    public ResultItemCount(Item item, int count)
    {
        this.item = item;
        this.count = count;
    }
}

////키를 눌렀는가? -> 오래 눌러야 되는 방식인데 불편함...
//if(Key_goToVillage)
//{
//    if (Input.GetKey(KeySetting.keys[KeyInput.CANCLE]))
//    {
//        pressVillage =  true;
//    }
//    else
//    {
//        pressVillage = false;
//    }
//}
//if(Key_Retry)
//{
//    if (Input.GetKey(KeySetting.keys[KeyInput.INTERACTIVE]))
//    {
//        pressRetry = true;
//    }
//    else
//    {
//        pressRetry = false;
//    }
//}

//if (pressVillage)
//{
//    goToVillageTime -= Time.deltaTime;
//    if (goToVillageTime <= 0f)
//    {
//        GameSceneManager.game.LoadScene("Villiage");
//        Time.timeScale = 1f;
//    }

//}
//else if (!pressVillage || goToVillageTime != 2f)
//{
//    goToVillageTime = 2f;
//}
//else if (pressRetry)
//{
//    RetryTime -= Time.deltaTime;
//    if (RetryTime <= 0f)
//    {
//        GameSceneManager.game.ReloadCurrentScene();
//        Time.timeScale = 1f;
//    }
//}
//else if (!pressRetry || RetryTime != 2f)
//{
//    RetryTime = 2f;
//}

//