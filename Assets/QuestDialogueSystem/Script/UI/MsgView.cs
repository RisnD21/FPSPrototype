using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
public class MsgView : MonoBehaviour
{
    const int ViewAmounts = 7;

    [Header("References")]
    [SerializeField] TextMeshProUGUI msgBlock;
    [SerializeField] Vector2 initialPos; // 使用 localPosition
    [SerializeField] float lineHeight;

    // Slots 位置表
    Vector2[] viewSlots = new Vector2[ViewAmounts];
    int targetSlotIndex;

    CanvasGroup canvasGroup;
    RectTransform rectTransform;

    // 狀態
    bool isFadingOut;
    bool doneDisplaying;

    public bool IsFadingOut => isFadingOut;
    public bool DoneDisplaying => doneDisplaying;

    public float displayedTime;
    public bool ShouldFade => displayedTime > 2;

    // Tween 控制
    Tween movingTween;
    Tween fadingTween;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        EstablishViewSlots();
        Reset();
    }

    void EstablishViewSlots()
    {
        viewSlots[0] = initialPos;
        for (int i = 1; i < ViewAmounts; i++)
        {
            viewSlots[i] = viewSlots[i - 1] + new Vector2(0, lineHeight);
        }
    }

    public void Reset()
    {
        isFadingOut = false;
        doneDisplaying = false;
        displayedTime = 0;

        msgBlock.text = "";
        targetSlotIndex = 0;

        movingTween?.Kill();
        fadingTween?.Kill();

        canvasGroup.alpha = 0;
        rectTransform.anchoredPosition = viewSlots[targetSlotIndex];
    }

    public void SetText(string text)
    {
        msgBlock.text = text;
    }

    public void MoveDownward(float duration)
    {
        movingTween?.Kill();
        targetSlotIndex = Mathf.Clamp(targetSlotIndex + 1, 0, ViewAmounts - 1);
        movingTween = rectTransform.DOAnchorPos(viewSlots[targetSlotIndex], duration);
    }

    public void FadeIn(float duration)
    {
        fadingTween?.Kill();
        fadingTween = canvasGroup.DOFade(1, duration);
    }

    public void FadeOut(float duration)
    {
        isFadingOut = true;
        fadingTween?.Kill();
        fadingTween = canvasGroup
            .DOFade(0, duration)
            .OnComplete(() => doneDisplaying = true);
    }
}
