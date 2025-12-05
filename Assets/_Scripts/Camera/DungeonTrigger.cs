using UnityEngine;

public class DungeonTrigger : MonoBehaviour
{
    public enum MoveDir { Left, Right, Up, Down }
    public MoveDir moveDirection;

    public ParticleSystem portalEffect;
    private MeshRenderer portalRenderer;

    
    private void Start()
    {
        // [코드 설명] 이 스크립트가 붙어있는 오브젝트에서 MeshRenderer 컴포넌트를 가져옵니다.
        // [코드 목적] 포털이 닫혔을 때 렌더러의 색상이나 활성화를 제어하기 위함입니다.
        portalRenderer = GetComponent<MeshRenderer>();

        // 초기 상태를 한 번 확인합니다.
        CheckPortalStatus();
    }

    private void Update()
    {
        //CheckPortalStatus();
    }

    private void CheckPortalStatus()
    {
        // [if문 설명] DungeonManager의 isCleared 상태를 확인합니다.
        if (RoomController.isCleared)
        {
            // 클리어 됨: 문 열림 상태

            // [코드 설명] 인스펙터에 연결된 파티클 시스템이 존재하고, 현재 재생 중이 아니라면 재생을 시작합니다.
            // [코드 목적] 포털이 열렸음을 화려한 이펙트로 표시합니다.
            if (portalEffect != null && !portalEffect.isPlaying)
            {
                portalEffect.Play();
            }

            // [코드 설명] 포털 MeshRenderer의 색상을 밝게 (예: 녹색) 변경합니다.
            if (portalRenderer != null)
            {
                // 문이 열렸음을 시각적으로 표시 (예: Emission을 켜거나, 색상을 변경)
                portalRenderer.material.color = Color.green;
            }
        }
        else
        {
            // 클리어 안 됨: 문 닫힘 상태

            // [코드 설명] 파티클 시스템이 존재하고 재생 중이라면 정지시킵니다.
            // [코드 목적] 문이 닫혀있으므로 이펙트를 보여줄 필요가 없습니다.
            if (portalEffect != null && portalEffect.isPlaying)
            {
                portalEffect.Stop();
            }

            // [코드 설명] 포털 MeshRenderer의 색상을 어둡게 (예: 붉은색) 변경합니다.
            if (portalRenderer != null)
            {
                portalRenderer.material.color = Color.black;
            }
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (RoomController.isCleared)
            {
                DungeonCamera cam = Camera.main.GetComponent<DungeonCamera>();
                DungeonPortal player = other.GetComponent<DungeonPortal>();

                switch (moveDirection)
                {
                    case MoveDir.Left:
                        cam.LeftMove();
                        player.LeftMove(other);
                        break;
                    case MoveDir.Right:
                        cam.RightMove();
                        player.RightMove(other);
                        break;
                    case MoveDir.Up:
                        cam.UpMove();
                        player.UpMove(other);
                        break;
                    case MoveDir.Down:
                        cam.DownMove();
                        player.DownMove(other);
                        break;
                }

                Debug.Log("던전이 클리어되어 플레이어가 이동했습니다.");

            }

            else
            {
                Debug.Log("던전을 클리어해야만 이 문을 통과할 수 있습니다!");
            }
        }
    }
}
