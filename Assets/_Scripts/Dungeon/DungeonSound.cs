using UnityEngine;

public class DungeonSound : MonoBehaviour
{
    SoundManager soundManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundManager = _MasterManager.Instance.SoundManager;
        DungeonBGM();
    }

    void DungeonBGM()
    {
        print("사운드체크");
        soundManager.StopBGM();
        soundManager.PlayBGM("영찬", 0);
    }

}
