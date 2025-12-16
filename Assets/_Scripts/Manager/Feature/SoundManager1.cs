using UnityEngine;
using System.Collections.Generic;

public class SoundManager1 : MonoBehaviour
{
    // 사운드를 재생할 오디오 소스 컴포넌트 (MP3 플레이어 역할)
    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    // 사운드 파일 자체 (Mp3 파일 등)
    [Header("Audio Clips")]
    public AudioClip[] playerSFXClips;
    public AudioClip[] monsterSFXClips;

    public AudioClip[] villiageBGMClips;
    public AudioClip[] shopBGMClips;
    public AudioClip[] dungeonBGMClips;

    [Header("볼륨 설정")]
    [Range(0f, 1f)]
    public float masterVolume;      // 마스터 볼륨
    [Range(0f, 1f)]
    public float bgmVolume;         // BGM 볼륨
    [Range(0f, 1f)]
    public float sfxVolume;         // SFX 볼륨


    Dictionary<string, AudioClip> bgmDict = new();
    Dictionary<string, AudioClip> sfxDict = new();

    public void Initialize()
    {
        foreach (AudioClip clip in villiageBGMClips)
        {
            bgmDict.Add(clip.name, clip);
        }

        foreach (AudioClip clip in shopBGMClips)
        {
            bgmDict.Add(clip.name, clip);
        }

        foreach (AudioClip clip in dungeonBGMClips)
        {
            bgmDict.Add(clip.name, clip);
        }

        foreach (AudioClip clip in playerSFXClips)
        {
            sfxDict.Add(clip.name, clip);
        }

        foreach (AudioClip clip in monsterSFXClips)
        {
            sfxDict.Add(clip.name, clip);
        }
    }

    // BGM
    public void PlayBGM(string name)
    {
        if (!bgmDict.ContainsKey(name)) return;

        bgmSource.clip = bgmDict[name];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayVilliageBGMIndex(int index)
    {
        if (index < 0 || index >= villiageBGMClips.Length) return;

        bgmSource.clip = villiageBGMClips[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayShopBGMIndex(int index)
    {
        if (index < 0 || index >= shopBGMClips.Length) return;

        bgmSource.clip = shopBGMClips[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayDungeonBGMIndex(int index)
    {
        if (index < 0 || index >= dungeonBGMClips.Length) return;

        bgmSource.clip = dungeonBGMClips[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        //재생중인 BGM이 있을 경우만 멈춤
        if(bgmSource.isPlaying) bgmSource.Stop();
    }

    // SFX
    public void PlaySFX(string name)
    {
        if (!sfxDict.ContainsKey(name)) return;

        sfxSource.PlayOneShot(sfxDict[name]);
    }

    public void PlayPlayerSFXIndex(int index)
    {
        if (index < 0 || index >= playerSFXClips.Length) return;

        sfxSource.clip = playerSFXClips[index];
        sfxSource.loop = false;
        sfxSource.Play();
    }

    public void PlayMonsterSFXIndex(int index)
    {
        if (index < 0 || index >= monsterSFXClips.Length) return;

        sfxSource.clip = monsterSFXClips[index];
        sfxSource.loop = false;
        sfxSource.Play();
    }

    //재생중인 BGM있는지 체크
    public bool PlayingBGM()
    {
        if (bgmSource.isPlaying) return true;
        else return false;
    }
}

