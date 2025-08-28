using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class TitleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI titlePanel;

    CanvasGroup canvasGroup;
    Sequence seq;

    [Header("Durations")]
    [SerializeField] float fadeInDuration = 0.2f;
    [SerializeField] float displayDuration = 3f;
    [SerializeField] float fadeOutDuration = 0.2f;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        Reset();
    }

    void Reset()
    {
        seq?.Kill();
        canvasGroup.alpha = 0;
        titlePanel.text = "";
    }

    public void PrintText(string text)
    {
        // 如果還在播放，直接 Reset 後重播
        if (seq != null && seq.IsActive()) Reset();

        titlePanel.text = text;

        seq = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1, fadeInDuration))
            .AppendInterval(displayDuration)
            .Append(canvasGroup
                .DOFade(0, fadeOutDuration)
                .OnComplete(Reset));
    }
}
