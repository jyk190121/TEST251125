using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;


public class InputSetting : MonoBehaviour
{
    [Header("기존 인풋 키 UI")]
    public TextMeshProUGUI currentUP;
    public TextMeshProUGUI currentDOWN;
    public TextMeshProUGUI currentLEFT;
    public TextMeshProUGUI currentRIGHT;

    [Header("새로운 인풋 키 UI")]
    public TMP_InputField inputUP;
    public TMP_InputField inputDOWN;
    public TMP_InputField inputLEFT;
    public TMP_InputField inputRIGHT;

    private Action<string> onChange;

    private void Awake()
    {
        //이벤트 연결
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
