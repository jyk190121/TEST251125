using UnityEngine;
using TMPro;

/// <summary>
/// 마을의 모든 시설 상호작용을 담당
/// 플레이어가 범위 내에 들어오면 프롬프트 표시
/// </summary>
public class FacilityInteraction : MonoBehaviour
{
    [SerializeField] private string facilityID;
    [SerializeField] private float interactionRange = 4f;
    [SerializeField] private string promptText = "대화하기";
    [SerializeField] private GameObject linkedUIPanel;

    // 프롬프트 표시 UI
    private KeyCode interactionKey;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private TextMeshProUGUI promptUIText;
    [SerializeField] private GameObject promptPanel;

    private Transform playerTransform;
    private bool isPlayerInRange = false;

    private void Start()
    {
        // 플레이어 찾기
        VillagePlayerControll playerController = FindAnyObjectByType<VillagePlayerControll>();
        if (playerController != null)
        {
            playerTransform = playerController.transform;
        }
        else
        {
            Debug.LogError("[FacilityInteraction] PlayerController를 찾을 수 없습니다");
        }

        // 프롬프트 초기화
        interactionKey = KeySetting.keys[KeyInput.INTERACTIVE];

        if (promptUIText != null)
            promptUIText.text = promptText;
        if (keyText != null)
        {
            string keyString = KeySetting.GetKeyString(KeyInput.INTERACTIVE);
            keyText.text = keyString;
        }

        HidePrompt();
    }

    private void Update()
    {
        if (playerTransform == null)
            return;

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            playerTransform.position
        );

        // 범위 내/외 확인
        if (distanceToPlayer <= interactionRange)
        {
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;

                VillageSystemManager systemManager = VillageSystemManager.Instance;

                if (!systemManager.IsFacilityUnlocked(facilityID)) return;

                else ShowPrompt();
            }

            // G 키 입력 확인
            if (Input.GetKeyDown(interactionKey))
            {
                OnInteraction();
            }
        }
        else
        {
            if (isPlayerInRange)
            {
                isPlayerInRange = false;
                HidePrompt();
            }
        }
    }

    private void OnInteraction()
    {
        // 상점 입장
        if (facilityID == "shop")
        {
            Debug.Log("[FacilityInteraction] 문라이터 입장!");
            // 나중에: SceneManager.LoadScene("ShopScene");
            GameSceneManager.game.LoadScene("ShopScene");
            return;
        }

        // 던전 입장
        if (facilityID == "dungeon")
        {
            Debug.Log("[FacilityInteraction] 던전 입장!");
            // 나중에: SceneManager.LoadScene("DungeonScene");
            GameSceneManager.game.LoadScene("DungeonTest");
            return;
        }

        // 시설 구매
        if (facilityID == "buy_facilities")
        {
            if (linkedUIPanel != null)
            {
                linkedUIPanel.SetActive(true);
            }
            Debug.Log("[FacilityInteraction] 시설 구매 UI 열기");
            return;
        }

        // 일반 시설 (대장간, 나무 모자)
        VillageSystemManager systemManager = VillageSystemManager.Instance;

        if (systemManager.IsFacilityUnlocked(facilityID))
        {
            // 시설이 해금됨: UI 열기
            if (facilityID == "smithy")
            {
                SmithyUI smithyUI = linkedUIPanel.GetComponent<SmithyUI>();
                smithyUI.OpenUI();
                Debug.Log($"[FacilityInteraction] {facilityID} UI 열기");
            }
            else if (facilityID == "wooden_hat")
            {
                WoodenHatUI woodenHatUI = linkedUIPanel.GetComponent<WoodenHatUI>();
                woodenHatUI.OpenUI();
                Debug.Log($"[FacilityInteraction] {facilityID} UI 열기");
            }
        }
        else
        {
            // 시설이 잠금
            int cost = systemManager.GetFacilityUnlockCost(facilityID);
            string facilityName = systemManager.GetFacilityName(facilityID);
            Debug.Log($"[FacilityInteraction] {facilityName}은 잠금 상태입니다.");
        }
    }

    private void ShowPrompt()
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(true);
        }
    }

    private void HidePrompt()
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
        }
    }
}

