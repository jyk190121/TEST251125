using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [HideInInspector]
    public static GameSceneManager game;

    //private static SceneManager instance;
    //public static SceneManager Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = FindAnyObjectByType<SceneManager>();
    //            if (instance == null)
    //            {
    //                GameObject obj = new GameObject("SceneManager");
    //                instance = obj.AddComponent<SceneManager>();
    //            }
    //        }
    //        return instance;
    //    }
    //}

    //void Awake()
    //{
    //    if (instance != null && instance != this)
    //    {
    //        Destroy(instance);
    //        return;
    //    }
    //    instance = this;
    //    DontDestroyOnLoad(gameObject);
    //}

    void Awake()
    {
        game = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void LoadScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        LoadScene(currentSceneName);
    }

    public bool IsCurrentScene(string sceneName)
    {
        // 결과가 같으면 true, 다르면 false를 반환(return)합니다.
        return SceneManager.GetActiveScene().name.Equals(sceneName);
    }

}