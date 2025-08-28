using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [SerializeField] RectTransform panel;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Vector2 offset = new(16, -16);

    bool isVisible;

    void Awake() => Instance = this;

    void Start()
    {
        panel.gameObject.SetActive(false);
        isVisible = false;
    }

    public void Show(string tooltipText, Vector2 screenPos)
    {
        if (string.IsNullOrEmpty(tooltipText)) return;
        text.text = tooltipText;
        panel.gameObject.SetActive(true);
        isVisible = true;
        UpdatePosition(screenPos);
    }

    public void Hide()
    {
        if (!isVisible) return;
        panel.gameObject.SetActive(false);
        isVisible = false;
    }

    public void UpdatePosition(Vector2 screenPos)
    {
        if (!isVisible) return;
        panel.position = screenPos + offset;
    }
}