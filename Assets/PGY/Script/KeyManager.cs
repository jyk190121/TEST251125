using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//단축키를 세팅해주는 스크립트

public enum KeyInput
{
    UP,             
    DOWN,
    LEFT,
    RIGHT,
    MAINATTACK,
    SUBATTACK,
    ROLL,
    TouchNPC,
    PENDANT,
    SWITCHWEAPON,
    OPTION,
    KEYCOUNT
     
}

public static class KeySetting
{
    public static Dictionary<KeyInput, KeyCode> keys = new Dictionary<KeyInput, KeyCode>();
}

public class KeyManager : MonoBehaviour
{
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
        KeyCode.Escape
    };

    private void Awake()
    {
        KeySetting.keys.Clear();        //기존에 키 딕셔너리 청소
        for(int i = 0; i< (int)KeyInput.KEYCOUNT; i++)
        {
            KeySetting.keys.Add((KeyInput)i, defaultKeys[i]);       //키 딕셔너리에 키값과 Value값 추가
        }
    }

    int waitingKey = -1;

    private void Update()
    {
        if(waitingKey != -1)
        {
            DetectNewKey();
        }
    }
    public void ChangeKey(int num)      //키 변경 선택시 호출할 함수 -> DectectNewKey함수를 계속 호출하기 위한 장치, int num => 변경할 키의 key값 
    {                                  
        waitingKey = num;
    }

    void DetectNewKey()         //키변경 값을 입력받아 넣는 함수
    {
        foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))        //유니티가 가진 모든 키코드를 확인
        {
            if (Input.GetKeyDown(code))                                         //키 입력을 받았을 때 그 키가 유니티 키코드 내에 유효하다!
            {
                KeySetting.keys[(KeyInput)waitingKey] = code;                   //기존 키 값을 새로 누른 키 값으로 교체한다.
                waitingKey = -1;                                                //DetectNewKey는 다시 비활성화
                break;
            }
        }
    }
}
