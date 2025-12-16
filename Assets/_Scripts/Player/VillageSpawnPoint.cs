using UnityEngine;

public class VillageSpawnPoint : MonoBehaviour
{
    Vector3 newSpawnPoint = new Vector3(17,0,11);
    private void OnEnable()
    {
        //던전, 펜던트 등의 복귀일때
        if (_MasterManager.Instance.DataManager.GetReturn())
        {
            transform.position = newSpawnPoint;
        }
    }
}
