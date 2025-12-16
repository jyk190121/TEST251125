using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{
    GameSceneManager gameSceneManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameSceneManager = _MasterManager.Instance.GameSceneManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            gameSceneManager.LoadScene("ShopScene");
        }
    }
}
