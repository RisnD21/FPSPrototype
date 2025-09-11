using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class ReinforceIcon : MonoBehaviour
{
    [SerializeField] SpriteRenderer imageObj;
    [SerializeField] float displayDuration = 2;
    [SerializeField] float fadeInDuration = 0.1f;
    [SerializeField] float fadeOutDuration = 0.5f;

    Color initColor;
    Coroutine routine;

    void Awake()
    {
        // 萬一忘了拖引用，嘗試就地取材
        if (imageObj == null) imageObj = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        initColor = imageObj.color;
        // 不能直接改 color.a（Color 是值型別），要整個回寫
        var c = imageObj.color;
        c.a = 0f;
        imageObj.color = c;
    }

    public void CallReinforcement()
    {
        // 先殺掉舊 tween & 協程，避免重入
        imageObj.DOKill();
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        routine = StartCoroutine(CallHelp());
    }

    IEnumerator CallHelp()
    {
        // 少了分號；另外用 WaitForCompletion 讓流程更可控
        yield return imageObj.DOFade(1f, fadeInDuration)
            .SetLink(gameObject)
            .WaitForCompletion();

        yield return new WaitForSeconds(displayDuration);

        yield return imageObj.DOFade(0f, fadeOutDuration)
            .SetLink(gameObject)
            .WaitForCompletion();

        HideIcon();
    }

    void HideIcon()
    {
        var c = imageObj.color;
        c.a = 0f;
        imageObj.color = c;
    }

    void OnDestroy()
    {
        // Coroutine 沒有 kill；用 StopCoroutine / StopAllCoroutines
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
        // 把綁在這個物件上的 tween 全部清掉
        if(imageObj != null) imageObj.DOKill();
    }
}