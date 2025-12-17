using UnityEngine;

public class VillageSpawnPoint : MonoBehaviour
{
    Vector3 originalSpawnPoint;
    Vector3 newSpawnPoint = new Vector3(17,0,11);

    private void Awake()
    {
        originalSpawnPoint = transform.position;
    }
    private void OnEnable()
    {
        //던전, 펜던트 등의 복귀일때
        if (_MasterManager.Instance.DataManager.GetReturn())
        {
            _MasterManager.Instance.DataManager.ChangeReturn(false);
            transform.position = newSpawnPoint;
        }
        else if(!_MasterManager.Instance.DataManager.GetReturn())
        {
            transform.position = originalSpawnPoint;
        }
    }
}
