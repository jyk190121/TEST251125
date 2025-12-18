using UnityEngine;
using UnityEngine.InputSystem;

public class Rest : MonoBehaviour
{
    PlayerModel model;

    bool isPlayerIn = false;

    int count = 0;

    private void Start()
    {
        model = _MasterManager.Instance.DataManager.GetStat();
    }

    void Update()
    {
        Recovery(100);
    }

    void Recovery(int hp)
    {
        if (isPlayerIn)
        {
            if (count == 0)
            {
                count++;
                _MasterManager.Instance.DataManager.AddHP(hp);
                Debug.Log($"HP회복! 현재 HP:{model.HP}");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = false;
        }
    }
}
