using UnityEngine;
using static DayManager;

public class DungeonManager : MonoBehaviour
{
    [HideInInspector]
    public static DungeonManager dungeon;

    [SerializeField]
    private GameObject miniMapPrefab;

    public DayManager dayManager;


    private void Start()
    {
        dayManager = FindAnyObjectByType<DayManager>();
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

    //낯 밤 변경
    public void ChangeDay()
    {
        if (dayManager.IsDay)
        {
            dayManager.ChangeTimeOfDay(TimeOfDay.Night);
        }
        else if (dayManager.IsNight)
        {
            dayManager.ChangeTimeOfDay(TimeOfDay.Day);
        }
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
