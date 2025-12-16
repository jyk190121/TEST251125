using UnityEngine;

public class UIManager : MonoBehaviour
{
    private PlayerUI uiPanel;

    private void Start()
    {
        uiPanel = FindAnyObjectByType<PlayerUI>();

        if (uiPanel == null)
            Debug.LogError("[UIManager] UIPanel을 찾을 수 없습니다");
    }

    public void SetPlayerUIActive(bool active)
    {
        SetLeftPanelActive(active);
        SetRightPanelActive(active);
    }

    public void SetLeftPanelActive(bool active) => uiPanel?.SetLeftPanelActive(active);
    public void SetRightPanelActive(bool active) => uiPanel?.SetRightPanelActive(active);


    public void Initialize()
    {
        // 초기 패널 상태 설정
        SetLeftPanelActive(false);
        SetRightPanelActive(false);

        Debug.Log("[UIManager] 초기화 완료");
    }
}

