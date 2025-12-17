using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
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
    public TextMeshProUGUI deadReason;
    public TextMeshProUGUI goToVillage;
    public TextMeshProUGUI retry;

    public TextMeshProUGUI itemCount;
    public TextMeshProUGUI MonsterCount;

    public Transform ItemList;
    public Transform MonsterList;

    public Image HowImage;

    //상단에 띄워야할 이미지 3가지
    public List<Sprite> sprites;
    

    void Start()
    {
        _MasterManager.Instance.DataManager.RegisterBattleRecord(this);
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

    public void OpenResultPanel(bool Clear, bool Pendent)
    {
        //결과창 케이스 별 분리
        resultPanel.SetActive(true);            // 평소엔 꺼놨다가 키기
        retry.gameObject.SetActive(false);      // retry는 사망 시에만
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
        }

        goToVillage.text = $"{KeySetting.keys[KeyInput.CANCLE]}   마을로 가기";
        
        itemCount.text = items.Count.ToString();
        MonsterCount.text = KilledMonster.Count.ToString();

        // 획득한 아이템 표시
        foreach(var item in items)
        {
            GameObject obj = item.itemPrefab;
            Instantiate(obj, ItemList);
        }

        // 잡은 몬스터 표시
        foreach(var monster in KilledMonster)
        {
            GameObject obj = monster.MobPrefab;
            Instantiate(obj, MonsterList);
        }

        _MasterManager.Instance.DataManager.SetisClear(false);
        _MasterManager.Instance.DataManager.SetisClear(false);
    }
}
