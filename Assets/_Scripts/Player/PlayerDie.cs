using UnityEngine;
using UnityEngine.UIElements;

public class PlayerDie : MonoBehaviour
{
    public GameObject deadPanel;
    GameSceneManager gameSceneManager;

    public void Dead()
    {
        deadPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GoHome()
    {
        _MasterManager.Instance.DataManager.ChangeReturn(true);
        gameSceneManager.LoadScene("Villiage");
        Time.timeScale = 1f;
    }

    public void Retry()
    {
        gameSceneManager.ReloadCurrentScene();
        Time.timeScale = 1f;
    }
}
