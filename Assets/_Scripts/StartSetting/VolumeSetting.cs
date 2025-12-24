using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    public TextMeshProUGUI masterText;
    public TextMeshProUGUI bgmText;
    public TextMeshProUGUI sfxText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //슬라이더 초기화
        masterSlider.value = SoundManager.Instance.masterVolume;
        bgmSlider.value = SoundManager.Instance.bgmVolume;
        sfxSlider.value = SoundManager.Instance.sfxVolume;
        UpdateTexts(); // 텍스트도 초기화

        //Fill 상태 초기화
        if (masterSlider.fillRect != null) masterSlider.fillRect.gameObject.SetActive(masterSlider.value > 0);
        if (bgmSlider.fillRect != null) bgmSlider.fillRect.gameObject.SetActive(bgmSlider.value > 0);
        if (sfxSlider.fillRect != null) sfxSlider.fillRect.gameObject.SetActive(sfxSlider.value > 0);

        //이벤트 연결
        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        bgmSlider.onValueChanged.AddListener(OnBGMChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);

    }

    void OnMasterChanged(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);
        if (masterSlider.fillRect != null)
        {
            //값이 0보다 크면 Fill을 활성화하고, 그렇지 않으면 비활성화
            masterSlider.fillRect.gameObject.SetActive(value > 0);
        }
        UpdateTexts();
    }

    void OnBGMChanged(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
        if(bgmSlider.fillRect != null)
        {
            bgmSlider.fillRect.gameObject.SetActive(value > 0);
        }
        UpdateTexts();
    }

    void OnSFXChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
        if (sfxSlider.fillRect != null)
        {
            sfxSlider.fillRect.gameObject.SetActive(value > 0);
        }
        UpdateTexts();
    }

    void UpdateTexts()
    {
        masterText.text = $"Master: {(masterSlider.value * 100):F0}%";
        bgmText.text = $"BGM: {(bgmSlider.value * 100):F0}%";
        sfxText.text = $"SFX: {(sfxSlider.value * 100):F0}%";
    }
}
