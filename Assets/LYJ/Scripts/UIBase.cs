using UnityEngine;
using TMPro;

public abstract class UIBase : MonoBehaviour
{
    [SerializeField] protected string panelName;
    protected CanvasGroup canvasGroup;
    protected TextMeshProUGUI titleText;

    public string PanelName => panelName;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // 자식에서 제목 텍스트 찾기
        titleText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Debug.Log($"UI 열림: {panelName}");
    }

    public virtual void Close()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
        Debug.Log($"UI 닫힘: {panelName}");
    }
}

