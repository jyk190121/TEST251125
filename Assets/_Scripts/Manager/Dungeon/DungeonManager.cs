using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [HideInInspector]
    public static DungeonManager dungeon;

    [SerializeField]
    private GameObject miniMapPrefab;


    private void Start()
    {
       
    }
   
    void Update()
    {
        
    }

    public void OnMiniMap()
    {
        miniMapPrefab.SetActive(true);
    }

    public void OffMiniMap()
    {
        miniMapPrefab.SetActive(false);
    }

    /*
    void EnterDungeon()
    {
        if (GameSceneManager.game.IsCurrentScene("DungeonTest"))
        {
            OnMiniMap();
        }
        else OffMiniMap();  
    }
    */

    public void Initialize()
    {

    }
}
