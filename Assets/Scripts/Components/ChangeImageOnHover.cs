using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ChangeImageOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Images")]
    [SerializeField] Image image;        // 底圖（可選）
    [SerializeField] Image imageToSwap;  // 會被淡入/淡出的上層圖（排序較高）

    [Header("Timings")]
    [SerializeField] float showDelay = 0.3f;
    [SerializeField] float fadeInDuration = 0.5f;
    [SerializeField] float fadeOutDuration = 0.2f;

    bool isHovering;
    Coroutine delayRoutine;
    Tween currentTween;

    void Awake()
    {
        if (imageToSwap != null)
        {
            // 確保一開始是隱藏狀態
            var c = imageToSwap.color;
            c.a = 0f;
            imageToSwap.color = c;

            // 避免上層圖阻擋到按鈕的指標事件
            imageToSwap.raycastTarget = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovering) return;
        isHovering = true;

        if (delayRoutine != null) StopCoroutine(delayRoutine);
        delayRoutine = StartCoroutine(ShowAfterDelay());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        if (delayRoutine != null)
        {
            StopCoroutine(delayRoutine);
            delayRoutine = null;
        }

        FadeOut();
    }

    void OnDisable()
    {
        isHovering = false;

        if (delayRoutine != null)
        {
            StopCoroutine(delayRoutine);
            delayRoutine = null;
        }

        // 停用時保險起見把顯示狀態清回初始
        currentTween?.Kill();
        if (imageToSwap != null)
        {
            var c = imageToSwap.color;
            c.a = 0f;
            imageToSwap.color = c;
        }
    }

    void OnDestroy()
    {
        currentTween?.Kill();
        currentTween = null;
    }

    System.Collections.IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(showDelay);
        delayRoutine = null;

        if (!isHovering) yield break;
        FadeIn();
    }

    void FadeIn()
    {
        if (imageToSwap == null) return;

        currentTween?.Kill();
        currentTween = imageToSwap
            .DOFade(1f, fadeInDuration)
            .SetEase(Ease.OutQuad)     // OutBack 有回彈；想要更穩定就用 OutQuad
            .SetLink(gameObject);      // 綁定生命週期，避免殘留
    }

    void FadeOut()
    {
        if (imageToSwap == null) return;

        currentTween?.Kill();
        currentTween = imageToSwap
            .DOFade(0f, fadeOutDuration)
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject);
    }
}
