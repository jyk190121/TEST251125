using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

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
    TextMeshProUGUI deadReason;
    TextMeshProUGUI goToVillage;
    TextMeshProUGUI retry;

    TextMeshProUGUI itemCount;
    TextMeshProUGUI MonsterCount;

    

    void Start()
    {
        _MasterManager.Instance.DataManager.RegisterBattleRecord(this);
    }

    void Update()
    {
        
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
        if (Pendent)
        {
            deadReason.text = "펜던트로 탈출함";
        }
        else if (Clear)
        {
            deadReason.text = "던전 클리어 후 복귀";
        }
        else
        {
            deadReason.text = "사고로 사망";
        }

        goToVillage.text = $"{KeySetting.keys[KeyInput.CANCLE]}   마을로 가기";
        retry.text = $"";
    }
}
