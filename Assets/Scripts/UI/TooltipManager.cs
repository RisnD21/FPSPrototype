using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [SerializeField] GameObject defaultTooltipPanel;
    [SerializeField] TextMeshProUGUI defaultTextBlock;
    [SerializeField] GameObject prolongTooltipPanel;
    [SerializeField] TextMeshProUGUI prolongTextBlock;

    [SerializeField] int swapSizeThreshold = 40;

    GameObject currentPanel;
    TextMeshProUGUI textBlock;

    [SerializeField] Vector2 offset = new(16, -16);

    bool isVisible;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        defaultTooltipPanel.SetActive(false);
        prolongTooltipPanel.SetActive(false);
        isVisible = false;
    }

    public void Show(string tooltipText, Vector2 screenPos)
    {
        if (string.IsNullOrEmpty(tooltipText)) return;
        if(tooltipText.Length < swapSizeThreshold) UseDefaultTooltip();
        else UseProlongTooltip();

        textBlock.text = tooltipText;

        currentPanel.gameObject.SetActive(true);
        isVisible = true;
        UpdatePosition(screenPos);
    }

    void UseDefaultTooltip()
    {
        currentPanel = defaultTooltipPanel;
        textBlock = defaultTextBlock;
    }

    void UseProlongTooltip()
    {
        currentPanel = prolongTooltipPanel;
        textBlock = prolongTextBlock;
    }

    public void Hide()
    {
        if (!isVisible) return;
        currentPanel.SetActive(false);
        textBlock.text = "";
        isVisible = false;
    }

    public void UpdatePosition(Vector2 screenPos)
    {
        if (!isVisible) return;
        currentPanel.transform.position = screenPos + offset;
    }
}