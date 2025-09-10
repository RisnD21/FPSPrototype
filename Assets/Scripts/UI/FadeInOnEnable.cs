using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class FadeInOnEnable : MonoBehaviour
{
    [SerializeField] float fadeDuration = 2f;   // Fade time
    [SerializeField] float targetAlpha = 1f;      // Alpha after fade in
    [SerializeField] bool fadeFromZero = true;    // Start from 0 alpha?

    CanvasGroup canvasGroup;
    Tween fadeTween;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        // Reset previous tween
        fadeTween?.Kill();

        if (fadeFromZero)
            canvasGroup.alpha = 0f;

        fadeTween = canvasGroup.DOFade(targetAlpha, fadeDuration)
            .SetUpdate(true); // true = ignore Time.timeScale (optional)
    }

    void OnDisable()
    {
        fadeTween?.Kill();
    }
}
