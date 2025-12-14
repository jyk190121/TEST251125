using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    // 사운드를 재생할 오디오 소스 컴포넌트 (MP3 플레이어 역할)
    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    // 사운드 파일 자체 (Mp3 파일 등)
    [Header("Audio Clips")]
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;

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
        foreach (AudioClip clip in bgmClips)
        {
            bgmDict.Add(clip.name, clip);
        }

        foreach (AudioClip clip in sfxClips)
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

    public void PlayBGMIndex(int index)
    {
        if (index < 0 || index >= bgmClips.Length) return;

        bgmSource.clip = bgmClips[index];
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

    public void PlaySFXIndex(int index)
    {
        if (index < 0 || index >= sfxClips.Length) return;

        sfxSource.clip = sfxClips[index];
        sfxSource.loop = false;
        sfxSource.Play();
    }

}

