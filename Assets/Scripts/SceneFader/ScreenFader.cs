using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float defaultDuration = 0.35f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        canvasGroup.alpha = 1f;
    }

    public Tween FadeIn(float duration = -1)
    {
        if (duration < 0) duration = defaultDuration;
        canvasGroup.blocksRaycasts = true;

        return canvasGroup.DOFade(1f, duration).SetUpdate(true);
    }

    public Tween FadeOut(float duration = -1)
    {
        if (duration < 0) duration = defaultDuration;

        return canvasGroup.DOFade(0f, duration).SetUpdate(true).OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = false;
        });
    }
}
