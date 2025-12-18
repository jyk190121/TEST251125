using UnityEngine;
using UnityEngine.InputSystem;

public class Rest : MonoBehaviour
{
    PlayerModel model;

    bool isPlayerIn = false;

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
            Debug.Log($"HP회복! 현재 HP:{model.HP}");
            _MasterManager.Instance.DataManager.AddHP(hp);
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
