using UnityEngine;

public class DungeonDay : MonoBehaviour
{
    int count = 0;
    DungeonManager dungeonManager;

    private void Start()
    {
        dungeonManager = FindAnyObjectByType<DungeonManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (dungeonManager.dayManager.IsDay)
            {
                count++;
                _MasterManager.Instance.DungeonManager.ChangeDay();

                if (count > 1) return;
            }
            else if (dungeonManager.dayManager.IsNight)
            {
                count++;
                _MasterManager.Instance.DungeonManager.ChangeDay();

                if (count > 1) return;
            }
        }
    }
}
