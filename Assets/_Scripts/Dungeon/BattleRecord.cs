using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Threading;
using Unity.VisualScripting;

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

    //켜져있을때 입력값 확인
    bool Key_goToVillage = false;
    bool Key_Retry = false;


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
        
    }

    private void Update()
    {
        if (Key_goToVillage)
        {
            if (Input.GetKeyDown(KeySetting.keys[KeyInput.CANCLE]))
            {
                _MasterManager.Instance.DataManager.SetisClear(false);
                _MasterManager.Instance.DataManager.SetisPendant(false);
                GameSceneManager.game.LoadScene("Villiage");
            }
        }
        if (Key_Retry)
        {
            if (Input.GetKeyDown(KeySetting.keys[KeyInput.INTERACTIVE]))
            {
                _MasterManager.Instance.DataManager.SetisClear(false);
                _MasterManager.Instance.DataManager.SetisPendant(false);
                GameSceneManager.game.ReloadCurrentScene();
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
        //결과창 케이스 별 분리
        resultPanel.SetActive(true);            // 평소엔 꺼놨다가 키기
        retry.gameObject.SetActive(false);      // retry는 사망 시에만
        Key_goToVillage = true;
        if (Pendent)
        {
            deadReason.text = "펜던트로 탈출함";
            HowImage.sprite = sprites[0];
        }
        else if (Clear)
        {
            deadReason.text = "던전 클리어 후 복귀";
            HowImage.sprite = sprites[1];
        }
        else
        {
            retry.gameObject.SetActive(true);
            deadReason.text = "사고로 사망";
            retry.text = $"{KeySetting.keys[KeyInput.INTERACTIVE]}   다시 플레이";
            HowImage.sprite = sprites[2];
            Key_Retry = true;
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

        // 획득한 아이템 표시
        foreach (var item in items)
        {
            // 1. 슬롯 프리팹 생성 (ItemList의 자식으로)
            GameObject obj = Instantiate(slotPrefab, ItemList, false);

            // 2. 슬롯의 이미지 컴포넌트를 가져와서 아이템 아이콘으로 변경
            Image iconImage = obj.GetComponent<Image>();
            if (iconImage != null && item.icon != null)
            {
                iconImage.sprite = item.icon;
            }
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