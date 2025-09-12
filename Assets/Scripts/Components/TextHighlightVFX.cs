using TMPro;
using DG.Tweening;
using UnityEngine;


[RequireComponent(typeof(TMP_Text))]
public class TextHighlightVFX : MonoBehaviour
{
    TMP_Text targetText;
    [SerializeField] float fadeInDuration = 1;
    [SerializeField] float waitDuration = 0;
    [SerializeField] float fadeOutDuration = 1f; // 每次淡入淡出的時間
    [SerializeField] bool loops = true;
    int Loop => loops == true? -1: 0;
    Tween currentTween;

    void Awake() 
    {
        targetText = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        targetText.alpha = 0f;

        Sequence seq = DOTween.Sequence();
        seq.Append(targetText.DOFade(1f, fadeInDuration)
                .SetLoops(Loop, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true));
        seq.AppendInterval(waitDuration);
        seq.Append(targetText.DOFade(0f, fadeOutDuration)
                .SetLoops(Loop, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true));
        
        currentTween = seq.SetLink(gameObject);
    }

    void OnDisable() 
    {
        if(currentTween != null);
        currentTween?.Kill();
        currentTween = null;
    }
}
