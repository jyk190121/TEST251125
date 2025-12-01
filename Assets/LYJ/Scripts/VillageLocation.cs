//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class VillageLocation : MonoBehaviour
//{
//    [SerializeField] private LocationType locationType;
//    [SerializeField] private string locationName;
//    [SerializeField] private string panelName;  // 열어야 할 패널 이름
//    [SerializeField] private Color highlightColor = Color.yellow;

//    private bool playerInRange = false;
//    private TextMeshProUGUI interactionPrompt;
//    private SpriteRenderer spriteRenderer;
//    private Color originalColor;

//    void Start()
//    {
//        // 스프라이트 렌더러 초기화
//        spriteRenderer = GetComponent<SpriteRenderer>();
//        if (spriteRenderer != null)
//        {
//            originalColor = spriteRenderer.color;
//        }

//        // 상호작용 프롬프트 생성
//        CreateInteractionPrompt();
//    }

//    void CreateInteractionPrompt()
//    {
//        // Canvas 생성
//        GameObject canvasObj = new GameObject("InteractionPrompt");
//        canvasObj.transform.SetParent(transform);
//        canvasObj.transform.localPosition = Vector3.up * 2f;

//        Canvas canvas = canvasObj.AddComponent<Canvas>();
//        canvas.renderMode = RenderMode.WorldSpace;

//        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
//        canvasRect.sizeDelta = new Vector2(200, 100);

//        // TextMeshProUGUI 생성
//        GameObject textObj = new GameObject("PromptText");
//        textObj.transform.SetParent(canvasObj.transform);
//        textObj.transform.localPosition = Vector3.zero;

//        interactionPrompt = textObj.AddComponent<TextMeshProUGUI>();
//        interactionPrompt.text = $"[E] {locationName}";
//        interactionPrompt.fontSize = 36;
//        interactionPrompt.alignment = TextAlignmentOptions.Center;
//        interactionPrompt.color = Color.white;

//        // 그림자 효과 추가
//        Shadow shadow = textObj.AddComponent<Shadow>();
//        shadow.effectColor = Color.black;
//        shadow.effectDistance = new Vector2(2, -2);

//        // 아웃라인 효과 추가 (선택사항)
//        Outline outline = textObj.AddComponent<Outline>();
//        outline.effectColor = Color.black;
//        outline.effectDistance = new Vector2(0.2f, -0.2f);

//        RectTransform textRect = textObj.GetComponent<RectTransform>();
//        textRect.sizeDelta = new Vector2(400, 100);

//        canvasObj.SetActive(false);
//    }

//    void Update()
//    {
//        if (playerInRange && Input.GetKeyDown(KeyCode.E))
//        {
//            InteractWithLocation();
//        }
//    }

//    void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            playerInRange = true;
//            ShowInteractionPrompt(true);
//            HighlightLocation(true);
//            Debug.Log($"{locationName}에 도착했습니다");
//        }
//    }

//    void OnTriggerExit2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            playerInRange = false;
//            ShowInteractionPrompt(false);
//            HighlightLocation(false);
//            Debug.Log($"{locationName}을 떠났습니다");
//        }
//    }

//    void ShowInteractionPrompt(bool show)
//    {
//        if (interactionPrompt != null)
//        {
//            interactionPrompt.transform.parent.gameObject.SetActive(show);
//        }
//    }

//    void HighlightLocation(bool highlight)
//    {
//        if (spriteRenderer != null)
//        {
//            spriteRenderer.color = highlight ? highlightColor : originalColor;
//        }
//    }

//    void InteractWithLocation()
//    {
//        Debug.Log($"{locationName} 상호작용");

//         //UIManager를 통해 패널 열기
//        if (MasterManager.Instance != null)
//        {
//            MasterManager.Instance.UIManager.OpenPanel(panelName);
//            ShowInteractionPrompt(false);  // 프롬프트 숨기기
//        }
//    }

//    public LocationType GetLocationType() => locationType;
//}


