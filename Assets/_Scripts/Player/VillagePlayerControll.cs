using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

//PlayerMove에서 입력받은 값에 따라 실행되는 함수를 정리한 스크립트
public class VillagePlayerControll : MonoBehaviour
{
    CharacterController CC;
    PlayerModel model;

    void Start()
    {
        CC = GetComponent<CharacterController>();
        model = _MasterManager.Instance.DataManager.GetStat();
        Debug.Log("모델" + model);
    }

    //장비 장착시 호출! player의 스탯값 변경!
    public void RefreshStat()
    {
        model = _MasterManager.Instance.DataManager.GetStat();
    }

    public void Move(Vector3 dir)
    {
        if (dir != Vector3.zero)
        {
            // 이동
            CC.Move(dir * model.moveSpeed * Time.deltaTime);
        }
    }
}
