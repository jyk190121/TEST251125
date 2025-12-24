using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TMPro;

//단축키를 세팅해주는 스크립트

public enum KeyInput
{
    UP,             //위로
    DOWN,           //아래로
    LEFT,           //왼쪽
    RIGHT,          //오른쪽
    MAINATTACK,     //일반공격
    SUBATTACK,      //특수공격
    ROLL,           //구르기
    INTERACTIVE,    //상호작용
    PENDANT,        //펜던트 사용
    SWITCHWEAPON,   //무기 교체
    OPTION,         //옵션 열기
    INVENTORY,      //인벤토리 열기
    QUICKSLOT,      //퀵슬롯 아이템 사용
    CANCLE,         //취소
    KEYCOUNT
}

public static class KeySetting
{
    public static Dictionary<KeyInput, KeyCode> keys = new Dictionary<KeyInput, KeyCode>();

    public static string GetKeyString(KeyInput keyInput)
    {
        return keys[keyInput].ToString();  // "W", "Space", "I" 같은 스트링 반환
    }
}

public class InputManager : MonoBehaviour
{
    //미리 배열을 한번만 만들어둬서 이후에 새로 호출x
    KeyCode[] allKeys;

    //키 변경 중복 실행 방지
    bool isRebinding = false;

    //KeyCode EnumType
    KeyCode[] defaultKeys = new KeyCode[]
    {
        KeyCode.W,      //위로
        KeyCode.S,      //아래로
        KeyCode.A,      //왼쪽
        KeyCode.D,      //오른쪽
        KeyCode.J,      //일반공격
        KeyCode.K,      //특수공격
        KeyCode.Space,  //구르기
        KeyCode.G,      //상호작용
        KeyCode.B,      //펜던트 사용
        KeyCode.T,      //무기 교체
        KeyCode.Escape, //옵션 열기
        KeyCode.I,      //인벤토리 열기
        KeyCode.E,      //퀵슬롯 사용
        KeyCode.C
    };

    private void Awake()
    {
        KeySetting.keys.Clear();        //기존에 키 딕셔너리 청소
        for(int i = 0; i< (int)KeyInput.KEYCOUNT; i++)
        {
            KeySetting.keys.Add((KeyInput)i, defaultKeys[i]);       //키 딕셔너리에 키값과 Value값 추가
        }
        
        allKeys = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
    }

    public async void ChangeKey(int num)   
    {
        if (isRebinding) return;

        isRebinding = true;
        Debug.Log("키 입력 받아야함!");

        //1. 키 입력을 받을 때까지 대기
        KeyCode pressedKey = await WaitForKeyPress();

        //2. 중복체크
        if (KeySetting.keys.ContainsValue(pressedKey))
        {
            Debug.Log("이미 지정된 키입니다");
        }
        else
        {
            //3. 키 변경 적용
            KeySetting.keys[(KeyInput)num] = pressedKey;
            Debug.Log($"{(KeyInput)num} 키가 {pressedKey}로 변경되었습니다.");
        }

        isRebinding = false;

    }

    private async Task<KeyCode> WaitForKeyPress()
    {
        while (true)
        {
            if (Input.anyKeyDown)
            {
                for (int i = 0; i < allKeys.Length; i++)
                {
                    // 실제 눌린 키를 찾아 즉시 반환하며 비동기 종료
                    if (Input.GetKeyDown(allKeys[i])) return allKeys[i];
                }
            }
            // CPU 점유를 막고 다음 프레임까지 양보, 아니면 이 함수가 다먹음
            await Task.Yield();
        }
    }

    public void Initialize()
    {
    }
}
