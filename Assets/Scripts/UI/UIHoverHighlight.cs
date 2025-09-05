using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening; 

[RequireComponent(typeof(Image))]
public class UIHoverHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    Image baseImage;        // 原始玉佩
    [SerializeField] Image glowOverlay;      // 發光覆蓋層 (預設 alpha=0、raycastTarget=false）

    [Header("Glow Fade")]
    [SerializeField] bool fadeGlow = true;
    [SerializeField] float fadeDuration = 0.15f;

    // [Header("Events (Optional)")]
    // public UnityEvent onClick;
    //active if we need to click the obj

    Tween glowTween;

    void Reset() { baseImage = GetComponent<Image>(); }
    void OnEnable() => SetHighlight(false);

    void Awake()
    {
        baseImage = GetComponent<Image>();
        baseImage.raycastTarget = true;

        if (glowOverlay != null)
        {
            glowOverlay.raycastTarget = false;          
            var c = glowOverlay.color; 
            c.a = 0f; 
            glowOverlay.color = c;
            glowOverlay.enabled = true;  // 常駐啟用，只改 alpha
        }
    }

    public void OnPointerEnter(PointerEventData _) => SetHighlight(true);
    public void OnPointerExit (PointerEventData _) => SetHighlight(false);

    // public void OnPointerClick(PointerEventData _) => onClick?.Invoke();
    //active if we need to click the obj

    public void SetHighlight(bool on)
    {
        if (glowOverlay != null)
        {
            if (fadeGlow)
            {
                glowTween?.Kill(); // 停止上一次的 Tween
                glowTween = glowOverlay.DOFade(on ? 1f : 0f, fadeDuration)
                                       .SetUpdate(true); // 用 unscaledDeltaTime
            }
            else
            {
                var c = glowOverlay.color;
                c.a = on ? 1f : 0f;
                glowOverlay.color = c;
            }
        }
    }

    void OnDestroy() //incase
    {
        glowTween?.Kill();
    }
}
