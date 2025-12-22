using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof (SoundManager))]
public class SoundManager : MonoBehaviour
{
    // 사운드 옵션 ON/OFF
    [Header("사운드 UI")]
    public GameObject soundPanel;

    // 사운드를 재생할 오디오 소스 컴포넌트 (MP3 플레이어 역할)
    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    // 사운드 파일 자체 (Mp3 파일 등)
    [Header("Audio Clips")]

    [Header("진영")]
    public AudioClip[] BGMClips1;
    public AudioClip[] SFXClips1;

    [Header("시우")]
    public AudioClip[] BGMClips2;
    public AudioClip[] SFXClips2;

    [Header("영찬")]
    public AudioClip[] BGMClips3;
    public AudioClip[] SFXClips3;

    [Header("현수")]
    public AudioClip[] BGMClips4;
    public AudioClip[] SFXClips4;

    [Header("건영")]
    public AudioClip[] BGMClips5;
    public AudioClip[] SFXClips5;

    [Header("유정")]
    public AudioClip[] BGMClips6;
    public AudioClip[] SFXClips6;


    //public AudioClip[] villiageBGMClips;
    //public AudioClip[] shopBGMClips;
    //public AudioClip[] dungeonBGMClips;

    [Header("볼륨 설정")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;      // 마스터 볼륨
    [Range(0f, 1f)]
    public float bgmVolume = 0.5f;       // BGM 볼륨
    [Range(0f, 1f)]
    public float sfxVolume = 0.5f;       // SFX 볼륨

    Dictionary<string, AudioClip[]> bgmDict = new Dictionary<string, AudioClip[]>();
    Dictionary<string, AudioClip[]> sfxDict = new Dictionary<string, AudioClip[]>();

    public static SoundManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        //사운드 옵션 창 열고 닫기
        if (Input.GetKeyDown(KeySetting.keys[KeyInput.OPTION]))
        {
            if (SceneManager.GetActiveScene().name == "StartScene")
            {
                soundPanel.gameObject.SetActive(false);
                return;
            }
            soundPanel.gameObject.SetActive(!soundPanel.gameObject.activeSelf);
        }
    }

    public void Initialize()
    {
        AddBGM("진영", BGMClips1);
        AddBGM("시우", BGMClips2);
        AddBGM("영찬", BGMClips3);
        AddBGM("현수", BGMClips4);
        AddBGM("건영", BGMClips5);
        AddBGM("유정", BGMClips6);


        AddSFX("진영", SFXClips1);
        AddSFX("시우", SFXClips2);
        AddSFX("영찬", SFXClips3);
        AddSFX("현수", SFXClips4);
        AddSFX("건영", SFXClips5);
        AddSFX("유정", SFXClips6);

        // 초기 볼륨 적용
        ApplyAllVolumes();
    }

    // BGM
    public void PlayBGM(string name, int index)
    {
        if (!bgmDict.TryGetValue(name, out AudioClip[] clips)) return;
        if (clips == null) return;
        if (index < 0 || index >= clips.Length) return;

        bgmSource.clip = clips[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        //재생중인 BGM이 있을 경우만 멈춤
        if (bgmSource == null) return;
        if (bgmSource.isPlaying) bgmSource.Stop();
    }

    // SFX
    public void PlaySFX(string name, int index)
    {
        if (!sfxDict.TryGetValue(name, out AudioClip[] clips)) return;
        if (clips == null) return;
        if (index < 0 || index >= clips.Length) return;

        sfxSource.clip = clips[index];
        sfxSource.loop = false;
        sfxSource.PlayOneShot(sfxSource.clip);
    }

    //재생중인 BGM있는지 체크
    public bool PlayingBGM()
    {
        if (bgmSource == null) return false;
        if (bgmSource.isPlaying) return true;
        else return false;
    }

    void AddBGM(string name, AudioClip[] clips)
    {
        bgmDict[name] = clips;
    }

    void AddSFX(string name, AudioClip[] clips)
    {
        sfxDict[name] = clips;
    }

    // 마스터 볼륨 조절
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyAllVolumes();
    }

    // 배경음악 볼륨 조절
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        ApplyAllVolumes();
    }

    //효과음 볼륨 조절
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyAllVolumes();
    }

    // 모든 볼륨 설정을 실제 오디오 소스에 적용
    private void ApplyAllVolumes()
    {
        if (bgmSource != null)
        {
            // 최종 BGM 볼륨 = 마스터 볼륨 * BGM 볼륨
            bgmSource.volume = masterVolume * bgmVolume;
        }
        if (sfxSource != null)
        {
            // 최종 SFX 볼륨 = 마스터 볼륨 * SFX 볼륨
            sfxSource.volume = masterVolume * sfxVolume;
        }
    }
}

