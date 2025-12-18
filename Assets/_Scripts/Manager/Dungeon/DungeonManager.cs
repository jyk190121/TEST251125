using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [SerializeField]
    private GameObject miniMapPrefab;

    private void Start()
    {
        EnterDungeon();
    }
   
    void Update()
    {
        
    }

    void OnMiniMap()
    {
        miniMapPrefab.SetActive(true);
    }

    void OffMiniMap()
    {
        miniMapPrefab.SetActive(false);
    }

    void EnterDungeon()
    {
        if (GameSceneManager.game.IsCurrentScene("DungeonTest"))
        {
            OnMiniMap();
        }
        else OffMiniMap();  
    }

    public void Initialize()
    {

    }
}
