using TMPro;
using DG.Tweening;
using UnityEngine;


[RequireComponent(typeof(TMP_Text))]
public class TextHighlightVFX : MonoBehaviour
{
    TMP_Text targetText;
    [SerializeField] float fadeDuration = 2f; // 每次淡入淡出的時間
    [SerializeField] bool loops = true;
    int Loop => loops == true? -1: 0;

    void Awake() 
    {
        targetText = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        targetText.alpha = 1f;

        // 建立閃爍效果：1 → 0 → 1
        targetText.DOFade(0.5f, fadeDuration)
            .SetLoops(Loop, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);
    }
}
